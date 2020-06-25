using Microsoft.Owin;
using Owin;
[assembly: OwinStartupAttribute(typeof(Time.Startup))]
namespace Time
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
