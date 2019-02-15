using Guoc.BigMall.Infrastructure.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class TransferEditModel
    {
        [ElmMinValue(1, "调拨单不存在。")]
        public int Id { get; set; }
        public string Code { get; set; }
        [ElmMinValue(1, "必须指定有效的调出门店。")]
        public int FromStoreId { get; set; }
        [ElmMinValue(1, "必须指定有效的调入门店。")]
        public int ToStoreId { get; set; }
        public string Remark { get; set; }
        public List<TransferItemEditModel> Items { get; set; }
    }
}
