using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Cotizador.Startup))]
namespace Cotizador
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
