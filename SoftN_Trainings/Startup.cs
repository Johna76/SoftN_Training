using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SoftN_Trainings.Startup))]
namespace SoftN_Trainings
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
