using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.IO;

namespace ADS.SaleEvidence.RetailServices.FileListener
{
    public class Worker : IWorker
    {
        #region Fields

        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion Fields

        #region Constructors

        public Worker()
        {

        }

        #endregion Constructors

        #region Interface Implementation

        public void ProccessFile(string fileName)
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'ProccessFile'");

                String content = ReadFile(fileName);

                ProccessData(content);

                DeleteFile(fileName);
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
            }

            _logger.DebugFormat("Method 'ProccessFile' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }
        
        #endregion Interface Implementation

        #region Private Methods

        private String ReadFile(String fileName)
        {
            var startTime = DateTime.Now;
            String retVal;
            StreamReader sr = null;

            try
            {
                _logger.Debug("Entered method 'ReadFile'");

                sr = new StreamReader(fileName);
                retVal = sr.ReadToEnd();
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                retVal = String.Empty;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            _logger.DebugFormat("Method 'ReadFile' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
            return retVal;
        }

        private void ProccessData(String fileContent)
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'ProccessData'");

                var lines = fileContent.Split(Environment.NewLine.ToCharArray())
                    .Where(it => String.IsNullOrEmpty(it) == false);
                _logger.DebugFormat("Number of lines - {0}", lines.Count());

                int counter = 0;
                lines.ToList().ForEach(line =>
                {
                    _logger.DebugFormat("Line {0} - {1}", ++counter, line);
                });
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
            }

            _logger.DebugFormat("Method 'ProccessData' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        private void DeleteFile(String fileName)
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'DeleteFile'");

                File.Delete(fileName);
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
            }

            _logger.DebugFormat("Method 'DeleteFile' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        #endregion Private Methods
    }
}
