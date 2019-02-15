using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Domain.Service
{
    public class ProductPurchasePriceService
    {
        IDBContext _db;
        public ProductPurchasePriceService(IDBContext dbcontext)
        {
            this._db = dbcontext;
        }

        /// <summary>
        /// 查询商品当前的采购价。
        /// </summary>
        public ProductPurchasePrice QueryCurrentPurchasePrice(int productId)
        {
            var now = DateTime.Now;
            var productPurchasePrice = _db.Table<ProductPurchasePrice>().FirstOrDefault(p => p.ProductId == productId && p.Status == 1 && p.StartTime <= now && p.EndTime >= now);
            return productPurchasePrice;
        }
    }
}
