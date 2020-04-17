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
```
