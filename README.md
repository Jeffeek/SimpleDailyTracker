# SimpleDailyTracker
In the repository you can see the "sampledata" folder, which contains 5 test files, with which you can see the functionality of the program
---
SimpleDailyTracker.UI folder contains the json file AppConfiguration.json, which contains information for automating the import and export of data:
```json
{
  "DirectorySettings": {
    "ImportDirectory": "directory", // D:\\DailyInfo\\Import\\
    "ExportDirectory": "directory" // D:\\DailyInfo\\Export\\
  },
  "FileSearch": {
    "Extension": ".json", // EXTENSION OF IMPORT FILES
    "SearchPattern": "day*.json", // SEARCH PATTERN TO FIND FILES FOR IMPORT
    "DayFinderRegex": "day([\\d]+)\\.json$" // REGULAR EXPRESSION WHICH IS USED TO FIND DAY OF FILE (FOR GROUPING DATA IN CHART)
  } 
}
```
---
1. Refresh button: imports all files from ImportDirectory
2. Add button: select files for import

![Overall interface preview](https://i.ibb.co/Jv5Z5tX/image.png)
---
1. At the left you can see all users with: Name, Average Steps Count, Best Steps Count, Worst Steps Count
2. At th right a chart by day for selected user (red dot shows the highest result of all time for this user)
3. At users list some users are highlited with orange color (users whose best or worst results differ from the average number of steps for the entire period (for this user) by more than 20%)

![Activity interface](https://i.ibb.co/pZhJxzv/image.png)
---
Application supports 3 types of export: XML, CSV and JSON:

![Export interface](https://i.ibb.co/ccBhpzy/image.png)
1. Select user for export
2. Select type of export
3. Click on button
4. Look at ExportDirectory (configuring in AppConfiguration.json)
