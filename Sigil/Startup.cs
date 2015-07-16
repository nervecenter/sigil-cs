using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sigil.Startup))]
namespace Sigil
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
