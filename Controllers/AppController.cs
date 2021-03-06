﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using DataAccess;
using Newtonsoft.Json.Linq;
using _g = DataAccess.AppGlobals2;
using System.Diagnostics;

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
                    // Clear global messages
                    DALData.DAL.LogGlobalMessage("Clear global messages from AppArgs...", "initClear", true);
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



        public List<AppReturn> get()
        {

            bool isWithQParam = AppArgs.ContainsKey(_g.KEY_QPARAM_JSON);
            if (isWithQParam) return ProcessQParam();

            if (isAppDebug) return new List<AppReturn>() { DebugData };

            AppReturn appReturn = new AppReturn();

            // populate props with AppSettings
            foreach (KeyValuePair<string, dynamic> tk in _g.AppSetings)
            {
                appReturn.props.Add(tk.Key, tk.Value);
            }

            return new List<AppReturn>() { appReturn };
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

            // generate collection of commands to execute
            foreach (JProperty jp in (JToken)values)
            {
                // iterate through all tables to generate CommandParams collection
                if (jp.Name != _g.KEY_REQUEST_HEADER_CODE)
                {
                    // get table object from the collection
                    DALTable tbl = AppDataset.AppTables[jp.Name];

                    // get collection of CommandParams per table
                    List<CommandParam> cmdsTemp = tbl.GetCommandParamsForPosting((JArray)jp.Value, args);
                    if(DALData.DAL.globalError.Length != 0)
                    {
                        // error has occured, therefore empty list is returned. handle error here
                        break;
                    }

                    // append commands to the general collection for execution in bulk
                    foreach (CommandParam cmd in cmdsTemp) cmds.Add(cmd);
                }

                // execute commands
            }

            // execute all commands in the collection
            List<ReturnObject> cmdResults = DALData.DAL.Excute(cmds, true);

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

            //ret.returnStrings.Add("Process Duration in milliseconds: " + dur.ToString() + "(ms)");
            //if (errMessage.Length!=0) ret.returnStrings.Add("Error:" + errMessage);

            //ret.stamps = new DALStamps(cmds[0].table.columns, "@alv");
            ret.stamps = null;

            //AppReturn ar = new AppReturn();
            //ar.stamps = new DALStamps(cmds[0].table.columns, "@alv");
            //retVal.Add(ar);

            ret.errorMessage = DALData.DAL.globalError;
            
            retVal.Add(ret);

            foreach (ReturnObject rObj in cmdResults)
            {
                ret = new AppReturn();
                ret.errorMessage = rObj.result.exceptionMessage;
                retVal.Add(ret);
            }

            return retVal;
            //return new List<AppReturn> { new AppReturn() };
        }

        private List<AppReturn> nullget(string table, string key = "", string keyField = "")
        {
            List<AppReturn> retVal = new List<AppReturn> { };
            AppReturn ret = new AppReturn();
            retVal.Add(ret);
            return retVal;
        }


        private List<AppReturn> ProcessQParam()
        {
            // add JArray parameters to AppArgs with key _g.KEY_REQ_ARGS_ARR then process
            AppArgs.Add(_g.KEY_REQ_ARGS_ARR, DALGlobals.btoJA(_g.TKVStr(AppArgs, _g.KEY_QPARAM_JSON)));
            return ExecuteGetRequest();
        }

        private List<AppReturn> ExecuteGetRequest()
        {
            List<AppReturn> retVal = new List<AppReturn> { };
            JObject reqConfig = null;

            foreach(JObject jParam in _g.TKVJArr(AppArgs, _g.KEY_REQ_ARGS_ARR))
            {
                string[] tableCodeArr = _g.TKVStr(jParam, "code").Split('|');
                string tableCode = tableCodeArr[0];

                if (tableCodeArr.Length > 1)
                {
                    // table code was supplied with fromClause expression code to be parsed during
                    // creation of SQL statement.
                    jParam["code"] = tableCode;
                    jParam.Add("fromClauseExpr", tableCodeArr[1]);
                }

                if (tableCode == "") continue;  // table code not found
                //http://soga-alv/NgArbi/api/app?_p=W3siY29kZSI6IkBtaXNjIn0seyJjb2RlIjoidXNlciIsInBhZ2VOdW1iZXIiOjEsInBhZ2VTaXplIjozNX1d
                if (tableCode == "@config")
                {
                    // miscellaneous parameters passed from the client
                    reqConfig = new JObject();

                    AppReturn misc = new AppReturn();
                    misc.returnType = "config";
                    misc.subsKey = _g.TKVStr( jParam, "subsKey");

                    retVal.Insert(0, misc);

                    continue;
                }

                //if (tableCodeArr.Length > 1)
                //{
                //    // fromClauseExpr is specified, add entry to AppArgs object
                //    AppArgs.Add("fromClauseExpr", tableCodeArr[1]);
                //}


                List<ReturnObject> ret = AppDataset.AppTables[tableCode].Get(AppArgs, jParam);

                // iterate through the results of table Get method to build the final return collection
                int retCount = 0;
                foreach (ReturnObject retObj in ret)
                {
                    AppReturn appRet = new AppReturn()
                    {
                        // used as handle of list in the client-side data capture reoutine
                        returnCode = retObj.returnCode,
                        returnType = retObj.returnType,

                        // date and time stamp ( by default this parameter is set in the constructor
                        // requestDateTime = DateTime.Now,

                        recordCount = retObj.result.recordCount,
                        records = retObj.result.jsonReturnData,
                        returnDataParams = retObj.result.returnDataParams,
                        recordsList = retObj.result.returnData,
                        recordsProps = retObj.result.recordsProps,
                        //columns = retObj.result.columns
                        //columnsArr = retObj.result.jsonReturnData
                        fieldNames = retObj.result.fieldsNames,
                        errorMessage = retObj.result.error,

                        requestDateTime = retObj.result.requestDateTime,
                        requestDuration = retObj.result.requestDuration,

                        subsKey = _g.TKVStr(AppArgs, _g.QS_SUBSCRIPTION_KEY),
                    };

                    retCount++;
                    if (retCount == 1) appRet.globalMesages = DALData.DAL.globalMessages;
                    retVal.Add(appRet);
                }
            }


            return retVal;
        }

        public List<AppReturn> get(string table, string key = "",
            string keyField = "", string includedFields = "",
            string filterExpression = "", string sortFields = "", 
            string pageNumber="0",string pageSize="0",
            string requestConfig = "")
            //string keyField = "")
        {
            // process request where all parameters are embedded in the Base64 querystring parameter "_p"

            // Add all parameters to the AppArgs object

            List<AppReturn> retVal = new List<AppReturn> { };

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

                foreach(string tblCode in AppDataset.AppTables.Keys)
                {
                    DALTable tbl = AppDataset.AppTables[tblCode];
                    appReturn.processLogs.Add(tblCode, tbl.tableName);
                    appReturn.processLogs.Add(tblCode + "_relation", tbl.tableRelations == null ? "0" : tbl.tableRelations.Count.ToString());
                    foreach(string pv in tbl.tableProcessLogs.Keys)
                    {
                        appReturn.processLogs.Add(tblCode + "_proc_" + pv, tbl.tableProcessLogs[pv]);
                    }
                }

                //retVal.Add(appReturn); return retVal;
                return new List<AppReturn> { appReturn };
            }

            if (table == "@locs")
            {
                Stopwatch st = new Stopwatch();
                st.Start();
                string[] keyArr = key.Split('`');
                string pCode = "";

                if (keyArr.Length >= 2) pCode = keyArr[1];

                DALData.DAL.BuildNodeLocation(Convert.ToInt32(keyArr[0]),pCode);
                appReturn.returnCode = table;
                st.Stop();
                appReturn.requestDuration = st.ElapsedMilliseconds;

                return new List<AppReturn> { appReturn };
            }
            if (table == "@mtbl")
            {
                // multi table query, accepts json formatted parameters supplied as 
                // Base64 encoded querystring parameters named ?p=
                // api call <protocol>://<domain>[/application]/api/app/@mtbl?p=<base64 encoded parameters>
                return new List<AppReturn> { appReturn };
            }

            if (isAppDebugPaths) return new List<AppReturn> { DebugPath };


            JObject jArgs = new JObject() { };
            string[] tableArr = table.Split('|');

            jArgs.Add("code", tableArr[0]);
            if(tableArr.Length>1) jArgs.Add("fromClauseExpr", tableArr[1]);
            
            jArgs.Add("key", (key == "-" ? "" : key));
            jArgs.Add("keyField", (keyField == "-" ? "" : keyField));
            jArgs.Add("includedFields", (includedFields == "-" ? "" : includedFields));
            jArgs.Add("filterExpression", (filterExpression == "-" ? "" : filterExpression));
            jArgs.Add("sortFields", (sortFields == "-" ? "" : sortFields));
            jArgs.Add("pageNumber", (!pageNumber.All(char.IsDigit) ? 0 : Convert.ToInt64(pageNumber)));
            jArgs.Add("pageSize", (!pageSize.All(char.IsDigit) ? 0 : Convert.ToInt64(pageSize)));
            jArgs.Add("requestConfig", (requestConfig == "-" ? "" : requestConfig));

            AppArgs.Add(_g.KEY_REQ_ARGS_ARR, new JArray() { jArgs });

            return ExecuteGetRequest();

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
