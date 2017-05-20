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
        private String _fileExtension;

        private FabricModule _fabricModule;

        #endregion Fields

        #region Constructors

        public Dispatcher(String folderName, String fileExtension)
        {
            _folderName = folderName;
            _fileExtension = fileExtension;

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

                _watcher = new FileSystemWatcher()
                {
                    Path = _folderName,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size,
                    Filter = "*.*"
                };
                _watcher.Changed += new FileSystemEventHandler(OnChanged);
                _watcher.EnableRaisingEvents = true;

                Task processExistingFiles = Task.Factory.StartNew( () => CheckFolderForAlreadyExistingFiles());
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
                
                var worker = GetWorker();

                _logger.DebugFormat("Process the file - {0}", fileName);
                worker.ProccessFile(fileName);
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }

            _logger.DebugFormat("Method 'OnChanged' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        private void CheckFolderForAlreadyExistingFiles()
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'CheckFolderForAlreadyExistingFiles'");
                var folder = new DirectoryInfo(_folderName);

                var allFiles = folder.EnumerateFiles();
                _logger.DebugFormat("Number of files - {0}", allFiles.Count());

                var files = allFiles.Where(it => it.Extension == _fileExtension);
                _logger.DebugFormat("Number of {0} files - {1}", _fileExtension, allFiles.Count());

                files.ToList().ForEach(it =>
                {
                    var worker = GetWorker();

                    _logger.DebugFormat("Process the file - {0}", it.FullName);
                    worker.ProccessFile(it.FullName);
                });
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }

            _logger.DebugFormat("Method 'CheckFolderForAlreadyExistingFiles' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        private IWorker GetWorker()
        {
            var startTime = DateTime.Now;
            IWorker worker;

            try
            {
                _logger.Debug("Entered method 'GetWorker'");

                var dataActivity = _fabricModule.Resolve<IDataActivity>();
                worker = _fabricModule.ResolveWorker(dataActivity);
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }

            _logger.DebugFormat("Method 'GetWorker' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
            return worker;
        }

        #endregion Private methods
    }
}
