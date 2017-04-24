using ADS.SaleEvidence.RetailServices.FileListener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ADS.SaleEvidence.RetailServices.ConsoleServiceHost
{
    internal class FileListenerServiceConfiguration
    {
        internal static void Configure(IWorker worker)
        {
            HostFactory.Run(configure =>
            {
                configure.Service<FileListenerService>(service =>
                {
                    service.ConstructUsing(s => new FileListenerService(worker));
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });

                // Setup account that windows service uses for runing
                configure.RunAsLocalSystem();
                configure.SetServiceName("CashRegistersFileListenerService");
                configure.SetDisplayName("Cash Registers File Listener");
                //configure.SetDescription();
            });
        }
    }
}
