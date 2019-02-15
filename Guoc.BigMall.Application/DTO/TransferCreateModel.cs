using Guoc.BigMall.Infrastructure.DataAnnotations;
using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class TransferCreateModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        //public TransferType Type { get; set; }
        [ElmMinValue(1, "必须指定有效的调出门店。")]
        public int FromStoreId { get; set; }
        //public string FromStoreName { get; set; }
        //public string ToStoreName { get; set; }
        [ElmMinValue(1, "必须指定有效的调入门店。")]
        public int ToStoreId { get; set; }
        //public TransferStatus Status { get; set; }
        public string Remark { get; set; }
        //public DateTime CreatedOn { get; set; }
        //public int CreatedBy { get; set; }
        //public string CreatedByName { get; set; }
        public List<TransferItemCreateModel> Items { get; set; }

        //public string ItemsJson { get; set; }
    }
}
