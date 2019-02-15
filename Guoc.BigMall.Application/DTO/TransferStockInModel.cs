using Guoc.BigMall.Infrastructure.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class TransferStockInModel
    {
        [ElmMinValue(1, "调拨单不存在。")]
        public int Id { get; set; }
        public List<TransferItemStockInModel> Items { get; set; }
    }
}
