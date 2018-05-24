using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PortalCShar.Startup))]
namespace PortalCShar
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
