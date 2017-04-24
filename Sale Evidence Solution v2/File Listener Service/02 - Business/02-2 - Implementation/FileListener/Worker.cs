using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADS.SaleEvidence.RetailServices.FileListener
{
    public class Worker : IWorker
    {
        #region Fields

        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private FileSystemWatcher _watcher;
        private String _folderName;

        #endregion Fields

        #region Constructors

        public Worker(String folderName)
        {
            var startTime = DateTime.Now;

            _logger.Debug("Constructing the 'Worker' object...");

            _folderName = folderName;

            _logger.DebugFormat("Construction is done in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        #endregion Constructors

        #region IWorker interface implementation

        public void DoWork()
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'DoWork'");

                _watcher = new FileSystemWatcher();
                _watcher.Path = _folderName;
                _watcher.NotifyFilter = NotifyFilters.LastWrite;
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

        public String LoadFile(String fileName)
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'LoadFile'");
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }

            _logger.DebugFormat("Method 'LoadFile' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
                
                return String.Format("Hello, {0}", fileName);
        }

        #endregion IWorker interface implementation

        #region IDisposable interface implementation

        public void Dispose()
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'Dispose'");

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
            StreamReader sr = null;

            try
            {
                _logger.Debug("Entered method 'OnChanged'");

                var file = e.FullPath;
                _logger.DebugFormat("File - {0}", file);

                sr = new StreamReader(file);
                var content = sr.ReadToEnd();

                _logger.DebugFormat("Content:{0}{1}", Environment.NewLine, content);
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            _logger.DebugFormat("Method 'OnChanged' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        #endregion Private methods
    }
}
