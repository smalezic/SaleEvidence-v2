using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADS.SaleEvidence.RetailServices.FileListener
{
    public interface IWorker : IDisposable
    {
        void DoWork();
        String LoadFile(String fileName);
    }
}
