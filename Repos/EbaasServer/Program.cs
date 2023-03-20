using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;

using Topshelf;
using Microsoft.Owin.Hosting;
using Ebaas.WebApi;
using Newtera.Common.Core;

namespace EbaasServer
{
    public class Program
    {
        static int Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<HostingConfiguration>();
                x.RunAsLocalSystem();
                x.SetDescription("Ebaas Server as Windows service");
                x.SetDisplayName("Ebaas Server");
                x.SetServiceName("Newtera.Ebaas.Server");
            });
            return (int)exitCode;
        }
    }

    public class HostingConfiguration : ServiceControl
    {
        private IDisposable _webApplication;

        public bool Start(HostControl hostControl)
        {
            string baseURL = ConfigurationManager.AppSettings[NewteraNameSpace.BASE_URL];

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[NewteraNameSpace.CURRENT_CULTURE]))
            {
                string culture = ConfigurationManager.AppSettings[NewteraNameSpace.CURRENT_CULTURE];
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(culture);
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo(culture);
            }

            Console.WriteLine("Starting the service on " + baseURL);
            _webApplication = WebApp.Start<Startup>(baseURL);
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _webApplication.Dispose();
            return true;
        }
    }
}
