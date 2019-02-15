
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Domain.ValueObject;

namespace Guoc.BigMall.Application.ViewObject
{
    public class StocktakingPlanDto
    {

        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public int Quantity { get; set; }
        public int? FirstQuantity { get; set; }
        public int? diffFristQuantity
        {
            get
            {
                if (!this.FirstQuantity.HasValue)
                {
                    return null;
                }
                else
                {
                    if (this.IsSNProduct == 1) return null;
                    return this.FirstQuantity.Value - this.Quantity;
                }
            }
        }
        public DateTime? StocktakingDate { get; set; }
        public int? ComplexQuantity { get; set; }
        public int? diffComplexQuantity
        {
            get
            {
                if (!this.ComplexQuantity.HasValue)
                {
                    return null;
                }
                else
                {
                    if (this.IsSNProduct == 1) return null;
                    return this.ComplexQuantity.Value - this.Quantity;
                }
            }
        }
        public DateTime? ComplexDate { get; set; }
        public string PlanCode { get; set; }
        public StocktakingPlanStatus Status { get; set; }
        public string StatusName { get { return this.Status.Description(); } }
        public string CategoryCodeAndName { get { return string.Format("[{0}]{1}", this.CategoryCode, this.CategoryName); } }
        public string BrandCodeAndName { get { return string.Format("[{0}]{1}", this.BrandCode, this.BrandName); } }
        public string StoreCodeAndName { get { return string.Format("[{0}]{1}", this.StoreCode, this.StoreName); } }
        public string SurplusSNCode { get; set; }
        public string MissingSNCode { get; set; }

        public string StocktakingDateStr
        {
            get
            {
                return (StocktakingDate == null ? null : StocktakingDate.Value.ToString("yyyy-MM-dd"));
            }
        }

        public string ComplexDateStr
        {
            get
            {
                return (ComplexDate == null ? null : ComplexDate.Value.ToString("yyyy-MM-dd"));
            }
        }

        public DateTime? CreatedOn { get; set; }

        public string CreatedOnStr
        {
            get
            {
                return (CreatedOn == null ? null : CreatedOn.Value.ToString("yyyy-MM-dd"));
            }
        }

        public int IsSNProduct { get; set; }

        public string SNProduct
        {
            get
            {
                return this.IsSNProduct == 1 ? "是" : "否";
            }
        }

        public bool IsPushSap { get; set; }

        public string PushSap
        {
            get { return this.IsPushSap ? "成功" : "未推送"; }
        }
    }
}
