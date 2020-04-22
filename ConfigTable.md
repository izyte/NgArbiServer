# Table Configration Instructions

Table configuration is stored in JSON file located inside the application schema folder 
```  
<apiFolder>\App_Data\schema\config  
filenaming convention: config.table.<table code>.json
```

#### Definitions:
```json
{
    "tableName" : "tbl_Anomalies",
    "tableFieldPrefix":"AN_",
    "tableClassFilename":"",
    "tableClass":"TblAnomalies",
    "tableRowClass":"TblAnomaliesRow",
    "links":[],
	"tableDisplayA":"AN_REF",
	"tableDisplayB":"AN_TITLE",
	"relations":[
		{"foreign_code":"rf", "type":"lnk"},
		{"foreign_code":"ft", "type":"lnk"},
		{"local_field":"AN_STATUS", "foreign_code":"lkp","foreign_field":"LKP_ID","foreign_group":"143", "type":"lkp"},
		{"local_field":"AN_ASSET_ID", "foreign_code":"node","foreign_field":"REC_TAG", "type":"lkp"}
	],
    "columns": [
        {"name":"AN_ID","type":"Int64","keyPosition":0},
    ],
    "captions":{
    },
    "tableCode":"an",
    "description":"Anomalies Table"
}
```
- tableName - name of the table as defined in the database engine
- tableFieldPrefix - first characters before the first underscore ```( _ )``` in the field name definition
- tableClassFilename
- tableClass - name of the table class which will be used when generating client-side ```TypeScript```
- tableRowClass - name of the row class which will be used when generating client-side ```TypeScript```
- links
- tableDisplayA - field ('A') which contains data that will be displayed when the table is used as a lookup
- tableDisplayB - field ('B') which contains data that will be displayed when the table is used as a lookup
- relations - collection of links of how the table is related to other tables<br/>
  Types:
  1. lnk - a type of relation where the current table is connected to another table through an intermediate link table. This is normally required if a one-to-many connection has to be made between two tables <br/>linked by ```parentKey->childKey```.
  2. lkp - lookup relation where the parent table stores the keyValue from the child table.
     - local_field - the field name in the parent table where the key value of the child table is stored
     - foreign_code - the tableCode property of the child table
     - foreign_field - the field in the child table where lookup will be done. if this is left blank or null, the defined keyFiled in the child table will be used instead.
- columns - array of field definitions where special properties (keyPostion, groupPosition, uniquePosition, sortPosition, etc.) of fields can be defined. Key field is a mandatory entry!.
- captions - dictionary of field descriptions which can be used in the client-side user interface. ```fieldNames``` are used as key in the collection dictionary.
- tableCode - code to be used to refer to the table object in the table collection dictionary
- description - describes the table



##### Examples:
###### For Anomalies table, the config file must contain the following `(<apiFolder>\schema\config\config.table.anom.json)`
```json

{
    "tableCode":"anom",
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
    "description":"Anomalies Table"
}
```
##### Setting Links for Anomaly Table
```json
{
    ...,
    "links":[
        {},
        {},
        {}
    ]
    ...
}
```


###### For Change Tracking table, the config file must contain the following `(<apiFolder>\schema\config.table.chgTrack.json)`
```json

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
