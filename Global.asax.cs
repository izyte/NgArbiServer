using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using DataAccess;
using System.Configuration;

namespace NgArbi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            /*************************** custom calls ******************************************/

            // Initialize data access connection and other properties
            DAL.connectionString = ConfigurationManager.ConnectionStrings["cnsAppAPI"].ConnectionString;
            DALGlobals.APP_SETTINGS = DataAccess.AppGlobals2.AppSetings;
            DALGlobals.GeneralRetObj = new ReturnObjectExternal();
            AppDataset.configPath = "";
            AppDataset.clientDevPath = "";

            DAL.LogMessage("Application Started ..");
            DAL.LogMessage("Schema Path: " + DataAccess.AppGlobals2.PATH_SCHEMA_CONFIG);
            DAL.LogMessage("Client Tables Path: " + DataAccess.AppGlobals2.PATH_TARGET_TYPESCRIPT_PATH);
            DAL.LogMessage(HttpContext.Current.Server.MapPath("App_Data"));

            // Initialize dataset
            AppDataset.Initialize();

        }
    }
}
