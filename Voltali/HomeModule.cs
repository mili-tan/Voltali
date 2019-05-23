using Nancy;

namespace Voltali
{
    public sealed class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", x => "Voltali Download Manager");
        }
    }
}
