using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Entity
{
    public class StoreInventoryBatchSNCode:BaseEntity
    {
        /// <summary>
        /// 商品SKUID
        /// </summary>
        public int ProductId { get; set; }
        
        /// <summary>
        /// 批次号
        /// </summary>
        public long BatchNo { get; set; }
        /// <summary>
        /// 串码
        /// </summary>
        public string SNCode { get; set; }
    }
}
