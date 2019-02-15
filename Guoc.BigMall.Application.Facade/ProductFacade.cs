using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using Guoc.BigMall.Infrastructure.Extension;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Domain.ValueObject;
using Guoc.BigMall.Infrastructure;

namespace Guoc.BigMall.Application.Facade
{
    public class ProductFacade : IProductFacade
    {
        IDBContext _db;
        public ProductFacade(IDBContext dbContext)
        {
            this._db = dbContext;

        }

        public IEnumerable<ProductDto> LoadSupplierProduct(ViewObject.Pager page, SearchProduct search)
        {

            IEnumerable<ProductDto> rows;
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrWhiteSpace(search.Name))
            {
                where += " and p.name  like @name";
                param.name = string.Format("%{0}%", search.Name.Trim());
            }
            if (!string.IsNullOrWhiteSpace(search.Code))
            {
                where += " and p.code like @code";
                param.code = string.Format("%{0}%", search.Code.Trim());

            }
            if (!string.IsNullOrEmpty(search.BrandIds))
            {
                where += " and b.id in @brandId";
                param.brandId = search.BrandIds.Split(',').ToArray();
            }
            if (!string.IsNullOrWhiteSpace(search.CategoryId))
            {
                where += " and c.id =@categoryCode";
                param.categoryCode = Convert.ToInt32(search.CategoryId);
            }
            if (!string.IsNullOrWhiteSpace(search.Ids) && search.Ids != "0")
            {
                where += " AND p.Id IN @Ids";
                param.Ids = search.Ids.Split(',').ToIntArray();
            }
            if (!string.IsNullOrWhiteSpace(search.StoreId))
            {
                where += "  and e.id=@StoreId";
                param.StoreId = Convert.ToInt32(search.StoreId);
            }
            if (!string.IsNullOrWhiteSpace(search.SupplierId))
            {
                where += "  and s.id=@SupplierId";
                param.SupplierId = Convert.ToInt32(search.SupplierId);
            }
            StringBuilder str = new StringBuilder();
            str.Append(" select * from (");
            str.AppendFormat(@" SELECT  ROW_NUMBER() OVER ( ORDER BY p.Id ) rows ,
                                        p.Id ,
                                        p.Name ,
                                        p.Code ,
                                        p.Spec AS Specification ,
                                        p.Unit ,
                                        p.HasSNCode ,
                                        i.StoreId ,
                                        e.Code AS StoreCode ,
                                        e.Name AS StoreName ,
                                        t.InventoryQuantity AS InventoryQuantity ,
                                        t.SupplierId ,
                                        s.Name AS SupplierName ,
                                        s.Code AS SupplierCode ,
                                        b.Name AS BrandName ,
                                        c.Name AS CategoryName ,
                                        ( SELECT TOP 1
                                                    PP.PurchasePrice
                                          FROM      ProductPurchasePrice PP
                                          WHERE     PP.ProductId = i.ProductId
                                                    AND PP.SupplierId = t.SupplierId
                                                    AND PP.Status = 1
                                                    AND PP.StartTime <= GETDATE()
                                                    AND PP.EndTime >= GETDATE()
                                          ORDER BY  PP.Id DESC
                                        ) AS CostPrice
                                FROM    Product p
                                        INNER JOIN StoreInventory i ON p.Id = i.ProductId
                                        INNER JOIN Category c ON c.Id = p.CategoryId
                                        INNER JOIN Brand b ON b.Id = p.BrandId
                                        INNER JOIN ( SELECT b.StoreId ,
                                                            b.SupplierId ,
                                                            b.ProductId ,
                                                            SUM(b.Quantity) AS InventoryQuantity
                                                     FROM   StoreInventoryBatch b
                                                     GROUP BY b.StoreId ,
                                                            b.SupplierId ,
                                                            b.ProductId
                                                   ) t ON t.StoreId = i.StoreId
                                                          AND t.ProductId = i.ProductId
                                        INNER JOIN Supplier s ON t.SupplierId = s.Id
                                        INNER JOIN Store e ON e.Id = i.StoreId
                                WHERE   1 = 1 {0}", where);
            str.AppendFormat(" ) as t where t.rows between {0} and {1}", (page.PageIndex - 1) * page.PageSize + 1, page.PageIndex * page.PageSize);
            rows = _db.DataBase.Query<ProductDto>(str.ToString(), param);
            string sqlcount = string.Format(@"SELECT  COUNT(1)
                                                FROM    StoreInventory i
                                                        INNER JOIN Product p ON p.Id = i.ProductId
                                                        INNER JOIN dbo.Category c ON c.Id = p.CategoryId
                                                        INNER JOIN dbo.Brand b ON b.Id = p.BrandId
                                                        INNER JOIN ( SELECT b.StoreId ,
                                                                            b.SupplierId ,
                                                                            b.ProductId
                                                                     FROM   StoreInventoryBatch b
                                                                     GROUP BY b.StoreId ,
                                                                            b.SupplierId ,
                                                                            b.ProductId
                                                                   ) t ON t.StoreId = i.StoreId
                                                                          AND t.ProductId = i.ProductId
                                                        INNER JOIN Supplier s ON t.SupplierId = s.Id
                                                        INNER JOIN Store e ON e.Id = i.StoreId
                                                WHERE   1 = 1 {0}", where);
            page.Total = _db.DataBase.ExecuteScalar<int>(sqlcount.ToString(), param);
            return rows;
        }

        public IEnumerable<ProductDto> LoadStoreProduct(Pager page, SearchProduct search)
        {
            IEnumerable<ProductDto> rows;
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrWhiteSpace(search.Name))
            {
                where += " and p.name  like @name";
                param.name = string.Format("%{0}%", search.Name.Trim());
            }
            if (!string.IsNullOrWhiteSpace(search.Code))
            {
                where += " and p.code like @code";
                param.code = string.Format("%{0}%", search.Code.Trim());

            }
            if (!string.IsNullOrEmpty(search.BrandIds))
            {
                where += " and b.id in @brandId";
                param.brandId = search.BrandIds.Split(',').ToArray();
            }

            var categorySql = "";
            var categoryTable = "Category";
            if (!string.IsNullOrWhiteSpace(search.CategoryId))
            {
                //where += " and c.id in @CategoryIds";

                //传递父级品类时，查询所有子级品类
                categorySql = @"
                 WITH   categoryA
                          AS ( SELECT   *
                               FROM     Category
                               WHERE    Category.Id IN @CategoryIds
                               UNION ALL
                               SELECT   c.*
                               FROM     Category c ,
                                        categoryA
                               WHERE    c.ParentCode = categoryA.Code
                             )";

                categoryTable = "categoryA";
                param.CategoryIds = search.CategoryId.Split(',').ToIntArray();
            }

            if (!string.IsNullOrWhiteSpace(search.Ids))
            {
                where += " AND p.Id IN @Ids";
                param.Ids = search.Ids.Split(',').ToIntArray();
            }
            if (!string.IsNullOrWhiteSpace(search.StoreId))
            {
                where += "  and e.id=@StoreId";
                param.StoreId = Convert.ToInt32(search.StoreId);
            }

            if (search.ContainsNoStock == false)
            {
                where += " and i.Quantity >0";
            }

            StringBuilder dataSql = new StringBuilder();
            dataSql.AppendLine(categorySql);
            dataSql.AppendFormat(@"SELECT  p.Id ,
                                        p.Name ,
                                        p.Code ,
                                        p.Spec AS Specification ,
                                        p.Unit ,
                                        p.HasSNCode ,
                                        i.StoreId ,
                                        e.Code AS StoreCode ,
                                        e.Name AS StoreName ,
                                        i.Quantity AS InventoryQuantity ,
                                        b.Name AS BrandName ,
                                        c.Code AS CategoryCode ,
                                        c.Name AS CategoryName ,
                                        ( SELECT TOP 1
                                                    PP.PurchasePrice
                                          FROM      ProductPurchasePrice PP
                                          WHERE     PP.ProductId = i.ProductId
                                                    AND PP.Status = 1
                                                    AND PP.StartTime <= GETDATE()
                                                    AND PP.EndTime >= GETDATE()
                                          ORDER BY  PP.Id DESC
                                        ) AS CostPrice
                                FROM    Product p
                                        INNER JOIN StoreInventory i ON p.Id = i.ProductId
                                        INNER JOIN {0} c ON c.Id = p.CategoryId
                                        INNER JOIN Brand b ON b.Id = p.BrandId
                                        INNER JOIN Store e ON e.Id = i.StoreId
                                WHERE   1 = 1 {1}", categoryTable, where);

            if (page.IsPaging == false)
            {
                return _db.DataBase.Query<ProductDto>(dataSql.ToString(), param);
            }

            var pageSql = "{0} ORDER BY p.Id ASC OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY".FormatWith(dataSql, (page.PageIndex - 1) * page.PageSize, page.PageSize);
            rows = _db.DataBase.Query<ProductDto>(pageSql, param);

            var countSql = @"{0} 
                            SELECT  COUNT(1)
                            FROM    Product p
                                    INNER JOIN StoreInventory i ON p.Id = i.ProductId
                                    INNER JOIN {1} c ON c.Id = p.CategoryId
                                    INNER JOIN Brand b ON b.Id = p.BrandId
                                    INNER JOIN Store e ON e.Id = i.StoreId
                            WHERE   1 = 1 {2}".FormatWith(categorySql, categoryTable, where);
            page.Total = _db.DataBase.ExecuteScalar<int>(countSql, param);
            return rows;
        }

        public IEnumerable<ProductDto> LoadProduct(SearchProduct search)
        {
            IEnumerable<ProductDto> rows;
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrWhiteSpace(search.Name))
            {
                where += " and p.name  like @name";
                param.name = string.Format("%{0}%", search.Name.Trim());
            }
            if (!string.IsNullOrWhiteSpace(search.Code))
            {
                if (search.Code.Contains(","))
                {
                    where += " and p.code In @codes";
                    param.codes = search.Code.Trim().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    where += " and p.code like @code";
                    param.code = string.Format("%{0}%", search.Code.Trim());
                }
            }
            if (!string.IsNullOrEmpty(search.BrandIds))
            {
                where += " and b.id in @brandId";
                param.brandId = search.BrandIds.Split(',').ToArray();
            }

            var categorySql = "";
            var categoryTable = "Category";
            if (!string.IsNullOrWhiteSpace(search.CategoryId))
            {
                //where += " and c.id in @CategoryIds";

                //传递父级品类时，查询所有子级品类
                categorySql = @"
                 WITH   categoryA
                          AS ( SELECT   *
                               FROM     Category
                               WHERE    Category.Id IN @CategoryIds
                               UNION ALL
                               SELECT   c.*
                               FROM     Category c ,
                                        categoryA
                               WHERE    c.ParentCode = categoryA.Code
                             )";

                categoryTable = "categoryA";
                param.CategoryIds = search.CategoryId.Split(',').ToIntArray();
            }

            if (!string.IsNullOrWhiteSpace(search.Ids))
            {
                where += " AND p.Id IN @Ids";
                param.Ids = search.Ids.Split(',').ToIntArray();
            }

            if (!string.IsNullOrWhiteSpace(search.StoreId))
            {
                where += "  and e.id=@StoreId";
                param.StoreId = Convert.ToInt32(search.StoreId);
            }

            StringBuilder str = new StringBuilder();
            str.AppendLine(categorySql);
            str.AppendFormat(@" SELECT  p.Id ,
                                        p.Name ,
                                        p.Code ,
                                        p.Spec AS Specification ,
                                        p.Unit ,
                                        i.StoreId ,
                                        e.Code AS StoreCode ,
                                        e.Name AS StoreName ,
                                        i.Quantity AS InventoryQuantity ,
                                        b.Name AS BrandName ,
                                        c.Code AS CategoryCode ,
                                        c.Name AS CategoryName ,
                                        ( SELECT TOP 1
                                                    PP.PurchasePrice
                                          FROM      ProductPurchasePrice PP
                                          WHERE     PP.ProductId = i.ProductId
                                                    AND PP.Status = 1
                                                    AND PP.StartTime <= GETDATE()
                                                    AND PP.EndTime >= GETDATE()
                                          ORDER BY  PP.Id DESC
                                        ) AS CostPrice
                                FROM    Product p
                                        INNER JOIN StoreInventory i ON p.Id = i.ProductId
                                        INNER JOIN {0} c ON c.Id = p.CategoryId
                                        INNER JOIN dbo.Brand b ON b.Id = p.BrandId
                                        INNER JOIN Store e ON e.Id = i.StoreId
                                WHERE   1 = 1 {1}", categoryTable, where);
            rows = _db.DataBase.Query<ProductDto>(str.ToString(), param);
            return rows;
        }

        public IEnumerable<ProductDto> GetList(Pager page, SearchProduct search)
        {
            IEnumerable<ProductDto> rows;
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrWhiteSpace(search.Name))
            {
                where += " and p.name  like @name";
                param.name = string.Format("%{0}%", search.Name.Trim());
            }
            if (!string.IsNullOrWhiteSpace(search.Code))
            {
                where += " and p.code like @code";
                param.code = string.Format("%{0}%", search.Code.Trim());

            }
            if (!string.IsNullOrEmpty(search.BrandIds))
            {
                where += " and b.id in @brandId";
                param.brandId = search.BrandIds.Split(',').ToArray();
            }
            if (!string.IsNullOrWhiteSpace(search.CategoryId))
            {
                where += " and c.id =@categoryCode";
                param.categoryCode = Convert.ToInt32(search.CategoryId);
            }
            if (!string.IsNullOrWhiteSpace(search.Ids))
            {
                where += " AND p.Id IN @Ids";
                param.Ids = search.Ids.Split(',').ToIntArray();
            }

            StringBuilder str = new StringBuilder();
            str.Append(" select * from (");
            str.AppendFormat(@" SELECT p.* ,  b.Name  as BrandName,    c.Name as CategoryName, ROW_NUMBER() OVER(ORDER BY p.id ) rows
                                 FROM   dbo.Product p
                                        INNER JOIN dbo.Category c ON c.Id = p.CategoryId
                                        INNER JOIN dbo.Brand b ON b.Id = p.BrandId
                                WHERE 1=1 {0}", where);
            str.AppendFormat(" ) as t where t.rows between {0} and {1}", (page.PageIndex - 1) * page.PageSize + 1, page.PageIndex * page.PageSize);
            rows = _db.DataBase.Query<ProductDto>(str.ToString(), param);
            string sqlcount = string.Format(@" SELECT count(1)
                                             FROM   dbo.Product p
                                                    INNER JOIN dbo.Category c ON c.Id = p.CategoryId
                                                    INNER JOIN dbo.Brand b ON b.Id = p.BrandId
                                            WHERE 1=1 {0}", where);
            page.Total = _db.DataBase.ExecuteScalar<int>(sqlcount.ToString(), param);
            return rows;
        }

        public IEnumerable<ProductDto> GetSNcodeList(Pager page, SearchProduct search)
        {
            IEnumerable<ProductDto> rows;
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrWhiteSpace(search.Name))
            {
                where += " and p.name  like @name";
                param.name = string.Format("%{0}%", search.Name.Trim());
            }
            if (!string.IsNullOrWhiteSpace(search.Code))
            {
                where += " and p.code like @code";
                param.code = string.Format("%{0}%", search.Code.Trim());

            }
            if (!string.IsNullOrEmpty(search.BrandIds))
            {
                where += " and b.id in @brandId";
                param.brandId = search.BrandIds.Split(',').ToArray();
            }
            if (!string.IsNullOrWhiteSpace(search.CategoryId))
            {
                where += " and c.id =@categoryCode";
                param.categoryCode = Convert.ToInt32(search.CategoryId);
            }
            if (!string.IsNullOrWhiteSpace(search.Ids))
            {
                where += " AND p.Id IN @Ids";
                param.Ids = search.Ids.Split(',').ToIntArray();
            }
            if (!string.IsNullOrWhiteSpace(search.StoreId))
            {
                where += "  and e.id=@StoreId";
                param.StoreId = Convert.ToInt32(search.StoreId);
            }
            //根据SNcode精确查询
            if (!string.IsNullOrWhiteSpace(search.SNCodes))
            {
                where += " and i.SNCode  IN @SNCodes";
                param.SNCodes = search.SNCodes.Split(',');
            }
            //根据SNcode模糊查询
            else if (!string.IsNullOrWhiteSpace(search.SNCodeLike))
            {
                where += " and i.SNCode  like @SNCode";
                param.SNCode = string.Format("%{0}%", search.SNCodeLike);
            }

            if (search.ContainsNoStock == false)
            {
                where += " and i.Quantity >0";
            }

            var dataSql = @"
                SELECT DISTINCT
                        p.Id ,
                        p.Name ,
                        p.Code ,
                        p.Spec AS Specification ,
                        p.Unit ,
                        p.HasSNCode ,
                        i.StoreId ,
                        i.SNCode ,
                        e.Code AS StoreCode ,
                        e.Name AS StoreName ,
                        i.Price AS BatchCostPrice ,
                        ( CASE WHEN p.HasSNCode = 1 THEN i.Quantity
                               ELSE t.Quantity
                          END ) AS InventoryQuantity ,
                        p.BrandId ,
                        b.Name AS BrandName ,
                        p.CategoryId ,
                        c.Code AS CategoryCode ,
                        c.FullName AS CategoryName ,
                        ( SELECT TOP 1
                                    SalePrice
                          FROM      ProductPrice sp
                          WHERE     p.Id = sp.ProductId
                                    AND sp.PriceType = 1
                                    AND sp.Status = 1
                                    AND sp.StartTime <= GETDATE()
                                    AND sp.EndTime >= GETDATE()
                                    AND sp.SalePrice > 0
                                    AND ( sp.StoreId =0 
                                          OR sp.StoreId = t.StoreId
                                        )
                          ORDER BY  sp.Slevel DESC ,
                                    sp.Id DESC
                        ) SalePrice
                FROM    StoreInventoryBatch i
                        INNER JOIN StoreInventory t ON i.StoreId = t.StoreId
                                                       AND i.ProductId = t.ProductId
                        INNER JOIN Product p ON p.Id = i.ProductId
                        INNER JOIN dbo.Category c ON c.Id = p.CategoryId
                        INNER JOIN dbo.Brand b ON b.Id = p.BrandId
                        INNER JOIN Store e ON e.Id = i.StoreId
                WHERE   1 = 1 {0}".FormatWith(where);   // priceType =1 ==> (int)ProductPriceType.SalePrice

            dataSql = " SELECT * FROM ({0}) AS tmp ".FormatWith(dataSql);
            if (search.HasSalePrice == true)
            {
                dataSql += " WHERE tmp.SalePrice > 0 ";
            }

            if (page.IsPaging == false)
            {
                return _db.DataBase.Query<ProductDto>(dataSql, param);
            }

            var pageSql = "{0} ORDER BY tmp.Id ASC OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY".FormatWith(dataSql, (page.PageIndex - 1) * page.PageSize, page.PageSize);
            rows = _db.DataBase.Query<ProductDto>(pageSql, param);

            var countSql = " SELECT COUNT(1) FROM ({0}) as temp".FormatWith(dataSql);
            page.Total = _db.DataBase.ExecuteScalar<int>(countSql, param);
            return rows;
        }

        public ProductDto LoadProductById(int storeId, int productId)
        {
            var sql = @"SELECT p.*
                        FROM Product p
                        INNER JOIN StoreInventory si ON p.Id = si.ProductId
                        WHERE si.StoreId = @StoreId
                          AND p.Id = @ProductId";

            var products = _db.DataBase.Query<ProductDto>(sql, new { StoreId = storeId, ProductId = productId });

            if (products.Any() == false)
                throw new FriendlyException("存在无效商品:{0}".FormatWith(productId));

            return products.FirstOrDefault();
        }


        public ProductDto QueryProduct(int storeId, string snCode)
        {
            var sql = @"select p.*,b.Quantity from storeinventoryBatch b
left join Product p on b.ProductId = p.Id
where b.SNCode = @SNCode and b.StoreId = @StoreId";

            var product = _db.DataBase.QuerySingle<ProductDto>(sql, new { StoreId = storeId, SNCode = snCode });

            if (product==null)
                throw new FriendlyException("商品:{0} 不存在".FormatWith(snCode)); 
            return product;
        }
    }
}
