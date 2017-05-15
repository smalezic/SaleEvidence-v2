using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using log4net;
using ADS.SaleEvidence.RetailServices.ObjectModel;

namespace ADS.SaleEvidence.RetailServices.DataBroker
{
    public class SaleEvidenceDbContext : DbContext
    {
        #region Fields

        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion Fields

        public DbSet<Terminal> Terminals { get; set; }
        public DbSet<SalePoint> SalePoints { get; set; }
        public DbSet<ObjectModel.Action> Actions { get; set; }
        public DbSet<Selling> Sellings { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Article> CodeBooks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _logger.Debug("On configuring");
            String connectionString = ConfigurationManager.ConnectionStrings["SaleEvidenceModelContainer"].ToString();
            _logger.DebugFormat("Connection string - {0}", connectionString);
            optionsBuilder.UseSqlServer(connectionString);
            _logger.Debug("On configuring is done");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _logger.Debug("Fluent API");

            modelBuilder.Entity<SalePoint>()
                .ToTable("LegalEntities_SalePoint");

            modelBuilder.Entity<Selling>()
                .ToTable("Actions_Selling");

            modelBuilder.Entity<Article>()
                .ToTable("Articles");

            modelBuilder.Entity<CodeBook>()
                .ToTable("CodeBooks");

            _logger.Debug("Fluent API is done");
        }
    }
}
