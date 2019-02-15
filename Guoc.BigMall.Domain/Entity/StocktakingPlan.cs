using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;

namespace Guoc.BigMall.Domain.Entity
{
    public class StocktakingPlan
    {
        public StocktakingPlan()
        {
            this.Items = new List<StocktakingPlanItem>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int Method { get; set; }
        public StocktakingPlanStatus Status { get; set; }
        public int StoreId { get; set; }
        public string Note { get; set; }
        public DateTime? StocktakingDate { get; set; }
        public DateTime? ComplexDate { get; set; }

        public string StoreCode { get; set; }

        public bool IsPushSap { get; set; }
        public List<StocktakingPlanItem> Items { get; set; }
    }
}
