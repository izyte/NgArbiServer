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

        public List<AppReturn> post([FromBody]JObject values)
        {
            /************************************************************************************
             * 20 Dec 2019 - alv
             * values:
             * {
             *  "<tableCode>":[{data row object 1},...,{data row object n}]
             * }
             ************************************************************************************/
            List<AppReturn> retVal = new List<AppReturn> { };
            foreach(JProperty jp in (JToken)values)
            {
                // iterate through all tables to get
                ReturnObject ret = AppDataset.AppTables[jp.Name].Post((JArray)jp.Value);
            }
            return retVal;
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
                appReturn.subsKey = "Hello Test Me!";

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