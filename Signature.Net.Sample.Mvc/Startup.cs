using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Signature.Net.Sample.Mvc.Startup))]
namespace Signature.Net.Sample.Mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
