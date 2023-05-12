// See https://aka.ms/new-console-template for more 


using System.Diagnostics;
using AEMWellPlatformConsole.Utils;
using AEMWellPlatformConsole.ViewModel;

class Program
{
    public  static async Task Main(string[] args)
    {
        Console.WriteLine("HELLO WORLD");
        var vm = new SyncDataViewModel();
        var res =  vm.GetBearerToken();
        if (res.Success)
        {
            Console.WriteLine("Syncing Data");
            var listPlats =  await vm.GetJsonData();
            await vm.SyncData(listPlats);
            
        }
        else if (res is ErrorResult err)
        {
            Console.WriteLine(err.Message);
        }
        Console.WriteLine("PRESS ANY KEY TO CONTINUE");
        Console.ReadKey();
    }
}