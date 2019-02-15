using Guoc.BigMall.Infrastructure.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class TransferItemEditModel
    {
        public int Id { get; set; }

        public int TransferOrderId { get; set; }

        [ElmMinValue(1, "商品不存在。")]
        public int ProductId { get; set; }

        [ElmMinValue(1, "调拨数量必须 ≥ 1。")]
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Amount { get { return this.Price * this.Quantity; } }
    }
}
