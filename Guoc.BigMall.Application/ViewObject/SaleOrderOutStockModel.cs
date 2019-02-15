using System.Collections.Generic;
namespace Guoc.BigMall.Application.ViewObject
{
    public class SaleOrderOutStockModel
    {
        public SaleOrderOutStockModel()
        {
            this.Items = new List<SaleOrderItemOutStockModel>();
        }

        /// <summary>
        /// 销售订单Id
        /// </summary>
        public int Id { get; set; }
        public string Code { get; set; }
        public string Remark { get; set; }

        public List<SaleOrderItemOutStockModel> Items { get; set; }
    }

    public class SaleOrderItemOutStockModel
    {
        public int ProductId { get; set; }

        public int ActualQuantity { get; set; }

        /// <summary>
        ///  多个逗号分隔
        /// </summary>
        public string SNCode { get; set; }

        public List<string> GetSNCodeList()
        {
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(this.SNCode)) {
                return list;
            }

            list.AddRange(this.SNCode.Split(','));
            return list;
        }
    }
}