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

            //JObject appArgs = AppArgs;

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

            /************************************************************************************
             * 15 April 2020 - alv
             * 
             * public static string KEY_REQUEST_HEADER_CODE = "__header__";
             * 
             * public static string KEY_REQUEST_STAMP = "_req_stamp_";
             * public static string KEY_USER_ID = "__uid__";
             * public static string KEY_USER_RIGHTS = "__rights__";
             * public static string KEY_ACTION = "__action__";
             * 
             * JSON
             * 
               "__header__" :
               {
                    "_req_stamp_":"",
                    "__uid__":"alv",
                    "__rights__":"",
                    "__action__":""
               }
               "__header__" :{"_req_stamp_":"","__uid__":"alv","__rights__":"","__action__":""}

               https://www.base64encode.org/
               Equivalent base64: eyJfcmVxX3N0YW1wXyI6IiIsIl9fdWlkX18iOiJhbHYiLCJfX3JpZ2h0c19fIjoiIiwiX19hY3Rpb25fXyI6IiJ9
             *
             * Converted to Base64 text
             ************************************************************************************/

            DateTime startProcess = DateTime.Now;
            

            //dGhpcyBpcyBhIHRlc3Q=
            byte[] bytes = Convert.FromBase64String("dGhpcyBpcyBhIHRlc3Q=");
            string text = System.Text.Encoding.Default.GetString(bytes);
            byte[] bytes2 = System.Text.Encoding.Default.GetBytes(text);
            string textB64 = Convert.ToBase64String(bytes2);

            JObject args = AppArgs;

            if (values.ContainsKey(_g.KEY_REQUEST_HEADER_CODE))
            {
                string header = (String)values[_g.KEY_REQUEST_HEADER_CODE];         // extract header Base64
                byte[] jsonBytes = Convert.FromBase64String(header);                // convert to byte array
                string jsonText = System.Text.Encoding.Default.GetString(jsonBytes);// convert to JSON string
                JObject json = JObject.Parse(jsonText);                             // convert to JSON object

                // loop through header information and add token to AppArgs if not yet existing
                foreach (JProperty jph in (JToken)json)
                {
                    if (!args.ContainsKey(jph.Name))
                        args.Add(jph.Name, jph.Value);
                }
            }

            List <AppReturn> retVal = new List<AppReturn> { };
            List<CommandParam> cmds = new List<CommandParam>();

            foreach (JProperty jp in (JToken)values)
            {
                // iterate through all tables to generate CommandParams collection
                if (jp.Name != _g.KEY_REQUEST_HEADER_CODE)
                {
                    // get table object from the collection
                    DALTable tbl = AppDataset.AppTables[jp.Name];

                    // get collection of CommandParams per table
                    List<CommandParam> cmdsTemp = tbl.GetCommandParamsForPosting((JArray)jp.Value, args);

                    // append commands to the general collection for execution in bulk
                    foreach (CommandParam cmd in cmdsTemp) cmds.Add(cmd);
                }

                // execute commands
            }

            // execute all commands in the collection
            string errMessage = DALData.DAL.Excute(cmds, true);

            DateTime endProcess = DateTime.Now;

            AppReturn ret = new AppReturn();
            long dur = ((endProcess.Millisecond +
                endProcess.Second * 1000 +
                endProcess.Minute * 60 * 1000 +
                endProcess.Hour * 60 * 60 * 1000) - 
                (startProcess.Millisecond + 
                startProcess.Second * 1000 + 
                startProcess.Minute * 60 * 1000 +
                startProcess.Hour * 60 * 60 * 1000));

            ret.returnStrings.Add("Process Duration in milliseconds: " + dur.ToString() + "(ms)");
            if (errMessage.Length!=0) ret.returnStrings.Add("Error:" + errMessage);

            retVal.Add(ret);

            return retVal;
        }

        public List<AppReturn> nullget(string table, string key = "", string keyField = "")
        {
            List<AppReturn> retVal = new List<AppReturn> { };
            AppReturn ret = new AppReturn();
            retVal.Add(ret);
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