using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class StocktakingItem
    {
        public int Id { get; set; }
        public int StocktakingId { get; set; }
        public int ProductId { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public int Quantity { get; set; }
        public int FirstQuantity { get; set; }
        public int ComplexQuantity { get; set; }
        public int CheckQuantity { get; set; }
        public string Note { get; set; }
    }
}
