using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;
namespace Guoc.BigMall.Application.ViewObject
{
    public class PurchaseOrderDto
    {
        public PurchaseOrderDto()
        {
            this.Items = new List<PurchaseOrderItemDto>();
        }
        public int Id { get; set; }
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string SupplierCode { get; set; }

        public string SupplierName { get; set; }

        public int SupplierId { get; set; }

        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
        public CBPurchaseOrderStatus Status { get; set; }

        public string PurchaseOrderStatus
        {
            get
            {
                return Status.Description();
            }
        }
        public DateTime CreatedOn { get; set; }

        public string CreatedTime
        {
            get
            {
                return CreatedOn.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public string CreatedByName { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedTime
        {
            get
            {
                return UpdatedOn.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public string UpdatedByName { get; set; }

        /// <summary>
        /// 供应商备注：可以备注单号，其他信息
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 明细
        /// </summary>
        public List<PurchaseOrderItemDto> Items { get; set; }
        ///// <summary>
        ///// 验证码
        ///// </summary>
        //public int VerifyCode { get; set; }

        ///// <summary>
        ///// 门店联系人
        ///// </summary>
        //public string StoreContact { get; set; }
        ///// <summary>
        ///// 门店电话
        ///// </summary>
        //public string StorePhone { get; set; }
        /// <summary>
        /// 门店地域
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 门店详细地址
        /// </summary>
        public string Address { get; set; }


        public string PurchaseGroupName { get; set; }
        public string PurchaseGroupCode { get; set; }
        public string SapOrderId { get; set; }

        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal TotalAmount
        {
            get
            {
                return this.Items.Sum(n => n.CostPrice * n.Quantity);
            }
        }
       
        
        public PurchaseOrderBillType BillType { get; set; }
        public string BillTypeName
        {
            get
            {
                return this.BillType.Description();
            }
        }
        public PurchaseOrderType OrderType { get; set; }
        public string OrderTypeName
        {
            get
            {
                return this.OrderType.Description();
            }
        }
       
    }
}
