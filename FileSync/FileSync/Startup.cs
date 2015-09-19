using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FileSync.Startup))]
namespace FileSync
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
