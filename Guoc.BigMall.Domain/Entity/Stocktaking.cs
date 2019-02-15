using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class Stocktaking
    {
        public int Id { get; set; }
        public int StocktakingPlanId { get; set; }
        public string Code { get; set; }
        public string StocktakingType { get; set; }
        public string ShelfCode { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int Status { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public int StoreId { get; set; }
        public string Note { get; set; }
    }
}
