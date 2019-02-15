using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;

namespace Guoc.BigMall.Application.ViewObject
{
    public class OrderProfitDto
    {
        [Description("单号")]
        public string SaleOrderCode { get; set; }
        [Description("制单日期")]
        public string CreateOnStr { get { return this.CreatedOn.Value.ToString("yyyy-MM-dd hh:mm:ss"); } }
        public DateTime? CreatedOn { get; set; }
        [Description("支付日期")]
        public string PaidDateStr { get { return this.PaidDate.HasValue ? this.PaidDate.Value.ToString("yyyy-MM-dd hh:mm:ss") : ""; } }
        public DateTime? PaidDate { get; set; }
        [Description("门店")]
        public string StoreName { get; set; }
        public int CategoryId { get; set; }
        [Description("类别")]
        public string CategoryName { get; set; }
        [Description("品牌")]
        public string BrandName { get; set; }
        [Description("供应商")]
        public string SupplierName { get; set; }

        public int ProductId { get; set; }
        [Description("商品编码")]
        public string ProductCode { get; set; }
        [Description("商品名称")]
        public string ProductName { get; set; }
        [Description("销售数量")]
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        [Description("进货成本")]
        public decimal CostAmount
        {
            get
            {
                return this.CostPrice * this.Quantity;
            }
        }
        [Description("销售单价")]
        public decimal RealPrice { get; set; }
        [Description("销售收入")]
        public decimal Amount
        {
            get
            {
                return this.RealPrice * this.Quantity;
            }
        }
        [Description("利润")]
        public decimal ProfitAmount
        {
            get
            {
                return (this.RealPrice - this.CostPrice) * Math.Abs(this.Quantity) * (this.Quantity < 0 ? -1 : 1);//退单数量为负
            }
        }

        public BillIdentity BillType { get; set; }


        public string BillTypeName
        {
            get
            {
                return this.BillType.Description();
            }
        }
        [Description("客户")]
        public string Buyer { get; set; }

        public int StoreId { get; set; }
        public int AuditedBy { get; set; }

        public string Remark { get; set; }



    }
}
