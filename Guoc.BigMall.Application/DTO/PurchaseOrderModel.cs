using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class PurchaseOrderModel
    {
        public PurchaseOrderModel()
        {
           
        }
        public int Id { get; set; }
        public string Code { get; set; }
        public int SupplierId { get; set; }

        public string SupplierCode { get; set; }
        public int StoreId { get; set; }

        public string StoreCode { get; set; }

        public int OrderType { get; set; }

        public string Remark { get; set; }

        public string EditByName { get; set; }
        public int EditBy { get; set; }
     
        /// <summary>
        /// 商品明细json 串
        /// </summary>
        public string Items { get; set; }

        public int SaleOrderId { get; set; }

       
        /// 合并支付的所有单据ID，逗号分隔
        /// </summary>
        public string Ids { get; set; }

       

    }
}
