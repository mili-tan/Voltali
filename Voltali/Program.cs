using System;
using Nancy.Hosting.Self;

namespace Voltali
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Voltali Download Manager");
            Console.WriteLine("-------------------");

            using (NancyHost host = new NancyHost(new HostConfiguration
                    {RewriteLocalhost = true, UrlReservations = new UrlReservations {CreateAutomatically = true}},
                new Uri("http://localhost:2020/")))
            {
                host.Start();

                Console.WriteLine("Voltali Download Manager is running on 2020 port");
                Console.WriteLine("Press any [Enter] to close the host.");
                Console.ReadLine();
            }
        }
    }
}
