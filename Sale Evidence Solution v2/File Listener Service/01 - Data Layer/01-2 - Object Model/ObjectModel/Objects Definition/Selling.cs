using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADS.SaleEvidence.RetailServices.ObjectModel
{
    public partial class Selling
    {
        [ForeignKey("Action")]
        public int Id { get; set; }
        [ForeignKey("Article")]
        public int ArticleId { get; set; }

        public short Quantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> InputPrice { get; set; }

        public virtual Article Article { get; set; }

        public virtual ObjectModel.Action Action { get; set; }
    }
}
