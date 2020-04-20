# Table Configration Instructions

Table configuration is stored in JSON file located inside the application schema folder 
```  
<apiFolder>\App_Data\schema\config  
filenaming convention: config.table.<table code>.json
```
##### Examples:
###### For Anomalies table, the config file must contain the following `(<apiFolder>\schema\config\config.table.anom.json)`
```JSON

{
    "tableName" : "tbl_Anomalies",
    "tableFieldPrefix":"an_",
    "tableClassFilename":"",
    "tableClass":"TblAnomalies",
    ```diff -"links":[] ```,
    "tableRowClass":"TblAnomaliesRow",
    "columns": [
        {"name":"an_id","type":"Int64","keyPosition":0},
    ],
    "captions":{
    },
    "tableCode":"anom",
    "description":"Anomalies Table"
}
```
###### For Change Tracking table, the config file must contain the following `(<apiFolder>\schema\config.table.chgTrack.json)`
```JSON

{
    "tableName" : "tbl_ChangeTracker",
    "tableFieldPrefix":"trk_",
    "tableClassFilename":"",
    "tableClass":"TblChangeTracker",
    "links":[],
    "tableRowClass":"TblChangeTrackerRow",
    "columns": [
        {"name":"trk_id","type":"Int64","keyPosition":0}
    ],
    "captions":{
    },
    "tableCode":"chgTrack",
    "description":"Change tracking collection table"
}

```


Open [README.md](https://github.com/izyte/NgArbiServer/blob/master/README.md)
