using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.IO;
using Microsoft.EntityFrameworkCore;
using ADS.SaleEvidence.RetailServices.RepositoryActivity;
using ADS.SaleEvidence.RetailServices.ObjectModel;
using System.Transactions;

namespace ADS.SaleEvidence.RetailServices.FileListener
{
    public class Worker : IWorker
    {
        #region Fields

        private const char UNDERSCORE = '_';
        private const char TAB = '\t';

        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IDataActivity _dataActivity;

        private struct FileNameParts
        {
            public String CachRegisterName { get; set; }
            public String ReportType { get; set; }
            public DateTime DateTimeStamp { get; set; }
        }

        #endregion Fields

        #region Constructors

        public Worker(IDataActivity dataActivity)
        {
            _dataActivity = dataActivity;
        }

        #endregion Constructors

        #region Interface Implementation

        public void ProccessFile(string fileName)
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'ProccessFile'");

                //SaleEvidenceDbContext context = new SaleEvidenceDbContext();
                //var terminals = context.Terminals;
                //var salepoints = context.SalePoints;
                //var actions = context.Actions.Take(10);
                //var sellings = context.Sellings
                //    .Join(context.Actions,
                //        selling => selling.Id,
                //        action => action.Id,
                //        (selling, action) => new { Selling = selling, Action = action }
                //    )
                //    //.Include(it => it.Article)
                //    .Take(10);
                //var articles = context.Articles.Take(10);

                //var terminals = _dataActivity.GetAll<Terminal>();

                // Getting the content of the file
                String content = ReadFile(fileName);
                
                // Parsing data from the file and store info to the database
                ProcessData(fileName, content);

                // Get rid of the processed file
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

        private void ProcessData(String fileName, String fileContent)
        {
            var startTime = DateTime.Now;

            try
            {
                _logger.Debug("Entered method 'ProcessData'");

                _logger.Debug("Get data from file name");
                var nameParts = ResolveFileName(fileName);

                _logger.DebugFormat("Find Terminal entity by cach register name - {0}", nameParts.CachRegisterName);
                var terminal = _dataActivity.GetByCriteria<Terminal>(it => it.mPOSId == nameParts.CachRegisterName)
                    .FirstOrDefault();
                
                if (terminal != null)
                {
                    _logger.Debug("Prepare content for storing data to database");
                    var lines = fileContent.Split(Environment.NewLine.ToCharArray())
                        .Where(it => String.IsNullOrWhiteSpace(it) == false);
                    _logger.DebugFormat("Number of lines - {0}", lines.Count());

                    int counter = 0;

                    // Example of file for processing
                    /*
                     * 25/04/17 20:04 Event:ZRepBegin
                     * ID	Barcode	Name	Price	Quantity	Turnover
                     * 1	0	COCA COLA 250 ML	130.00	3.0	390.0
                     * 2	0	FANTA 250ml	130.00	2.0	260.0
                     * 3	8600115360215	Marlboro	270.00	3.0	810.0
                     * 4	0	COCTA 200ml	130.00	5.0	650.0
                     * ---END OF REPORT---
                    */

                    lines.ToList().ForEach(line =>
                    {
                        _logger.DebugFormat("Line {0} - {1}", ++counter, line);

                        // The first two and the last line will be skipped as well as the first column
                        if (counter > 2 && counter < lines.Count())
                        {
                            try
                            {
                                _logger.Debug("Parsing the line");

                                var data = line.Split(TAB);

                                var barcode = data[1];
                                var articleName = data[2];
                                var price = data[3];
                                var quantity = data[4].Split('.').FirstOrDefault();

                                // Check barcode from line
                                if (String.IsNullOrWhiteSpace(barcode) || barcode == "0")
                                {
                                    // Barcode is unknown, there is no exact way to figure out which product is about
                                    _logger.Warn("Barcode is unknown!");

                                    // TODO: Search through CodeBook for Code field and compare with it
                                }
                                else
                                {
                                    _logger.DebugFormat("Find article - '{0}'", barcode);
                                    var article = _dataActivity.GetByCriteria<Article>(it => it.Barcode == barcode)
                                        .FirstOrDefault();
                                    if(article == null)
                                    {
                                        // For some barcodes the last digit isn't stored
                                        var shortenBarcode = barcode.Substring(0, barcode.Length - 1);
                                        _logger.DebugFormat("Find article for shorten barcode - '{0}'", shortenBarcode);
                                        article = _dataActivity.GetByCriteria<Article>(it => it.Barcode == shortenBarcode)
                                            .FirstOrDefault();
                                    }

                                    if (article != null)
                                    {
                                        _logger.DebugFormat("Article found - {0}", article.Name);

                                        _logger.DebugFormat("Parsing the quantity - '{0}'", quantity);
                                        short amount = 0;
                                        short.TryParse(quantity, out amount);
                                        _logger.DebugFormat("amount - '{0}'", amount);

                                        if (amount != 0)
                                        {
                                            // Check prices from CodeBook. If selling prices differs, update the price in CodeBook
                                            _logger.DebugFormat("Find the record from CodeBook by SalePointId - {0} and ArticleId - {1}", terminal.SalePointId, article.Id);
                                            var codeBook = _dataActivity.GetByCriteria<CodeBook>(it =>
                                                    it.SalePointId == terminal.SalePointId
                                                    && it.ArticleId == article.Id)
                                                .SingleOrDefault();

                                            if (codeBook != null)
                                            {
                                                _logger.DebugFormat("CodeBook.Id - {0}", codeBook.Id);
                                            }
                                            else
                                            {
                                                _logger.Warn("The CodeBook wasn't found!");
                                                codeBook = new CodeBook()
                                                {
                                                    SalePointId = terminal.SalePointId,
                                                    ArticleId = article.Id
                                                };
                                            }

                                            _logger.DebugFormat("Parsing the price - '{0}'", price);
                                            decimal sellingPrice = 0m;
                                            decimal.TryParse(price, out sellingPrice);
                                            _logger.DebugFormat("sellingPrice - '{0}'", sellingPrice);

                                            //using (var scope = new TransactionScope())
                                            //{
                                                // Update the CodeBook record
                                                codeBook.SellingPrice = sellingPrice;
                                                //codeBook.Name = articleName.Trim();
                                               
                                                if(codeBook.StoreAmount != null)
                                                {
                                                    codeBook.StoreAmount -= amount;
                                                }

                                                _logger.Debug("Updating the CodeBook");
                                                _dataActivity.Save<CodeBook>(codeBook);

                                                // Storing selling to the database
                                                _logger.Debug("Create an Action entity");
                                                var action = new ObjectModel.Action()
                                                {
                                                    InitTime = nameParts.DateTimeStamp,
                                                    Calculated = false,
                                                    SalePointId = terminal.SalePointId
                                                };

                                                _logger.Debug("Store Action entity to the database");
                                                action = _dataActivity.Save<ObjectModel.Action>(action);
                                                _logger.Debug("Create a Selling entity");
                                                var selling = new Selling()
                                                {
                                                    Id = action.Id,
                                                    ArticleId = article.Id,
                                                    Quantity = amount,
                                                    InputPrice = codeBook.InputPrice,
                                                    UnitPrice = codeBook.SellingPrice
                                                };

                                                _logger.Debug("Store Selling entity to the database");
                                                selling = _dataActivity.Save<Selling>(selling);

                                                _logger.Debug("All changes are saved");
                                            //    scope.Complete();
                                            //}
                                        }
                                        else
                                        {
                                            _logger.Warn("The amount has no value!");
                                        }
                                    }
                                    else
                                    {
                                        _logger.WarnFormat("The barcode {0} was not found!", barcode);
                                    }
                                }
                            }
                            catch (Exception exc)
                            {
                                // Log the error and continue
                                _logger.ErrorFormat("Error - {0}", exc.Message);
                            }
                        }
                        else
                        {
                            _logger.DebugFormat("This line is skipped - {0}. {1}", counter, line);
                        }
                    });
                }
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
            }

            _logger.DebugFormat("Method 'ProcessData' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
        }

        private FileNameParts ResolveFileName(String fileName)
        {
            var startTime = DateTime.Now;
            FileNameParts retVal;

            try
            {
                _logger.DebugFormat("Entered method 'ResolveFileName', fileName - {0}", fileName);

                fileName = Path.GetFileNameWithoutExtension(fileName);
                
                var nameParts = fileName.Split(UNDERSCORE);

                // Resolve date and time
                var dateString = nameParts[2];
                var year = int.Parse(String.Format("20{0}", dateString.Substring(0, 2)));
                var month = int.Parse(dateString.Substring(2, 2));
                var day = int.Parse(dateString.Substring(4));

                var timeString = nameParts[3];
                var hour = int.Parse(timeString.Substring(0, 2));
                var minute = int.Parse(timeString.Substring(2, 2));
                var second = int.Parse(timeString.Substring(4));

                _logger.DebugFormat("DateTime string - {0}-{1}-{2} {3}:{4}:{5}", year, month, day, hour, minute, second);

                DateTime transactionDate = new DateTime(year, month, day, hour, minute, second);

                retVal = new FileNameParts()
                {
                    CachRegisterName = nameParts[0],
                    ReportType = nameParts[1],
                    DateTimeStamp = transactionDate
                };

            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }

            _logger.DebugFormat("Method 'ResolveFileName' has been completed in {0}ms", (DateTime.Now - startTime).TotalMilliseconds);
            return retVal;
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
