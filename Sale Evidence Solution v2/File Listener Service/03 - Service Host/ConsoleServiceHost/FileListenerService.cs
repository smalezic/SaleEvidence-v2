using ADS.SaleEvidence.RetailServices.FileListener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace ADS.SaleEvidence.RetailServices.ConsoleServiceHost
{
    public class FileListenerService
    {
        #region Fields
        
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IDispatcher _dispatcher;

        #endregion Fields

        #region Constructor
        public FileListenerService(IDispatcher dispatcher)
        {
            var startTime = DateTime.Now;

            _logger.Debug("Constructing the 'FileListenerService' object...");

            _dispatcher = dispatcher;

            _logger.DebugFormat("Construction is done in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        #endregion Constructor

        #region Starting & Stopping service

        public void Start()
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'Start'");

                _dispatcher.DoWork();
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }

            _logger.DebugFormat("Method 'Start' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        public void Stop()
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'Stop'");

                _dispatcher.Dispose();
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }

            _logger.DebugFormat("Method 'Stop' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        #endregion Starting & Stopping service
    }
}
