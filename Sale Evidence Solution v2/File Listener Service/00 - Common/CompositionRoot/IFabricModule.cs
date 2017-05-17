using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ADS.SaleEvidence.RetailServices.FileListener;

namespace ADS.SaleEvidence.Common.CompositionRoot
{
    public interface IFabricModule
    {
        String Name { get; }

        void Load();
        void Load(ContainerBuilder builder);

        T Resolve<T>() where T : class;

        //T Resolve<T>(String folderName) where T : class;
        //IDispatcher ResolveDispatcher(IWorker worker, String folderName);
    }
}
