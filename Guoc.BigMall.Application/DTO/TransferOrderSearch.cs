using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class TransferOrderSearch
    {
        public string Code { get; set; }
        public TransferType? Type { get; set; }
        public TransferStatus? Status { get; set; }
        public string FromStoreId { get; set; }
        public string ToStoreId { get; set; }
        public long? BatchNo { get; set; }

        public string SAPCode { get; set; }

        public string ProductCode { get; set; }

        public string SNCode { get; set; }

        public string ProductName { get; set; }

        public string CategoryId { get; set; }

        public string CategoryCode { get; set; }
        public string BrandIds { get; set; }

        public string BrandCode { get; set; }

        public DateTime? CreateOnFrom { get; set; }
        public DateTime? CreateOnTo { get; set; }

        public string ProductIds { get; set; }

        public bool? IsPushSap { get; set; }
    }
}
