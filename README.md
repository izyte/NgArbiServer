# IMSA Web API C# ( Server-Side Application)
### App_Start/WebApiConfig/Register:
```C#
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{table}/{key}/{keyField}/{includedFields}",
                // to skip optional topics/parameters, supply a hyphen (-) character 
                // eg. skip keyField => ../api/app/<table code>/1/-/1,2,3
                // if supplied value for includedFields is 'df' - display fields, 
                //   columnInfo of the defined display fields will be resolved in the server
                // if supplied value for includedFields is comma delimited integers, 
                //   columnInfo of supplied indices will be resolved in the server
                defaults: new
                {
                    table = RouteParameter.Optional,
                    key = RouteParameter.Optional,
                    keyField = RouteParameter.Optional,
                    includedFields = RouteParameter.Optional
                }
            );
```

### API Call
#### GET
##### Initialize Table Definitions
```text
<protocol>://<domain>/[application]/api/app/@gents
```
##### Retrieve Records
```text
<protocol>://<domain>/[application]/api/app/<table code>/[key value]/[key field index]
```
- ```<table code>``` - table code set in the table configuration file.

- ```[key value]``` - long integer representing the value to be searched from the table's unique key field or from the field specified in ```[key field index]``` argument. If this argument is not specified, the request will return all the records found in table.

- ```[key field index]``` - zero-based index which represents the ordinal position a table column where the specified key values will be searched from. Default value of this parameter is the table's key field ordinal position.


#### POST
```text
<protocol>://<domain>/[application]/api/app
```
##### Required Header
```text
Content-Type: applications/json
```
##### Required body
```json
	// simple record update
	{
		"__header__":"<header code>",
		"<table code 1>":[
			{
				"_requestDate": "yyyy-mm-ddTHH:mm:ss.sssZ",
				"<keyField>":"<key field>",
				"field1":"updated value 1",
				"field2":"updated value 2",
				"field#":"updated value #",
			},
			{ [next_record] },
		],
		"<table code #>":[
			{ ... },
			{ ... }
		]
	}
	
	// record with linked table update
	
	{
		"__header__":"<header code>",
		"<table code 1>":[
			{
				"_requestDate": "yyyy-mm-ddTHH:mm:ss.sssZ",
				"<keyField>":"<key field>",
				"field1":"updated value 1",
				"field2":"updated value 2",
				"field#":"updated value #",
				"__links__":[
					{
						"table_code":"<child table code 1>",
						"action":"<add | remove>",
						"child_ids":"<null | id1,id2,id3,...,id#>"
					},
					{
						"table_code":"<child table code 2>",
						"action":"<add | remove>",
						"child_ids":"<null | id1,id2,id3,...,id#>"
					},
					{
						"table_code":"<child table code #>",
						"action":"<add | remove>",
						"child_ids":"<null | id1,id2,id3,...,id#>"
					}
				]
			},
			{ [next_record] },
		]
	}	
```

##### Response Object
```json
{}
```


### Sample Postman Requests
#### Refresh table configuration

<img src="https://github.com/izyte/NgArbiServer/blob/master/docs/postman_gents.png" title="Initialize table configurations" alt="Initialize able config">

#### Get all records from users table

<img src="https://github.com/izyte/NgArbiServer/blob/master/docs/postman_all_users.png" title="Get all records from the users table" alt="Get all records from the users table">

#### Selective updates on users table records

<img src="https://github.com/izyte/NgArbiServer/blob/master/docs/postman_selective_users_updates.png" title="Update selected fields for selected records on users table" alt="Update selected fields for selected records on users table">

#### Updates on reference file linked table to anomaly

<img src="https://github.com/izyte/NgArbiServer/blob/master/docs/postman_linked_table_update.png" title="Update linked reference table entry for anomaly record" alt="Update linked reference table entry for anomaly record">


### API Call
```text
api call:
	<protocol>://<domain>:[port]/<application>/api/<controller>/<table code>/[key value]/[key field index]
	
sample api call: 
	
	http://soga-alv/NgArbi/api/app/upln/1/2
	
	Where:
		protocol 	: http
		domain		: soga-alv
		port		: (not specified)
		application	: NgArbi
		controller	: app
		table code 	: upln
		key value	: 1	(optional)
		key field index	: 2	(optional)

	Notes:
		- If key value and key field is not specified, all records will be returned
		- If key field index is not specified, key field index used will be the index
		  of the field defined as keyField in the table configuration file in
		  <app folder>/App_Data/schema/config/config.table.<table code>.json file.
		  columns/keyPosition:0
		
		  Example:

		     "columns": [
		        {"name":"upln_id","type":"Int64","keyPosition":0},
		        {"name":"upln_user_id","type":"Int64","groupPosition":0}
		    ],
		    
		    Where:
			column "upln_id" is the key field because it has the key-value pair
			"keyPosition":0
			
get:
http://soga-alv/NgArbi/api/app/@gents
   if table code is set to @gents, api is instructed to generate TypeScript files 
   which serve as equivalent Table Class library that will be used by the client-side
   angular application

http://soga-alv/NgArbi/api/app/upln/1/2

Get a single record from Anomalies table where record id is 1039
API Call -
http://soga-alv/NgArbi/api/app/anom/1039
sql (sql)-
"select T.* from tbl_PlatformData as T  where [plnt_id] = @p0;"
params (prms)-
Count = 1
    [0]: {[@p0, 1039]}

get table-
ReturnObject tbl = DAL.GetRecordset(new CommandParam(sql, prms), withFields: false);

withFields: false : returns a record in JSON Array
[
    {
        "returnType": "table",
        "subsKey": "",
        "returnCode": "anom",
        "requestDateTime": "2020-04-17T13:34:29.4611716+08:00",
        "recordCount": 1,
        "records": null,
        "recordsList": [
            [
                1039.0,
                "16-0013",
                8956.0,
                null,
                8452.0,
                "SOGA Tech",
                "2016-03-28T13:31:14",
                8471.0,
                8470.0,
                "1",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                "2016-03-28T13:31:14",
                null,
                null,
                null,
                null,
                null,
                8471.0,
                8470.0,
                null,
                null,
                "2016-04-19T14:28:33",
                "SOGA Tech",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0.0,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            ]
        ],
        "recordsProps": null,
        "returnStrings": [],
        "props": {},
        "input": {},
        "inputArr": [],
        "columnsArr": null,
        "fieldNames": [
            "AN_ID",
            "AN_REF",
            "AN_ASSET_ID",
            "AN_TYPE",
            "AN_STATUS",
            "AN_RAISED_BY",
            "AN_RAISED_DATE",
            "AN_ORIG_CLASS",
            "AN_CURR_CLASS",
            "AN_REVNO",
            "AN_ASS_DATE",
            "AN_ASS_BY",
            "AN_ACT_BY_DATE",
            "AN_ACT_PARTY",
            "AN_MAINT_REQ",
            "AN_WO_REF",
            "AN_WO_STATUS",
            "AN_DATE_IDENT",
            "AN_ACT_REQ",
            "AN_EQ_FAILURE",
            "AN_TA_APPROVED",
            "AN_TA_NAME",
            "AN_TA_APPR_DATE",
            "AN_ORIG_AVAIL_CLASS",
            "AN_CURR_AVAIL_CLASS",
            "AN_AVAIL_UPD_DATE",
            "AN_AVAIL_UPD_BY",
            "AN_UPD_DATE",
            "AN_UPD_BY",
            "AN_FNCR_REQUIRED",
            "AN_FNCR",
            "AN_LIFE_TERM",
            "AN_PORTFOLIO_APPL",
            "AN_RISK_RANK_SEVERITY",
            "AN_RISK_RANK_LIKELIHOOD",
            "AN_START_EAST",
            "AN_END_EAST",
            "AN_DESC",
            "AN_START_NORTH",
            "AN_END_NORTH",
            "AN_NOTIFICATION_REF",
            "AN_RECCMD",
            "AN_NOTIFICATION_STATUS",
            "AN_ASSMNT",
            "AN_DELETED",
            "AN_AVAIL_COMMENTS",
            "AN_TITLE",
            "AN_ATTACHMENTS",
            "AN_RISK_RANK_COMMENTS",
            "ITV_ANOM_REF",
            "ITV_DATE_MOBIL",
            "ITV_DATE_PROD_RSTO",
            "ITV_TYPE",
            "ITV_TIME_DIAGNOSTIC",
            "ITV_VESSEL_TYPE",
            "ITV_CATEGORY",
            "ITV_TIME_PLAN_PROC",
            "ITV_TIME_MOBIL",
            "ITV_TIME_ACTUAL",
            "ITV_TIME_DEMOB",
            "ITV_TTIME_WEATHER",
            "ITV_TTIME_ROV",
            "ITV_TTIME_VESSEL",
            "ITV_TTIME_VEND_EQPT",
            "ITV_TTIME_OTHER",
            "ITV_AFE_COST",
            "ITV_ACTUAL_COST",
            "ITV_DATE_ACTUAL_COST",
            "ITV_WBS_NUMBER",
            "ITV_UPDATED",
            "ITV_UPDATED_BY",
            "ITV_SUMMARY",
            "ITV_VESSEL_NAME",
            "ITV_PARENT_ANOM_REV",
            "ITV_LEARNING",
            "ITV_AFE_SHELL_SHARE",
            "ITV_VESSEL_DAY_RATE",
            "ITV_WELL_DOWNTIME"
        ],
        "stamps": null,
        "inputParams": {},
        "result": "success",
        "errorMessage": "",
        "columns": null,
        "commands": null
    }
]

withFields: true : returns a record in  JSON Object
[
    {
        "returnType": "table",
        "subsKey": "",
        "returnCode": "anom",
        "requestDateTime": "2020-04-17T13:37:08.6283029+08:00",
        "recordCount": 1,
        "records": [
            {
                "AN_ID": 1039.0,
                "AN_REF": "16-0013",
                "AN_ASSET_ID": 8956.0,
                "AN_TYPE": null,
                "AN_STATUS": 8452.0,
                "AN_RAISED_BY": "SOGA Tech",
                "AN_RAISED_DATE": "2016-03-28T13:31:14",
                "AN_ORIG_CLASS": 8471.0,
                "AN_CURR_CLASS": 8470.0,
                "AN_REVNO": "1",
                "AN_ASS_DATE": null,
                "AN_ASS_BY": null,
                "AN_ACT_BY_DATE": null,
                "AN_ACT_PARTY": null,
                "AN_MAINT_REQ": null,
                "AN_WO_REF": null,
                "AN_WO_STATUS": null,
                "AN_DATE_IDENT": "2016-03-28T13:31:14",
                "AN_ACT_REQ": null,
                "AN_EQ_FAILURE": null,
                "AN_TA_APPROVED": null,
                "AN_TA_NAME": null,
                "AN_TA_APPR_DATE": null,
                "AN_ORIG_AVAIL_CLASS": 8471.0,
                "AN_CURR_AVAIL_CLASS": 8470.0,
                "AN_AVAIL_UPD_DATE": null,
                "AN_AVAIL_UPD_BY": null,
                "AN_UPD_DATE": "2016-04-19T14:28:33",
                "AN_UPD_BY": "SOGA Tech",
                "AN_FNCR_REQUIRED": null,
                "AN_FNCR": null,
                "AN_LIFE_TERM": null,
                "AN_PORTFOLIO_APPL": null,
                "AN_RISK_RANK_SEVERITY": null,
                "AN_RISK_RANK_LIKELIHOOD": null,
                "AN_START_EAST": null,
                "AN_END_EAST": null,
                "AN_DESC": null,
                "AN_START_NORTH": null,
                "AN_END_NORTH": null,
                "AN_NOTIFICATION_REF": null,
                "AN_RECCMD": null,
                "AN_NOTIFICATION_STATUS": null,
                "AN_ASSMNT": null,
                "AN_DELETED": null,
                "AN_AVAIL_COMMENTS": null,
                "AN_TITLE": null,
                "AN_ATTACHMENTS": 0.0,
                "AN_RISK_RANK_COMMENTS": null,
                "ITV_ANOM_REF": null,
                "ITV_DATE_MOBIL": null,
                "ITV_DATE_PROD_RSTO": null,
                "ITV_TYPE": null,
                "ITV_TIME_DIAGNOSTIC": null,
                "ITV_VESSEL_TYPE": null,
                "ITV_CATEGORY": null,
                "ITV_TIME_PLAN_PROC": null,
                "ITV_TIME_MOBIL": null,
                "ITV_TIME_ACTUAL": null,
                "ITV_TIME_DEMOB": null,
                "ITV_TTIME_WEATHER": null,
                "ITV_TTIME_ROV": null,
                "ITV_TTIME_VESSEL": null,
                "ITV_TTIME_VEND_EQPT": null,
                "ITV_TTIME_OTHER": null,
                "ITV_AFE_COST": null,
                "ITV_ACTUAL_COST": null,
                "ITV_DATE_ACTUAL_COST": null,
                "ITV_WBS_NUMBER": null,
                "ITV_UPDATED": null,
                "ITV_UPDATED_BY": null,
                "ITV_SUMMARY": null,
                "ITV_VESSEL_NAME": null,
                "ITV_PARENT_ANOM_REV": null,
                "ITV_LEARNING": null,
                "ITV_AFE_SHELL_SHARE": null,
                "ITV_VESSEL_DAY_RATE": null,
                "ITV_WELL_DOWNTIME": null
            }
        ],
        "recordsList": null,
        "recordsProps": null,
        "returnStrings": [],
        "props": {},
        "input": {},
        "inputArr": [],
        "columnsArr": null,
        "fieldNames": [
            "AN_ID",
            "AN_REF",
            "AN_ASSET_ID",
            "AN_TYPE",
            "AN_STATUS",
            "AN_RAISED_BY",
            "AN_RAISED_DATE",
            "AN_ORIG_CLASS",
            "AN_CURR_CLASS",
            "AN_REVNO",
            "AN_ASS_DATE",
            "AN_ASS_BY",
            "AN_ACT_BY_DATE",
            "AN_ACT_PARTY",
            "AN_MAINT_REQ",
            "AN_WO_REF",
            "AN_WO_STATUS",
            "AN_DATE_IDENT",
            "AN_ACT_REQ",
            "AN_EQ_FAILURE",
            "AN_TA_APPROVED",
            "AN_TA_NAME",
            "AN_TA_APPR_DATE",
            "AN_ORIG_AVAIL_CLASS",
            "AN_CURR_AVAIL_CLASS",
            "AN_AVAIL_UPD_DATE",
            "AN_AVAIL_UPD_BY",
            "AN_UPD_DATE",
            "AN_UPD_BY",
            "AN_FNCR_REQUIRED",
            "AN_FNCR",
            "AN_LIFE_TERM",
            "AN_PORTFOLIO_APPL",
            "AN_RISK_RANK_SEVERITY",
            "AN_RISK_RANK_LIKELIHOOD",
            "AN_START_EAST",
            "AN_END_EAST",
            "AN_DESC",
            "AN_START_NORTH",
            "AN_END_NORTH",
            "AN_NOTIFICATION_REF",
            "AN_RECCMD",
            "AN_NOTIFICATION_STATUS",
            "AN_ASSMNT",
            "AN_DELETED",
            "AN_AVAIL_COMMENTS",
            "AN_TITLE",
            "AN_ATTACHMENTS",
            "AN_RISK_RANK_COMMENTS",
            "ITV_ANOM_REF",
            "ITV_DATE_MOBIL",
            "ITV_DATE_PROD_RSTO",
            "ITV_TYPE",
            "ITV_TIME_DIAGNOSTIC",
            "ITV_VESSEL_TYPE",
            "ITV_CATEGORY",
            "ITV_TIME_PLAN_PROC",
            "ITV_TIME_MOBIL",
            "ITV_TIME_ACTUAL",
            "ITV_TIME_DEMOB",
            "ITV_TTIME_WEATHER",
            "ITV_TTIME_ROV",
            "ITV_TTIME_VESSEL",
            "ITV_TTIME_VEND_EQPT",
            "ITV_TTIME_OTHER",
            "ITV_AFE_COST",
            "ITV_ACTUAL_COST",
            "ITV_DATE_ACTUAL_COST",
            "ITV_WBS_NUMBER",
            "ITV_UPDATED",
            "ITV_UPDATED_BY",
            "ITV_SUMMARY",
            "ITV_VESSEL_NAME",
            "ITV_PARENT_ANOM_REV",
            "ITV_LEARNING",
            "ITV_AFE_SHELL_SHARE",
            "ITV_VESSEL_DAY_RATE",
            "ITV_WELL_DOWNTIME"
        ],
        "stamps": null,
        "inputParams": {},
        "result": "success",
        "errorMessage": "",
        "columns": null,
        "commands": null
    }
]


Get all records from Anomalies table where anomaly status(col index=4) is open(key value=8450)
API Call-
http://soga-alv/NgArbi/api/app/anom/8450/4

[
    {
        "returnType": "table",
        "subsKey": "",
        "returnCode": "anom",
        "requestDateTime": "2020-04-17T13:53:13.7180336+08:00",
        "recordCount": **50**,
        "records": null,
        "recordsList": [
            [
                995.0,
                "15-0023",
                9799.0,
                22.0,
                8450.0,
                "SOGA Tech",
                "2015-08-06T08:33:23",
                8471.0,
                8471.0,
                "5",
                null,
                null,
                "2016-02-01T10:20:13",
                null,
                null,
                null,
                null,
                "2015-08-06T08:33:23",
                null,
                0.0,
                0.0,
		...
	   ]
	}		
]

post:
http://soga-alv/NgArbi/api/app
data-
{
	"_requestDate":"2020-04-13T16:42:42.196Z",
	"plat_id":1,
	"plat_name":"Dev Platform-up",
	"plat_desc":"This is just a test platform!!!2",
	"plat_population":"85"
}
sql-
"update tbl_PlatformData set [plat_name] = @p0, [plat_desc] = @p1, [plat_population] = @p2 where [plat_id] = @p3;"
params-
Count = 4
    [0]: {[@p0, Dev Platform-up]}
    [1]: {[@p1, This is just a test platform!!!2]}
    [2]: {[@p2, 85]}
    [3]: {[@p3, 1]}

			

```

### Setup
#### Table configuration
For table configuration, open [ConfigTable.md](https://github.com/izyte/NgArbiServer/blob/master/ConfigTable.md)

#### Data Migration
For data migration instructions, open [DataMigration.md](https://github.com/izyte/NgArbiServer/blob/master/DataMigration.md)

#### Change Tracking Sample Records

<img src="https://github.com/izyte/NgArbiServer/blob/master/docs/UpdateTrackingSample.png" title="Sample Change Tracking Data" alt="Tracking Data">

### Recent Updates
```text
- ...
- test table configuration on lookups, anomalies, tree, nodes, users and user params
- included stamp fields to test tables

```


### Update in progress ...
