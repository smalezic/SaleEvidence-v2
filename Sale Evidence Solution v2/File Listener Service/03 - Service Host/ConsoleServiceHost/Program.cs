﻿using ADS.SaleEvidence.Common.CompositionRoot;
using ADS.SaleEvidence.RetailServices.FileListener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using log4net;
using ADS.SaleEvidence.RetailServices.RepositoryActivity;

namespace ADS.SaleEvidence.RetailServices.ConsoleServiceHost
{
    public class Program
    {
        #region Fields

        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion Fields

        #region Main

        static void Main(string[] args)
        {
            _logger.Debug("File Listener Service is about to start...");

            //FabricModule fabricModule = new FabricModule();
            //fabricModule.Load();

            var folderName = ConfigurationManager.AppSettings["FolderPath"];
            //var dataActivity = fabricModule.Resolve<IDataActivity>();
            //var worker = fabricModule.ResolveWorker(dataActivity);
            //var dispatcher = fabricModule.ResolveDispatcher(worker, folderName);

            var dispatcher = new Dispatcher(folderName);

            _logger.Debug("Starting the service!");
            FileListenerServiceConfiguration.Configure(dispatcher);

            _logger.DebugFormat("File Listener Service has been stopped");
        }

        #endregion Main
    }
}
