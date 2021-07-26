using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CarTp.Startup))]
namespace CarTp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
