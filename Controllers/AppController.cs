using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using DataAccess;
using Newtonsoft.Json.Linq;
using _g = DataAccess.AppGlobals2;

namespace NgArbi.Controllers
{

    public class AppController : ApiController
    {

        /**************************************  Private ***************************************/
        private JObject _appArgs = null;
        private JObject _appData = null;
        /***************************************    Class Constructor   ******************************************/
        public AppController()
        {
            // reset log from AppDataset everytime an AppController call is made ...
            //AppDataset.GeneralRetObj.debugStrings.Clear();
            //AppDataset.GeneralRetObj.debugStrings.Add(_g.AppSetings["PATH_SETTINGS"]);
            //AppDataset.GeneralRetObj.debugStrings.Add("-----------------------------------------");

        }

        public JObject AppArgs
        {
            get
            {

                if (_appArgs == null)
                {
                    _appArgs = new JObject();
                }
                else
                {
                    return _appArgs;
                }

                // Collect query string arguments and some headers ...
                List<KeyValuePair<string, string>> retList = ControllerContext.Request.GetQueryNameValuePairs().ToList();

                // populate return JObject with elements from the querystring list
                foreach (KeyValuePair<string, string> arg in retList) { _appArgs.Add(arg.Key, arg.Value); }

                //add request data / time stamp
                _appArgs.Add(_g.KEY_REQUEST_STAMP, DateTime.Now);

                // add parameters embedded in form data
                if (_appData != null)
                {
                    // add user id in AppArgs parameter if avaialble and not yet in the collection
                    if (_appData.ContainsKey(_g.KEY_USER_ID) && !_appArgs.ContainsKey(_g.KEY_USER_ID))
                    {
                        _appArgs.Add(_g.KEY_USER_ID, _g.TKVStr(_appData, _g.KEY_USER_ID));
                    }
                }

                // add content type
                if (this.Request.Content.Headers.Contains("Content-Type"))
                    _appArgs.Add(_g.KEY_CONTENT_TYPE, this.Request.Content.Headers.ContentType.ToString().ToLower());

                return _appArgs;

            }
        }

        private bool isAppDebugPaths
        {
            get
            {
                return _g.TKVBln(AppArgs, "debug_path");
            }
        }

        private bool isAppDebug
        {
            get
            {
                return _g.TKVBln(AppArgs, "debug");
            }
        }

        private JObject props
        {
            get
            {
                JObject ret = new JObject();
                //ret.Add("table", "");
                //ret.Add("key", "");
                //JObject args = AppArgs;
                //JObject args2 = AppArgs;

                foreach (var tk in AppArgs)
                {
                    ret.Add(tk.Key, tk.Value);
                }
                return ret;
            }
        }

        private AppReturn DebugPath
        {
            get
            {
                AppReturn ret = new AppReturn();
                ret.props.Add("PATH_SCHEMA", _g.PATH_SCHEMA);
                ret.props.Add("PATH_SCHEMA_CLIENT", _g.PATH_SCHEMA_CLIENT);
                ret.props.Add("PATH_SCHEMA_TEMPLATES", _g.PATH_SCHEMA_TEMPLATES);
                ret.props.Add("PATH_SETTINGS", _g.PATH_SETTINGS);
                return ret;
            }
        }

        private AppReturn DebugData
        {
            get
            {
                AppReturn ret = new AppReturn();
                ret.records.Add(SQLTexts);
                ret.props = props;
                return ret;
            }


            /************************************************ Sample JObject operations **********************
             * 
            appReturn.props.Add("dateProp", DateTime.Now);
            appReturn.props.Add("numberProp", DateTime.Now.Ticks);
            appReturn.props.Add("stringProp", "The quick brown fox jumps over the lazy dog");
            appReturn.props.Add("boolProp", false);
            int ctr = 0;
            foreach(string s in AppDataset.GeneralRetObj.debugStrings)
            {
                ctr++;
                appReturn.props.Add("ds" + ctr, s);
            }
            //appReturn.records.Add(JObject.FromObject(new {
            //    title = "James Newton-King",
            //    link = "http://james.newtonking.com",
            //    description = "James Newton-King's blog.",
            //}));
            //appReturn.records.Add(JObject.FromObject(new
            //{
            //    title = "Identity Thief",
            //    link = "http://identity.movies.com",
            //    description = "Movie by Melissa McCarthy",
            //}));
            //appReturn.records.Add(new JObject(
            //    new JProperty("title","Sister Act")
            //    , new JProperty("link", "http://sisteract.movies.com")
            //    , new JProperty("duration", DateTime.Now.TimeOfDay)
            //    , new JProperty("price", 295.99)
            //    , new JProperty("description", "Movie by Whoopy Goldberg")
            //    )
            //);
            **************************************************************************************/

        }

        private JObject SQLTexts
        {
            get
            {
                DALTable tbl = AppDataset.AppTables[_g.TKVStr(AppArgs, "table")];

                return JObject.FromObject(new
                {
                    InsertRecord = tbl.SQLText("insert"),
                    UpdateRecordById = tbl.SQLText("update"),

                    SelectAllRecords = tbl.SQLText("select", noCond: true),
                    SelectAllRecordsNoSort = tbl.SQLText("select", noSort: true),
                    SelectRecordByKey = tbl.SQLText("select"),

                    SelectRecordsByGroup = tbl.SQLText("select", byGroup: true),
                    SelectRecordsByGroupNoSort = tbl.SQLText("select", byGroup: true, noSort: true),

                    DeleteUserParamsByParamID = tbl.SQLText("delete"),
                    DeleteUserParamsByGroupID = tbl.SQLText("delete", byGroup: true),

                    //UpdateTextUserParamByParamId = tbl.SQLText("update", 
                    //    includeColumns: new List<ColumnInfo> { tblPrm.columns[4] }),

                    //ResetAllEmail = tbl.SQLText("update",
                    //    includeColumns: new List<ColumnInfo> { tbl.columns[4] },
                    //    whereColumns: new List<ColumnInfo> { tbl.columns[2] }),

                    ColumnsFromIndices = tbl.ColumnsFromIndices("2`2"),

                });
            }
        }
        /**********************************************************************************************************
         *  Client-Side Called Methods
         *********************************************************************************************************/



        public AppReturn get()
        {
            if (isAppDebug) return DebugData;

            AppReturn appReturn = new AppReturn();


            // populate props with AppSettings
            foreach (KeyValuePair<string, dynamic> tk in _g.AppSetings)
            {
                appReturn.props.Add(tk.Key, tk.Value);
            }

            return appReturn;
        }

        public List<AppReturn> get(string table, string key = "", string keyField = "")
        {
            // delare final return object collection
            List<AppReturn> retVal = new List<AppReturn> { };

            AppArgs.Add("table", table);
            AppArgs.Add("key", key);
            AppArgs.Add("keyField", keyField);

            if (isAppDebug) return new List<AppReturn> { DebugPath };

            AppReturn appReturn = new AppReturn();

            if (table == "@gents")
            {
                // Generate client-side typescript files
                AppDataset.Initialize();

                appReturn.props.Add("Tables", AppDataset.AppTables.Count());


                appReturn.props.Add("Views", AppDataset.AppViews.Count());
                appReturn.props.Add("StoredProcedures", AppDataset.AppProcedures.Count());

                //retVal.Add(appReturn); return retVal;

                return new List<AppReturn> { appReturn };
            }



            if (isAppDebugPaths)
                return new List<AppReturn> { DebugPath };

            // Get collection of recordsets
            List<ReturnObject> ret = AppDataset.AppTables[table].Get(AppArgs);
            // iterate through the results of table Get method to build the final return collection
            foreach (ReturnObject retObj in ret)
            {
                AppReturn appRet = new AppReturn()
                {
                    // used as handle of list in the client-side data capture reoutine
                    returnCode = retObj.returnCode,
                    returnType = retObj.returnType,
                    // date and time stamp
                    requestDateTime = DateTime.Now,

                    recordCount = retObj.result.recordCount,
                    records = retObj.result.jsonReturnData,
                    recordsList = retObj.result.returnData,
                    recordsProps = retObj.result.recordsProps,
                    //columns = retObj.result.columns
                    //columnsArr = retObj.result.jsonReturnData
                    fieldNames = retObj.result.fieldsNames,
                    errorMessage = retObj.result.error,

                    subsKey = _g.TKVStr(AppArgs, _g.QS_SUBSCRIPTION_KEY),
                };

                retVal.Add(appRet);
            }

            return retVal;
        }

        public AppReturn post([FromBody]JObject values)
        {
            _appData = values;
            AppReturn ret = new AppReturn();
            ret.commands = AppDataset.BuildCommandParamsListFromData(values, AppArgs);
            return ret;
        }

        public AppReturn post([FromBody]JObject values, double temp)
        {
            AppReturn ret = new AppReturn();

            DALTableLink tableLink = new DALTableLink(
                _g.TKVStr(values, _g.KEY_TABLE_CODES),
                AppDataset.AppTables
            );

            ret.returnStrings.Add("Table Code: " + tableLink.code);
            ret.returnStrings.Add("Table Key: " + tableLink.key);
            ret.returnStrings.Add("Has Child : " + tableLink.hasChild);
            if (tableLink.hasChild)
            {
                ret.returnStrings.Add("=> Child table code : " + tableLink.childCode);
                ret.returnStrings.Add("=> Child parent key : " + tableLink.childParentKey);
            }
            ret.returnStrings.Add("===============================================");

            foreach (JProperty jp in (JToken)values)
            {
                ret.returnStrings.Add(jp.Name + ": " + jp.Value.Type.ToString());
                if (jp.Name == "data")
                {
                    string datType = jp.Value.Type.ToString();
                    if (datType == "Array")
                    {
                        ret.inputArr = (JArray)jp.Value;
                    }
                    else
                    {
                        ret.input = (JObject)jp.Value;
                    }
                }
                else if (jp.Name == _g.KEY_TABLE_CODES)
                {

                }
            }

            if (ret.inputArr.Count != 0)
            {
                // iterate through the elements of the array
                ret.returnStrings.Add("================================ Data processing ... ================================");
                foreach (JObject jo in ret.inputArr)
                {
                    Int64 parentKey = _g.TKV64(jo, tableLink.key);
                    ret.returnStrings.Add(tableLink.key + " : " + parentKey);
                    ret.returnStrings.Add("  parent tokens : " + jo.PropertyValues().Count());

                    string[] action = tableLink.table.GetActionFromData(jo).Split('|');

                    ret.returnStrings.Add("  parent action : " + action[0]);

                    if (action[0] == "insert")
                    {
                        jo.Add("oldKey", parentKey);
                        // set new parent key
                        parentKey = Convert.ToInt64(action[1]);
                        jo[tableLink.key] = parentKey;
                    }




                    if (tableLink.hasChild)
                    {
                        if (jo.ContainsKey(tableLink.childCode))
                        {
                            // if parent contains a collection of child records
                            JArray childArr = _g.TKVJArr(jo, tableLink.childCode);

                            // iterate through the child elements
                            foreach (JObject cJo in childArr)
                            {

                                string[] childAction = tableLink.childTable.GetActionFromData(cJo).Split('|');

                                Int64 childKey = _g.TKV64(cJo, tableLink.childKey);
                                ret.returnStrings.Add("  -> " + tableLink.childKey + " : " + childKey);

                                ret.returnStrings.Add("        token count : " + cJo.PropertyValues().Count());
                                ret.returnStrings.Add("        child action : " + childAction[0]);

                                if (childAction[0] == "insert")
                                {
                                    if (cJo.ContainsKey(tableLink.childParentKey))
                                    {
                                        // update parent key value
                                        cJo[tableLink.childParentKey] = parentKey;
                                    }
                                    else
                                    {
                                        // create new field token
                                        cJo.Add(tableLink.childParentKey, parentKey);
                                    }

                                    cJo.Add("oldKey", childKey);
                                    // set new child key
                                    childKey = Convert.ToInt64(childAction[1]);
                                    cJo[tableLink.childKey] = childKey;
                                }


                            }
                        }
                    }

                }
            }

            return ret;
        }
        public AppReturn post([FromBody]JObject values, string second)
        {
            // assign values to global private variable to make it available 
            // for consumption in other method calls (e.g. AppArgs parameter)
            _appData = values;
            if (_appData.ContainsKey(_g.KEY_INDICES)) AppArgs.Add(_g.KEY_INDICES, _g.TKVStr(_appData, _g.KEY_INDICES));
            if (!AppArgs.ContainsKey(_g.KEY_ACTION)) AppArgs.Add(_g.KEY_ACTION, _g.TKVStr(values, _g.KEY_ACTION));
            AppReturn ret = new AppReturn();


            /*
                Sample update input where key indices is specified:
                resolved column: uprm_user_id
                note: in this particular example, uprm_id is not included in the set of
                      input parameters because it's the unique key field of the table.
                      This type of input, is used to update multiple records to have
                      the same set of values for the given list of fields
                POST: http://localhost:6072/api/app
                {
	                "table":"uprm",
	                "__uid__":"alv",
	                "__action__":"update",
	                "__key_indices__":"1",
	                "data":{
		                "_newId":-143,
		                "uprm_user_id":888,
		                "uprm_type_lkp_id":8003,
		                "uprm_value_lkp_id":8503,
		                "uprm_value_text":null
	                }
                }
                
                Sample update input where default key column(s) is used in where clause:
                default key: uprm_id
                POST: http://localhost:6072/api/app
                {
	                "table":"uprm",
	                "__uid__":"alv",
	                "__action__":"update",
	                "data":{
		                "_newId":-143,
		                "uprm_id":8900,
		                "uprm_type_lkp_id":8003,
		                "uprm_value_lkp_id":8503,
		                "uprm_value_text":"updating parameter record id 8900"
	                }
                }             

             */


            ret.returnStrings.Add("Update SQL:");
            ret.input = values;

            // if table code is not specified in the url, it should be specified in the 
            // input object as "table" root property 
            string tableCode = _g.TKVStr(values, "table");
            DALTable tbl = AppDataset.AppTables[tableCode];

            ret.stamps = new DALStamps(tbl.columns, _g.TKVStr(AppArgs, _g.KEY_USER_ID));

            string action = _g.TKVStr(AppArgs, _g.KEY_ACTION);
            if (action == "") action = "update";


            ret.returnStrings.Add(tbl.SQLText(action));
            //ret.columns = tbl.columns;

            ColumnInfo testColSearch = tbl.columns.Find(c => c.name == "user_id");
            ret.returnStrings.Add(testColSearch == null ? "Column Not Found" : "Column found!");
            ret.returnStrings.Add("Table Instantiated: " + tbl.instantiated.ToString());
            //ret.returnStrings.Add("NewID: " + tbl.NewAutoId.ToString());

            JObject data = _g.TKVJObj(values, "data");

            //ret.inputParams = tbl.BuildCommandParams(_g.TKVJObj(values, "data"),tbl.columns);

            //ret.input = data;
            //ret.input.Add(tbl.tableFieldPrefix + "updated_by", _g.TKVStr(values, "__uid__"));

            //ret.inputParams = tbl.BuildUpdateParams(data);
            DALTableFieldParams tblParams =
                new DALTableFieldParams(
                    data,
                    ret.stamps, tbl,
                    action == "update",
                    AppArgs.ContainsKey(_g.KEY_INDICES) ?
                        tbl.ColumnsFromIndices(_g.TKVStr(AppArgs, _g.KEY_INDICES)) : null
                );

            //ret.inputParams = tblParams.parameters;

            ret.returnStrings.Add("SQL: " + tblParams.SQLText);
            ret.returnStrings.Add("ColumnIndices: " + tbl.columnsIndex.ContainsKey("stray").ToString());
            ret.returnStrings.Add("New Temporary Id: " + tblParams.tempNewId);

            int pIdx = 0;
            Dictionary<string, dynamic> newParams = new Dictionary<string, dynamic>();
            foreach (KeyValuePair<string, dynamic> vv in tblParams.parameters)
            {
                newParams.Add("@p" + pIdx, vv.Value);
                pIdx++;
            }

            ReturnObject retObj = DAL.Excute(new CommandParam(tblParams.SQLText, tblParams.parameters));

            ret.inputParams = newParams;

            ret.returnStrings.Add("Error: " + retObj.result.exceptionMessage);
            ret.returnStrings.Add("Recs: " + retObj.result.affectedRecords);

            return ret;

        }
        public AppReturn post(string table, string key, [FromBody]JObject values, string keyField = "")
        {
            AppReturn ret = new AppReturn();

            return ret;
        }
        public AppReturn post(string table, [FromBody]JObject values)
        {
            AppReturn ret = new AppReturn();

            return ret;
        }



        //public ReturnObjectExternal xxxxpostX(string table, [FromBody]JObject values)
        //{
        //    ReturnObject retVal = AppDataset.Post(table, values, AppArgs);
        //    return retVal.result;
        //}


        /**********************************************************************************************************
         *  Private Methods
         *********************************************************************************************************/

        //private JObject AppArgs()
        //{
        //    // Collect query string arguments and some headers ...

        //    JObject ret = new JObject();

        //    List<KeyValuePair<string, string>> retList = ControllerContext.Request.GetQueryNameValuePairs().ToList();

        //    // add request data/time stamp
        //    retList.Add(new KeyValuePair<string, string>(AppGlobals.KEY_REQUEST_STAMP, DateTime.Now.ToString()));

        //    //add content type
        //    if (this.Request.Content.Headers.Contains("Content-Type"))
        //    {
        //        ret.Add(new KeyValuePair<string, string>(AppGlobals.KEY_CONTENT_TYPE, this.Request.Content.Headers.ContentType.ToString().ToLower()));
        //    }

        //    foreach (KeyValuePair<string, string> arg in retList)
        //    {
        //        ret.Add(arg.Key, arg.Value);
        //    }
        //    return ret;
        //}
    }
}