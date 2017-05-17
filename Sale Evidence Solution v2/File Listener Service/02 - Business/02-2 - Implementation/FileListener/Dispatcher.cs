using ADS.SaleEvidence.Common.CompositionRoot;
using ADS.SaleEvidence.RetailServices.FileListener.FileProcessor;
using ADS.SaleEvidence.RetailServices.RepositoryActivity;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADS.SaleEvidence.RetailServices.FileListener
{
    public class Dispatcher : IDispatcher
    {
        #region Fields

        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private FileSystemWatcher _watcher;

        //private IWorker _worker;
        private String _folderName;

        private FabricModule _fabricModule;

        #endregion Fields

        #region Constructors

        public Dispatcher(String folderName)
        {
            _folderName = folderName;

            _fabricModule = new FabricModule();
            _fabricModule.Load();
        }

        //public Dispatcher(IWorker worker, String folderName)
        //{
        //    var startTime = DateTime.Now;

        //    _logger.Debug("Constructing the 'Worker' object...");

        //    _worker = worker;
        //    _folderName = folderName;

        //    _logger.DebugFormat("Construction is done in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        //}

        #endregion Constructors

        #region IDispatcher interface implementation

        public void DoWork()
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'DoWork'");

                _watcher = new FileSystemWatcher();
                _watcher.Path = _folderName;
                _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
                _watcher.Filter = "*.*";
                _watcher.Changed += new FileSystemEventHandler(OnChanged);
                _watcher.EnableRaisingEvents = true;

            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }

            _logger.DebugFormat("Method 'DoWork' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        #endregion IDispatcher interface implementation

        #region IDisposable interface implementation

        public void Dispose()
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'Dispose'");

                _watcher.Changed -= OnChanged;

                _watcher.Dispose();
                _watcher = null;
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }

            _logger.DebugFormat("Method 'Dispose' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        #endregion IDisposable interface implementation

        #region Private methods

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'OnChanged'");

                var fileName = e.FullPath;
                _logger.DebugFormat("File - {0}", fileName);

                _logger.Debug("Wait for one second for finishing file coping");
                System.Threading.Thread.Sleep(1000);

                var dataActivity = _fabricModule.Resolve<IDataActivity>();
                var worker = _fabricModule.ResolveWorker(dataActivity);

                worker.ProccessFile(fileName);
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }

            _logger.DebugFormat("Method 'OnChanged' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        #endregion Private methods
    }
}
