using System;

namespace ADS.SaleEvidence.RetailServices.FileListener.FileProcessor
{
    public interface IWorker
    {
        void ProccessFile(String fileName);
    }
}
