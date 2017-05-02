using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using ADS.SaleEvidence.RetailServices.FileListener;
using log4net;
using ADS.SaleEvidence.RetailServices.RepositoryActivity;

namespace ADS.SaleEvidence.Common.CompositionRoot
{
    public class FabricModule : IFabricModule
    {
        #region Fields

        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static IContainer _container { get; set; }
        public string Name => throw new NotImplementedException();

        #endregion Fields

        #region Load

        public void Load()
        {
            try
            {
                Load(new ContainerBuilder());
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }
        }

        public void Load(ContainerBuilder builder)
        {
            try
            {
                // Register implementations of interfaces here
                builder.RegisterType<Dispatcher>().As<IDispatcher>();
                builder.RegisterType<Worker>().As<IWorker>();
                builder.RegisterType<DataActivity>().As<IDataActivity>();

                // Build the container
                _container = builder.Build();
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }
        }

        #endregion Load

        #region Resolve

        public T Resolve<T>() where T : class
        {
            try
            {
                ILifetimeScope clientLifetimeScope = _container.BeginLifetimeScope();

                return clientLifetimeScope.Resolve<T>();
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }
        }

        public IWorker ResolveWorker(IDataActivity dataActivity)
        {
            try
            {
                var paramList = new List<ResolvedParameter>
                {
                    new ResolvedParameter(
                            (pi, ctx) => pi.ParameterType == typeof(IDataActivity) && pi.Name == "dataActivity",
                            (pi, ctx) => dataActivity
                        )
                };
                ILifetimeScope clientLifetimeScope = _container.BeginLifetimeScope();

                return clientLifetimeScope.Resolve<IWorker>(paramList);
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }
        }

        public IDispatcher ResolveDispatcher(IWorker worker, String folderName)
        {
            try
            {
                var paramList = new List<ResolvedParameter>
                {
                    new ResolvedParameter(
                            (pi, ctx) => pi.ParameterType == typeof(IWorker) && pi.Name == "worker",
                            (pi, ctx) => worker
                        ),
                    new ResolvedParameter(
                            (pi, ctx) => pi.ParameterType == typeof(String) && pi.Name == "folderName",
                            (pi, ctx) => folderName
                        )
                };
                ILifetimeScope clientLifetimeScope = _container.BeginLifetimeScope();

                return clientLifetimeScope.Resolve<IDispatcher>(paramList);
            }
            catch (Exception exc)
            {
                _logger.Error("Error - ", exc);
                throw;
            }
        }

        #endregion Resolve
    }
}
