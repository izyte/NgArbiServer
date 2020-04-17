# Table Configration Instructions

Table configuration is stored in JSON file located inside the application schema folder 
```  
<apiFolder>\App_Data\schema\config  

filnaming convention: config.table.<table code>.json

eg.  <apiFolder>\schema\config\config.table.anom.json
```

###### The config file nust contain the following
```JSON

{
    "tableName" : "tbl_Anomalies",
    "tableFieldPrefix":"an_",
    "tableClassFilename":"",
    "tableClass":"TblAnomalies",
    "links":[],
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
