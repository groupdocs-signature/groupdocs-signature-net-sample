using System.Web.Mvc;
using System.Web.Routing;
using Groupdocs.Data.MsSql.Entity6.Signature;
using Groupdocs.Web.UI.Signature;

namespace MVCDemo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //GroupdocsSignature.Init(new GroupdocsSignatureSettings
            //{
            //    RootStoragePath = Server.MapPath("~/App_Data/"),
            //    DatabaseProvider = new MsSqlProvder(System.Configuration.ConfigurationManager.ConnectionStrings["SignatureDb"].ConnectionString)
            //});
        }

    }
}