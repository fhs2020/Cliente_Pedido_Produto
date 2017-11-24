using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProductStore.Startup))]
namespace ProductStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
