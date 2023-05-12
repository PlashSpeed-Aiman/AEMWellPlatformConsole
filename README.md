# AEMWellPlatformConsole
### NOTE:The .env file I used only contain username,password, and api link. I don't feel comfortable letting Github reading the credentials for the API

## Time Taken 
Approximately 12 Hours accumulated over the span of 5 days \
Reason:
1. Half of the time spent were on planning the programme, and studying EF Core as I don't usually use ORMs in most of my projects. I usually write my own SQL Queries. 
2. As I have 2 part time jobs, so I only managed to code during the night

## Problems Encountered & Solution
### Most of the problems stemmed from using EF Core, but I have learned a lot from doing this assessment
1. Identity_Insert OFF error for both tables \
    i.  Used Database.SQLCommand with SET IDENTITY_INSERT OFF \
    ii. Handled insert for both tables separately since each table has its own PK \
    iii. Not the best solution, but it works 
    
2. API return different set of data \
    i. Make the keys nullable \
    ii. When updating, I opted for using the current value if new value is null after deserializing the JSON string 
    
## SQL QUERY USED:
```SQL
SELECT PlatformName, PlatformId, UniqueName, Latitude, Longitude, CreatedAt, UpdatedAt
FROM (
    SELECT p.UniqueName AS PlatformName, w.PlatformId, w.UniqueName, w.Latitude, w.Longitude, w.CreatedAt, w.UpdatedAt,
           ROW_NUMBER() OVER (PARTITION BY w.PlatformId ORDER BY w.UpdatedAt DESC) AS RowNum
    FROM Wells w
    INNER JOIN Platforms p ON w.PlatformId = p.Id
) AS subquery
WHERE RowNum = 1;
```
![image](https://github.com/PlashSpeed-Aiman/AEMWellPlatformConsole/assets/62431177/10679901-2194-48fe-af2d-f76f04c6a1f0)
