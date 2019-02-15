using Guoc.BigMall.Infrastructure.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class TransferItemStockOutModel
    {
        [ElmMinValue(1, "调拨单明细不存在。")]
        public int Id { get; set; }
        public int ActualShipmentQuantity { get; set; }
        public List<string> SNCodes { get; set; }

        public TransferItemStockOutModel()
        {
            this.SNCodes = new List<string>();
        }
    }
}
