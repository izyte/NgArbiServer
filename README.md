# Web API C# ( Server-Side Application)
### App_Start/WebApiConfig/Register:
```C#
config.Routes.MapHttRoute:
	routeTemplate: api/{controller}/{key}/{keyField}

```
 
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

http://soga-alv/NgArbi/api/app/plat/1
sql (sql)-
"select T.* from tbl_PlatformData as T  where [plnt_id] = @p0;"
params (prms)-
Count = 1
    [0]: {[@p0, 1]}

get table-
ReturnObject tbl = DAL.GetRecordset(new CommandParam(sql, prms), withFields: false);

withFields: false : returns a record in JSON Array
[
     1,
     1,
     "Dev Platform",
     "This  is just a test platform!!!",
     75,
     88750.0,
     88750.0,
     0.5,
     2,
     20.0,
     1.0,
     5.0,
     0.0005,
     0.5,
     0.01,
     100000.0,
     10.0,
     3000.0,
     30.0,
     null,
     null,
     "2017-01-29T09:40:40",
     null
]

withFields: true : returns a record in  JSON Object
{
     "plat_id": 1,
     "plat_ctry_id": 1,
     "plat_name": "Dev Platform",
     "plat_desc": "This  is just a test platform!!!",
     "plat_population": 75,
     "plat_area": 88750.0,
     "plat_area_pop": 88750.0,
     "plat_frac_pop": 0.5,
     "plat_pop_dens_lpd_id": 2,
     "plat_temp_amb": 20.0,
     "plat_press_atm": 1.0,
     "plat_cofst_ubsc": 5.0,
     "plat_cofst_lbsc": 0.0005,
     "plat_cofst_ubbc": 0.5,
     "plat_cofst_lbbc": 0.01,
     "plat_cofst_ubecg": 100000.0,
     "plat_cofst_lbecg": 10.0,
     "plat_cofst_ubecl": 3000.0,
     "plat_cofst_lbecl": 30.0,
     "plat_updated": null,
     "plat_updated_by": null,
     "plat_created": "2017-01-29T09:40:40"
}

post:
http://soga-alv/NgArbi/api/app/plnt
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
