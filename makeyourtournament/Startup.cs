using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(makeyourtournament.Startup))]
namespace makeyourtournament
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
