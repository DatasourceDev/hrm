using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Workflow.Startup))]
namespace Workflow
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
