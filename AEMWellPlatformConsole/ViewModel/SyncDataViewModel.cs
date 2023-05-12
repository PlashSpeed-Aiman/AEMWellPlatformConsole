using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AEMWellPlatformConsole.Data;
using AEMWellPlatformConsole.Model;
using AEMWellPlatformConsole.Utils;
using Microsoft.EntityFrameworkCore;

namespace AEMWellPlatformConsole.ViewModel;

public class SyncDataViewModel
{
    private string? API_LINK = "";
    private const string LOGIN = "Account/Login";
    private const string PLATFORM_ACTUAL = "PlatformWell/GetPlatformWellActual";
    private const string PLATFORM_DUMMY = "PlatformWell/GetPlatformWellDummy";

    private HttpClient _httpClient;
    private  string? PASSWORD = "";
    private  string? USERNAME = "";
    //NOTE TO SELF: Context represents a session
    private PlatformWellContext dbContext; 
    public SyncDataViewModel()
    {
        var root = Directory.GetCurrentDirectory();
        var dotenv = Path.Combine(root, ".env");
        Debug.WriteLine(dotenv);
        DotEnv.Load(dotenv);
        API_LINK = Environment.GetEnvironmentVariable("AEM_API") ?? "";
        USERNAME = Environment.GetEnvironmentVariable("AEM_USERNAME") ?? "";
        PASSWORD = Environment.GetEnvironmentVariable("AEM_PASSWORD") ?? "";
        _httpClient= SetupClient();
        dbContext = new PlatformWellContext();
    }
    
    public async Task<List<Platform>> GetJsonData()
    {
        var res = await _httpClient.GetAsync(API_LINK + PLATFORM_ACTUAL);
        var resContent = await  res.Content.ReadAsStringAsync();
        Debug.WriteLine(_httpClient.DefaultRequestHeaders.Authorization);
        Debug.WriteLine(res.StatusCode.ToString());
        Debug.WriteLine(resContent);
        var test = JsonSerializer.Deserialize<List<Platform>>(resContent,new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        if (test != null)
        {
            foreach (var tes in test)
            {
                Debug.WriteLine(tes.CreatedAt);
                foreach (var well in tes.Wells)
                {
                    Debug.WriteLine(well.CreatedAt);
                }
            }

            return test;
        }

        return new List<Platform>();
    }

    public async Task SyncData(List<Platform> platforms)
    {
        using var transaction = dbContext.Database.BeginTransaction();

        foreach (var plat in platforms)
        {
            dbContext.Database.ExecuteSql($"SET IDENTITY_INSERT dbo.Platforms ON");
            var found_plat = dbContext.Platforms.SingleOrDefault(x => x.Id == plat.Id);
            if (found_plat != null)
            {
                found_plat.Id = plat.Id;
                found_plat.UniqueName = plat.UniqueName;
                found_plat.Longitude = plat.Longitude;
                found_plat.Latitude = plat.Latitude;
                found_plat.CreatedAt = plat.CreatedAt ?? found_plat.CreatedAt;
                found_plat.UpdatedAt = plat.UpdatedAt ?? found_plat.UpdatedAt;
                dbContext.Platforms.Update(found_plat);
            }
            else
            {
                var newplat = new Platform();
                newplat.Id = plat.Id;
                newplat.Latitude = plat.Latitude;
                newplat.Longitude = plat.Longitude;
                newplat.UniqueName = plat.UniqueName;
                newplat.CreatedAt = plat.CreatedAt;
                newplat.UpdatedAt = plat.UpdatedAt;
                newplat.Wells = new List<Well>();
                dbContext.Platforms.Add(newplat);
            }
           
        }
        dbContext.SaveChanges();
        dbContext.Database.ExecuteSql($"SET IDENTITY_INSERT dbo.Platforms OFF");


        #region  insertWell
        dbContext.Database.ExecuteSql($"SET IDENTITY_INSERT dbo.Wells ON");
        foreach (var varPlatform in platforms)
        {
            foreach (var varWell in varPlatform.Wells)
            {
                var found_well = dbContext.Wells.SingleOrDefault(x => x.Id == varWell.Id);
                if (found_well != null)
                {
                    found_well.Id = varWell.Id;
                    found_well.PlatformId = varWell.PlatformId;
                    found_well.UniqueName = varWell.UniqueName;
                    found_well.Longitude = varWell.Longitude;
                    found_well.Latitude = varWell.Latitude;
                    found_well.CreatedAt = varWell.CreatedAt ?? found_well.CreatedAt;
                    found_well.UpdatedAt = varWell.UpdatedAt ?? found_well.UpdatedAt;
                    
                }
                else
                {
                    dbContext.Wells.Add(varWell);    
                }
                
            }
            
           
        }
      
        dbContext.SaveChanges();
        dbContext.Database.ExecuteSql($"SET IDENTITY_INSERT dbo.Wells OFF");
        
        #endregion
        
        transaction.Commit();
    }
    
    public Result GetBearerToken()
    {
        HttpResponseMessage result;
        var dict = new Dictionary<string, string>()
        {
            {"username",USERNAME},
            {"password",PASSWORD}
        };
        var jsonObj = JsonSerializer.Serialize(dict);
        try
        {
            result = _httpClient.PostAsync(API_LINK + LOGIN, new StringContent(jsonObj, Encoding.UTF8, "application/json")).Result;
            Debug.WriteLine(jsonObj);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return new ErrorResult(ex.ToString());
        }

        Debug.WriteLine(result.Content.ReadAsStringAsync().Result);
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.Content.ReadAsStringAsync().Result.Trim('"'));
        return new SuccessResult();

    }

    public HttpClient SetupClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla Firefox");
        return client;
    }
}