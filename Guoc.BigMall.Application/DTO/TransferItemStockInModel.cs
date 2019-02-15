using Guoc.BigMall.Infrastructure.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class TransferItemStockInModel
    {
        [ElmMinValue(1, "调拨单明细不存在。")]
        public int Id { get; set; }
        public int ActualReceivedQuantity { get; set; }
        public List<string> StockInSNCodes { get; set; }

        public TransferItemStockInModel()
        {
            this.StockInSNCodes = new List<string>();
        }
    }
}
