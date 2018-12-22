using System;
using System.ServiceModel;

namespace PexesoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new ServiceHost(typeof(PexesoService.PexesoService)))
            {
                host.Open();
                Console.WriteLine("Server stared");

                Console.Read();
            }
        }
    }
}
