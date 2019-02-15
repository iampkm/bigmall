using System;
using System.Collections.Generic;
using System.Linq;

using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Infrastructure.Log;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using System.Dynamic;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Infrastructure;

namespace Guoc.BigMall.Application.Facade
{
    public class ProductPriceFacade : IProductPriceFacade
    {
        private readonly IDBContext _db;
        private readonly ILogger _logger;

        public ProductPriceFacade(IDBContext db, ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public ProductPriceDto GetCurrentPriceByProductCode(int storeId, string productCode)
        {
            var sql = @"SELECT pp.*
                        FROM Product p
                        INNER JOIN ProductPrice pp on p.Id = pp.ProductId
                        WHERE p.Code = @ProductCode
                          AND p.StoreId = @StoreId
                          AND pp.StartTime <= GETDATE()
                          AND pp.EndTime >= GETDATE()";  //todo status 字段是否需要加上

            var prices = _db.DataBase.Query<ProductPrice>(sql, new { StoreId = storeId, ProductCode = productCode });
            if (prices == null && prices.Count() == 0)
                throw new Exception("查无此商品:{0} 价格".FormatWith(productCode));

            var availablePrice = prices.OrderByDescending(p => p.PriceType).FirstOrDefault();
            var priceDto = new ProductPriceDto();
            availablePrice.MapTo(priceDto);

            return priceDto;
        }

        public List<ProductPriceDto> GetCurrentPriceByProductCodes(int storeId, List<string> productCodes)
        {
            var sql = @"SELECT pp.*
                        FROM Product p
                        INNER JOIN ProductPrice pp on p.Id = pp.ProductId
                        INNER JOIN (
                            SELECT ProductId, MAX(PriceType) AS 'PriceType'
                            FROM ProductPrice
                            GROUP BY ProductId
                        ) AS pp2 ON pp.ProductId = pp2.ProductId AND pp.PriceType = pp2.PriceType
                        WHERE p.Code IN @ProductCodes
                          AND p.StoreId = @StoreId
                          AND pp.StartTime <= GETDATE()
                          AND pp.EndTime >= GETDATE()";  //todo status 字段是否需要加上

            var dtos = new List<ProductPriceDto>();

            var prices = _db.DataBase.Query<ProductPrice>(sql, new { StoreId = storeId, ProductCodes = productCodes.ToArray() });
            if (prices == null && prices.Count() == 0)
                throw new Exception("查不到商品价格");

            prices.MapTo(dtos);

            return dtos;
        }

        public ProductPriceDto GetCurrentPriceByProductId(int storeId, int productId)
        {
            var sql = @"SELECT pp.*
                        FROM Product p
                        INNER JOIN ProductPrice pp on p.Id = pp.ProductId
                        WHERE p.Id = @ProductId
                          AND pp.StartTime <= GETDATE()
                          AND pp.EndTime >= GETDATE()";  //todo status 字段是否需要加上 AND pp.StoreId = @StoreId

            var productPrices = _db.DataBase.Query<ProductPrice>(sql, new { StoreId = storeId, ProductId = productId });
            if (productPrices == null && productPrices.Count() == 0)
                throw new Exception("查无此商品价格：{0}".FormatWith(productId));

            var price = productPrices.OrderByDescending(p => p.PriceType).FirstOrDefault();
            var priceDto = new ProductPriceDto();
            price.MapTo(priceDto);

            return priceDto;
        }

        public List<ProductPriceDto> GetCurrentPriceByProductIds(int storeId, List<int> productIds)
        {
            var sql = @"SELECT pp.*
                        FROM Product p
                        INNER JOIN ProductPrice pp on p.Id = pp.ProductId
                        INNER JOIN (
                            SELECT ProductId, MAX(PriceType) AS 'PriceType'
                            FROM ProductPrice
                            GROUP BY ProductId
                        ) AS pp2 ON pp.ProductId = pp2.ProductId AND pp.PriceType = pp2.PriceType
                        WHERE p.Id IN @ProductIds
                          AND p.StoreId = @StoreId
                          AND pp.StartTime <= GETDATE()
                          AND pp.EndTime >= GETDATE()";  //todo status 字段是否需要加上

            var dtos = new List<ProductPriceDto>();

            var prices = _db.DataBase.Query<ProductPrice>(sql, new { StoreId = storeId, ProductIds = productIds.ToArray() });
            if (prices == null && prices.Count() == 0)
                throw new Exception("查不到商品价格");

            prices.MapTo(dtos);

            return dtos;
        }


        public List<ViewObject.StoreProductPriceDto> QueryStoreProductPrice(ViewObject.Pager page, Search.SearchStoreProductPrice condition)
        {
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrEmpty(condition.StoreId) && condition.StoreId != "0")
            {
                where += "and si.StoreId in @StoreId ";
                param.StoreId = condition.StoreId.Split(',').ToIntArray(); ;
            }
            if (!string.IsNullOrEmpty(condition.ProductCode))
            {
                where += "and p.Code like @ProductCode  ";
                param.ProductCode = string.Format("{0}%", condition.ProductCode);
            }

            if (!string.IsNullOrEmpty(condition.ProductName))
            {
                where += "and p.Name like @ProductName ";
                param.ProductName = string.Format("%{0}%", condition.ProductName);
            }

            if (!string.IsNullOrEmpty(condition.BrandId))
            {
                where += "and b.Id in  @BrandId ";
                param.BrandId = condition.BrandId.Split(',').ToIntArray();
            }
            if (!string.IsNullOrEmpty(condition.CategoryCode))
            {
                where += "and c.Code like @CategoryCode ";
                param.CategoryCode = string.Format("{0}%", condition.CategoryCode);
            }
            if (!string.IsNullOrEmpty(condition.Mark) && !string.IsNullOrEmpty(condition.Quantity))
            {
                where += "and si.Quantity " + condition.Mark + " @Quantity ";
                param.Quantity = condition.Quantity;
            }

            string sql = @"select * from (
                            select ROW_NUMBER() OVER ( ORDER BY si.Id DESC ) AS rownum ,si.Id,
 p.Name as ProductName,p.Code as ProductCode,p.Spec as Specification,p.TaxRate,p.HasSNCode,
s.Name  as StoreName,s.Code as StoreCode ,c.FullName as CategoryName,b.Name as BrandName,si.Quantity,
(select top 1 SalePrice from productPrice sp
where p.Id= sp.ProductId and priceType =1 and startTime<=getdate() and endTime>=getdate() 
and (sp.storeId = 0  or sp.StoreId = si.StoreId) order by sp.Slevel desc,sp.id desc) SalePrice ,
(select top 1 SalePrice from productPrice sp
where p.Id= sp.ProductId and priceType =2 and startTime<=getdate() and endTime>=getdate() 
and (sp.storeId = 0  or sp.StoreId = si.StoreId) order by sp.Slevel desc,sp.id desc) MinSalePrice 
 from StoreInventory si
left join product p on si.ProductId = p.Id
left join Store s on s.Id = si.StoreId
left join Category c on c.Id = p.CategoryId
left join Brand b on b.Id = p.BrandId
                            where 1=1 {0} ) t ";

            if (!page.toExcel)
            {
                sql += string.Format(" where  t.rownum BETWEEN {0} AND {1} ", (page.PageIndex - 1) * page.PageSize + 1, page.PageIndex * page.PageSize);
            }
            sql = string.Format(sql, where);
            var rows = this._db.DataBase.Query<StoreProductPriceDto>(sql, param);
            if (page.toExcel)
            {
                return rows;
            }
            string sqlcount = string.Format(@" select  count(1)
                                                from StoreInventory si
left join product p on si.ProductId = p.Id
left join Store s on s.Id = si.StoreId
left join Category c on c.Id = p.CategoryId
left join Brand b on b.Id = p.BrandId   where 1=1 {0}", where);
            page.Total = _db.DataBase.ExecuteScalar<int>(sqlcount, param);
            return rows;
        }


        public StoreProductPriceDto GetStoreProductPrice(int storeId, int productId)
        {
            var sql = @"select 
p.Name as ProductName,p.Code as ProductCode,si.Quantity,
(select top 1 SalePrice from productPrice sp
where p.Id= sp.ProductId and priceType =1 and startTime<=getdate() and endTime>=getdate() 
and (sp.storeId = 0 or sp.StoreId = si.StoreId) order by sp.Slevel desc,sp.id desc) SalePrice ,
(select top 1 SalePrice from productPrice sp
where p.Id= sp.ProductId and priceType =2 and startTime<=getdate() and endTime>=getdate() 
and (sp.storeId = 0 or sp.StoreId = si.StoreId) order by sp.Slevel desc,sp.id desc) MinSalePrice
 from StoreInventory si
left join product p on si.ProductId = p.Id where si.StoreId=@StoreId and p.Id =@ProductId";

            var entity = _db.Table<StoreProductPriceDto>().QuerySingle(sql, new { StoreId = storeId, ProductId = productId });
            if (entity == null)
                throw new FriendlyException("查无此商品价格：{0}".FormatWith(productId));

            return entity;
        }
    }
}