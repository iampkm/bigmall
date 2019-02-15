using System;
using System.Collections.Generic;
using System.Linq;
using Guoc.BigMall.Domain.Entity;
//using Guoc.BigMall.Domain.Service;
using Dapper.DBContext;
using Guoc.BigMall.Application;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Infrastructure.Extension;
using Guoc.BigMall.Infrastructure;
using Guoc.BigMall.Application.ViewObject;
using System.Dynamic;
using System.Text;
using Guoc.BigMall.Application.Search;
using Guoc.BigMall.Domain.ValueObject;

namespace Guoc.BigMall.Application.Facade
{
    public class ReportFacade : IReportFacade
    {
        IDBContext _db;

        public ReportFacade(IDBContext dbContext)
        {
            this._db = dbContext;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IEnumerable<PurchaseSaleInventorySummaryDto> GetPageList(Pager page, SearchPurchaseSaleInventorySummary condition)
        {
            string basicSql = @"
                WITH    t AS ( SELECT   h.StoreId ,
                                        h.ProductId ,
                                        h.Quantity ,
                                        h.ChangeQuantity ,
                                        h.Price ,
                                        h.RealPrice ,
                                        h.CreatedOn ,
                                        h.BillId ,
                                        h.BillCode ,
                                        h.BillType ,
                                        h.CategoryPreferential ,
                                        h.BrandPreferential ,
                                        p.Code ProductCode ,
                                        p.Name ProductName ,
                                        c.Code CategoryCode ,
                                        c.FullName CategoryName ,
                                        b.Code BrandCode ,
                                        b.Name BrandName
                               FROM     StoreInventoryHistory h
                                        LEFT JOIN Product p ON h.ProductId = p.Id
                                        LEFT JOIN Category c ON c.Id = p.CategoryId
                                        LEFT JOIN Brand b ON b.Id = p.BrandId
                                        LEFT JOIN Store s ON s.Id = h.StoreId
                               WHERE    1 = 1 {0}
                             )
                    SELECT  {1}
                    FROM    ( SELECT DISTINCT
                                        t.StoreId ,
                                        t.ProductId ,
                                        t.ProductCode ,
                                        t.ProductName ,
                                        t.CategoryCode ,
                                        t.CategoryName ,
                                        t.BrandCode ,
                                        t.BrandName
                              FROM      t
                            ) AS M
                            LEFT JOIN ( SELECT  firstTb.StoreId ,
                                                firstTb.ProductId ,
                                                firstTb.Quantity AS FirstQuantity
                                        FROM    ( SELECT    ROW_NUMBER() OVER ( PARTITION BY t.StoreId,
                                                                              t.ProductId ORDER BY t.CreatedOn ASC ) Num ,
                                                            *
                                                  FROM      t
                                                ) firstTb
                                        WHERE   firstTb.Num = 1
                                      ) ft ON M.StoreId = ft.StoreId
                                              AND M.ProductId = ft.ProductId
                            LEFT JOIN ( SELECT  lastTb.StoreId ,
                                                lastTb.ProductId ,
                                                lastTb.Quantity + lastTb.ChangeQuantity AS LastQuantity
                                        FROM    ( SELECT    ROW_NUMBER() OVER ( PARTITION BY t.StoreId,
                                                                              t.ProductId ORDER BY t.CreatedOn DESC ) Num ,
                                                            *
                                                  FROM      t
                                                ) lastTb
                                        WHERE   lastTb.Num = 1
                                      ) lt ON M.StoreId = lt.StoreId
                                              AND M.ProductId = lt.ProductId
                            LEFT JOIN ( SELECT  t.StoreId ,
                                                t.ProductId ,
								                --本期采购数量
                                                SUM(CASE WHEN t.BillType IN ( 20, 21 )
                                                         THEN t.ChangeQuantity
                                                         ELSE 0
                                                    END)
                                                - SUM(CASE WHEN t.BillType IN ( 23, 24 )
                                                           THEN -t.ChangeQuantity
                                                           ELSE 0
                                                      END) AS PurchaseQuantity ,--本期采购入库总数-本期采购退货出库总数

									                  --本期采购金额
                                                SUM(CASE WHEN t.BillType IN ( 20, 21 )
                                                         THEN t.ChangeQuantity * t.Price
                                                         ELSE 0
                                                    END)
                                                - SUM(CASE WHEN t.BillType IN ( 23, 24 )
                                                           THEN -t.ChangeQuantity * t.Price
                                                           ELSE 0
                                                      END) AS PurchaseAmount--本期采购入库总金额-本期采购退货出库总金额
                                        FROM    t
                                        WHERE   t.BillType IN ( 20, 21, 23, 24 )
                                        GROUP BY t.StoreId ,
                                                t.ProductId
                                      ) AS pt ON M.StoreId = pt.StoreId
                                                 AND M.ProductId = pt.ProductId
                            LEFT JOIN ( SELECT  t.StoreId ,
                                                t.ProductId ,

								                --本期销售数量
                                                SUM(CASE WHEN t.BillType IN ( 11, 13, 15 )
                                                              AND t.StoreId = s.StoreId
                                                         THEN ABS(t.ChangeQuantity)
                                                         ELSE 0
                                                    END)
                                                - SUM(CASE WHEN t.BillType IN ( 12, 14, 16 )
                                                                AND t.StoreId = s.StoreId
                                                           THEN t.ChangeQuantity
                                                           ELSE 0
                                                      END) AS SaleQuantity ,--本期销售出库总数-本期销售退货入库总数

								                --本期销售金额
                                                SUM(CASE WHEN t.BillType IN ( 11, 13, 15 )
                                                              AND t.StoreId = s.StoreId
                                                         THEN ABS(t.ChangeQuantity)
                                                              * t.RealPrice
                                                         ELSE 0
                                                    END)
                                                - SUM(CASE WHEN t.BillType IN ( 12, 14, 16 )
                                                                AND t.StoreId = s.StoreId
                                                           THEN t.ChangeQuantity * t.RealPrice
                                                           ELSE 0
                                                      END) AS SaleAmount ,--本期销售出库总金额-本期销售退货入库总金额

								                --本期销售金额（券后）
                                                SUM(CASE WHEN t.BillType IN ( 11, 13, 15 )
                                                              AND t.StoreId = s.StoreId
                                                         THEN ABS(t.ChangeQuantity)
                                                              * t.RealPrice
                                                              - t.CategoryPreferential
                                                              - t.BrandPreferential
                                                         ELSE 0
                                                    END)
                                                - SUM(CASE WHEN t.BillType IN ( 12, 14, 16 )
                                                                AND t.StoreId = s.StoreId
                                                           THEN t.ChangeQuantity * t.RealPrice
                                                                - t.CategoryPreferential
                                                                - t.BrandPreferential
                                                           ELSE 0
                                                      END) AS SaleAmountAfterPreferential ,--本期销售出库总金额(不含券)-本期销售退货入库总金额(不含券)

								                --本期赠送数量
                                                SUM(CASE WHEN t.BillType IN ( 11, 13, 15 )
                                                              AND t.StoreId = s.StoreIdGift
                                                         THEN ABS(t.ChangeQuantity)
                                                         ELSE 0
                                                    END)
                                                - SUM(CASE WHEN t.BillType IN ( 12, 14, 16 )
                                                                AND t.StoreId = s.StoreIdGift
                                                           THEN t.ChangeQuantity
                                                           ELSE 0
                                                      END) AS GiftsQuantity ,--本期赠送出库总数-本期赠送退货入库总数

								                --本期赠送金额
                                                SUM(CASE WHEN t.BillType IN ( 11, 13, 15 )
                                                              AND t.StoreId = s.StoreIdGift
                                                         THEN ABS(t.ChangeQuantity)
                                                              * t.RealPrice
                                                         ELSE 0
                                                    END)
                                                - SUM(CASE WHEN t.BillType IN ( 12, 14, 16 )
                                                                AND t.StoreId = s.StoreIdGift
                                                           THEN t.ChangeQuantity * t.RealPrice
                                                           ELSE 0
                                                      END) AS GiftsAmount--本期赠送出库总金额-本期赠送退货入库总金额
                                        FROM    t ,
                                                SaleOrder s
                                        WHERE   t.BillCode = s.Code
                                                AND ( t.BillType IN ( 11, 12, 13, 14, 16 )
                                                      OR ( t.BillType = 15
                                                           AND s.Status = 6
                                                         )
                                                    )
                                        GROUP BY t.StoreId ,
                                                t.ProductId
                                      ) AS st ON M.StoreId = st.StoreId
                                                 AND M.ProductId = st.ProductId
                    WHERE   1 = 1 {2}";

            var fields = @"
                            M.* ,
                            ft.FirstQuantity ,
                            lt.LastQuantity ,
                            pt.PurchaseQuantity ,
                            pt.PurchaseAmount ,
                            st.SaleQuantity ,
                            st.SaleAmount ,
                            st.SaleAmountAfterPreferential ,
                            st.GiftsQuantity ,
                            st.GiftsAmount ,
                            '{0}' StartTime ,
                            '{1}' EndTime";

            dynamic param = new ExpandoObject();
            string where = "";
            string filter = "";
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();
            if (!string.IsNullOrEmpty(condition.ProductCodeOrBarCode))
            {
                where += " and p.Id in  @ProductCodeOrBarCode";
                param.ProductCodeOrBarCode = condition.ProductCodeOrBarCode.Split(',').ToArray();

            }
            if (!string.IsNullOrEmpty(condition.ProductName))
            {
                where += " and p.Name like @ProductName ";
                param.ProductName = string.Format("%{0}%", condition.ProductName);
            }

            if (!string.IsNullOrEmpty(condition.BrandId))
            {
                where += " and b.Id in  @BrandId ";
                param.BrandId = condition.BrandId.Split(',').ToIntArray();
            }
            if (!string.IsNullOrEmpty(condition.CategoryId))
            {
                where += " and c.Id = @CategoryId ";
                param.CategoryId = condition.CategoryId;
            }

            if (!string.IsNullOrWhiteSpace(condition.Time))
            {
                var times = condition.Time.Split(',');
                StartDate = Convert.ToDateTime(times[0]);
                EndDate = Convert.ToDateTime(times[1]);

                where += " AND h.CreatedOn >= @StartDate AND h.CreatedOn < @EndDate ";
                param.StartDate = StartDate;
                param.EndDate = EndDate.AddDays(1);
            }
            fields = fields.FormatWith(StartDate, EndDate);

            //if (!string.IsNullOrWhiteSpace(condition.SupplierId))
            //{
            //    where += " and M.SupplierId = " + condition.SupplierId;
            //}
            if (condition.checkedQuantity)
            {
                filter += " and (ft.FirstQuantity>0 or lt.LastQuantity>0 OR pt.PurchaseQuantity>0 OR pt.PurchaseAmount >0 OR st.SaleQuantity>0 OR st.SaleAmount>0 OR st.SaleAmountAfterPreferential>0 OR st.GiftsQuantity>0 OR st.GiftsAmount>0) ";
            }
            if (!string.IsNullOrEmpty(condition.StoreId))
            {
                where += " and s.Id IN @StoreId ";
                param.StoreId = condition.StoreId.Split(',');
            }

            var dataSql = basicSql.FormatWith(where, fields, filter);

            if (!page.IsPaging)
            {
                return this._db.DataBase.Query<PurchaseSaleInventorySummaryDto>(dataSql, param);
            }

            var pageSql = dataSql + " ORDER BY M.ProductId DESC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY".FormatWith(page.PageSize * (page.PageIndex - 1), page.PageSize);

            var rows = this._db.DataBase.Query<PurchaseSaleInventorySummaryDto>(pageSql, param);

            var countSql = basicSql.FormatWith(where, "COUNT(1)", filter);

            page.Total = _db.DataBase.ExecuteScalar<int>(countSql, param);
            return rows;
        }


        public IEnumerable<SaleOrderItemSummaryDto> GetSaleOrderItemList(Pager page, SearchSaleOrderItemSummary condition)
        {
            dynamic param = new ExpandoObject();
            string where = "";

            if (!string.IsNullOrEmpty(condition.ProductCodeOrBarCode))
            {
                where += " and p.id in  @ProductCodeOrBarCode";
                param.ProductCodeOrBarCode = condition.ProductCodeOrBarCode.Split(',').ToArray();

            }
            if (!string.IsNullOrEmpty(condition.BrandId))
            {
                where += "and p.BrandId in  @BrandId ";
                param.BrandId = condition.BrandId.Split(',').ToIntArray();
            }
            if (!string.IsNullOrEmpty(condition.CategoryId))
            {
                where += "and p.CategoryId=@CategoryId ";
                param.CategoryId = condition.CategoryId;
            }
            if (!string.IsNullOrWhiteSpace(condition.Time))
            {
                var times = condition.Time.Split(',');
                var StartDate = Convert.ToDateTime(times[0]);
                var EndDate = Convert.ToDateTime(times[1]).AddDays(1);
                where += " and so.PaidDate >= '" + StartDate + "' and so.PaidDate < '" + EndDate + "'";
            }
            if (!string.IsNullOrWhiteSpace(condition.BillType))
            {
                where += " and so.billType =" + condition.BillType;
            }
            if (!string.IsNullOrWhiteSpace(condition.SaleOrderCode))
            {
                where += "and so.Code like @SaleOrderCode ";
                param.SaleOrderCode = string.Format("%{0}%", condition.SaleOrderCode);
            }
            if (!string.IsNullOrWhiteSpace(condition.SNCode))
            {
                where += "and soi.SNCode like @SNCode ";
                param.SNCode = string.Format("%{0}%", condition.SNCode);
            }
            if (!string.IsNullOrWhiteSpace(condition.CreateUser))
            {
                where += " and so.CreatedBy  =" + condition.CreateUser;
            }
            if (!string.IsNullOrEmpty(condition.StoreId))
            {
                where += "and s.Id IN @StoreId ";
                param.StoreId = condition.StoreId.Split(',');
            }
            string sql = @"SELECT 
                            so.Code SaleOrderCode,
		                    so.CreatedOn,
							soi.SaleOrderId,
                            soi.ProductId,
		                    p.Code ProductCode,
		                    p.Name ProductName,
		                    (CASE WHEN so.OrderType=2 THEN -soi.Quantity ELSE soi.Quantity END) AS Quantity,
		                    soi.RealPrice,
		                    '' tax,
		                    so.BillType,
		                    so.Buyer,
		                    so.StoreId,
		                    s.Name StoreName,
		                    so.AuditedBy,
		                    a2.UserName AuditeUser,
		                    so.AuditedOn,
		                    so.CreatedBy,
		                    a1.UserName CreateUser,
		                    so.Remark,soi.ParentProductId,GiftType,
		                    '' BusinessUser,
		                    soi.SNCode ,
                            STUFF(( SELECT  ';[' + t.Name + ' : '
                                            + CAST(( CASE WHEN so.OrderType = 2 THEN -m.Quantity
                                                          ELSE m.Quantity
                                                     END ) AS VARCHAR(15)) + ']'
                                    FROM    SaleOrderItem m ,
                                            Product t
                                    WHERE   m.ProductId = t.Id
                                            AND m.GiftType = 2
                                            AND m.SaleOrderId = so.Id
                                            AND m.ParentProductId = soi.ParentProductId
                                  FOR
                                    XML PATH('')
                                  ), 1, 1, '') AS GiftItem
                    FROM    dbo.SaleOrder so  
                           Left JOIN  dbo.SaleOrderItem soi ON  so.Id = soi.SaleOrderId
                           Left JOIN  dbo.Product p ON   soi.ProductId = p.Id
		                   Left JOIN   dbo.Store s  ON       so.StoreId = s.Id
                           Left JOIN  dbo.Account a1 ON      a1.Id = so.CreatedBy
		                   Left JOIN  dbo.Account a2 ON         a2.Id = so.AuditedBy
                    WHERE  ((so.BillType <> 3 AND (so.Status = 5 OR so.RoStatus = 5)) OR so.Status  = 6)
                            AND soi.GiftType = {0}
                            {1} 
                    ORDER BY so.Code DESC";

            var productSql = sql.FormatWith(OrderProductType.Product.Value(), where);
            if (page.IsPaging)
                productSql += " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY".FormatWith(page.PageSize * (page.PageIndex - 1), page.PageSize);

            var productItems = this._db.DataBase.Query<SaleOrderItemSummaryDto>(productSql, param) as List<SaleOrderItemSummaryDto>;

            //var parentProductIds = productItems.Select(m => m.ParentProductId).Distinct().ToArray();
            //if (parentProductIds.Length > 0)
            //{
            //    var saleOrderIds = productItems.Select(m => m.SaleOrderId).Distinct().ToArray();
            //    var giftWhere = " {0} AND soi.SaleOrderId IN @SaleOrderIds AND soi.ParentProductId IN @ParentProductIds".FormatWith(where);
            //    var giftSql = sql.FormatWith(OrderProductType.Gift.Value(), giftWhere);
            //    param.SaleOrderIds = saleOrderIds;
            //    param.ParentProductIds = parentProductIds;
            //    var giftItems = this._db.DataBase.Query<SaleOrderItemSummaryDto>(giftSql, param) as List<SaleOrderItemSummaryDto>;

            //    giftItems.ForEach(g =>
            //    {
            //        var productItem = productItems.First(m => m.SaleOrderId == g.SaleOrderId && m.ParentProductId == g.ParentProductId);
            //        productItem.GiftItem = (productItem.GiftItem.IsNullOrEmpty() ? "" : productItem.GiftItem + ";") + "[{0} : {1}]".FormatWith(g.ProductName, g.Quantity);
            //    });
            //}

            if (!page.IsPaging) return productItems;

            string sqlCount = @"SELECT Count(1)
                    FROM   dbo.SaleOrder so  
                           Left JOIN  dbo.SaleOrderItem soi ON  so.Id = soi.SaleOrderId
                           Left JOIN  dbo.Product p ON   soi.ProductId = p.Id
		                   Left JOIN   dbo.Store s  ON       so.StoreId = s.Id
                           Left JOIN  dbo.Account a1 ON      a1.Id = so.CreatedBy
		                   Left JOIN  dbo.Account a2 ON         a2.Id = so.AuditedBy
                    WHERE   ((so.BillType <> 3 AND (so.Status = 5 OR so.RoStatus = 5)) OR so.Status  = 6)
		                    AND soi.GiftType = {0}
                            {1}".FormatWith(OrderProductType.Product.Value(), where);

            page.Total = _db.DataBase.ExecuteScalar<int>(sqlCount, param);
            return productItems;
        }

        IEnumerable<SaleOrderItemSummaryDto> ConventoryData(IEnumerable<SaleOrderItemSummaryDto> iList)
        {
            //var data = iList.Where(n => n.ParentProductId == 0);
            var data = iList.Where(n => n.GiftType == OrderProductType.Product);

            foreach (var item in data)
            {
                var gifts = (from x in iList
                             where x.ParentProductId == item.ParentProductId && x.GiftType == OrderProductType.Gift
                             select "[" + x.ProductName + ":" + x.Quantity + "]").ToArray();

                item.GiftItem = string.Join(";", gifts);
            }
            return data;
        }
        public IEnumerable<SaleOrderSummaryDto> GetSaleOrderSummaryList(Pager page, SearchSaleOrderSummary condition)
        {
            string sql = @"SELECT 
                                p.Id ,
                                so.Code SaleOrderCode ,
                                so.CreatedOn ,
                                so.PaidDate ,
                                soi.ProductId ,
                                p.Code ProductCode ,
                                p.Name ProductName ,
                                p.CategoryId ,
                                p.BrandId ,
                                so.BillType,
                                --soi.Quantity ,
                                (CASE WHEN so.OrderType=2 THEN -soi.Quantity ELSE soi.Quantity END) AS Quantity ,
                                soi.RealPrice ,
                                so.Buyer ,
                                so.StoreId ,
                                so.CreatedBy ,
                                '' businessUser
                        FROM   dbo.SaleOrder so 
                                LEFT JOIN  dbo.SaleOrderItem soi  ON so.Id = soi.SaleOrderId
                                LEFT JOIN  dbo.Product p  ON    soi.ProductId = p.Id
                        WHERE   ((so.BillType <> 3 AND (so.Status = 5 OR so.RoStatus = 5)) OR so.Status  = 6) ";
            dynamic param = new ExpandoObject();
            string where = "";
            if (!string.IsNullOrEmpty(condition.ProductCodeOrBarCode))
            {
                where += " and M.id in  @ProductCodeOrBarCode";
                param.ProductCodeOrBarCode = condition.ProductCodeOrBarCode.Split(',').ToArray();

            }
            if (!string.IsNullOrEmpty(condition.BrandId))
            {
                where += "and M.BrandId in  @BrandId ";
                param.BrandId = condition.BrandId.Split(',').ToIntArray();
            }
            if (!string.IsNullOrEmpty(condition.CategoryId))
            {
                where += "and M.CategoryId=@CategoryId ";
                param.CategoryId = condition.CategoryId;
            }
            if (!string.IsNullOrWhiteSpace(condition.Time))
            {
                var times = condition.Time.Split(',');
                var StartDate = Convert.ToDateTime(times[0]);
                var EndDate = Convert.ToDateTime(times[1]).AddDays(1);
                where += " and M.PaidDate >= '" + StartDate + "' and M.PaidDate < '" + EndDate + "'";
            }
            if (!string.IsNullOrWhiteSpace(condition.BillType))
            {
                where += " and M.billType =" + condition.BillType;
            }

            if (!string.IsNullOrWhiteSpace(condition.CreateUser))
            {
                where += " and M.CreatedBy  =" + condition.CreateUser;
            }
            if (!string.IsNullOrWhiteSpace(condition.StoreId))
            {
                where += " and M.StoreId IN ({0})".FormatWith(condition.StoreId);
            }
            if (!string.IsNullOrWhiteSpace(condition.Buyer))
            {
                where += "and M.Buyer like @Buyer ";
                param.ProductName = string.Format("%{0}%", condition.Buyer);
            }

            string sqlData = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.SummaryMethod))
            {
                switch (condition.SummaryMethod)
                {
                    case "1"://商品汇总
                        sqlData = @"SELECT  M.ProductId ,
                                    M.ProductCode ,
                                    M.ProductName ,
                                    ROUND(SUM(M.RealPrice * M.Quantity)/(CASE WHEN SUM(M.Quantity)=0 THEN 1 ELSE SUM(M.Quantity) END), 2) RealPrice ,
                                    SUM(M.RealPrice * M.Quantity) Amount,
                                    SUM(M.Quantity) Quantity
                                    FROM (" + sql + ") M  where 1=1 {0} GROUP BY M.ProductId ,M.ProductCode ,M.ProductName  {1}  ORDER BY M.ProductCode DESC  ";
                        break;
                    case "2"://品牌汇总
                        sqlData = @"SELECT  M.BrandId,
                                            b.Name BrandName,        
                                            ROUND(SUM(M.RealPrice * M.Quantity)/(CASE WHEN SUM(M.Quantity)=0 THEN 1 ELSE SUM(M.Quantity) END), 2) RealPrice ,
                                            SUM(M.RealPrice * M.Quantity) Amount,
                                            SUM(M.Quantity) Quantity
                                          FROM (" + sql + ")  M  LEFT JOIN dbo.Brand B ON M.BrandId =B.Id where 1=1 {0}  GROUP BY M.BrandId,b.Name {1} ORDER BY M.BrandId DESC ";
                        break;
                    case "3"://品类汇总
                        sqlData = @"SELECT  M.CategoryId,c.Name CategoryName,       
                                            ROUND(SUM(M.RealPrice * M.Quantity)/(CASE WHEN SUM(M.Quantity)=0 THEN 1 ELSE SUM(M.Quantity) END), 2) RealPrice ,
                                            SUM(M.RealPrice * M.Quantity) Amount,
                                            SUM(M.Quantity) Quantity
                                          FROM (" + sql + ") M  LEFT JOIN dbo.Category c ON M.CategoryId =c.Id  where 1=1 {0}  GROUP BY M.CategoryId,c.Name {1} ORDER BY M.CategoryId DESC  ";
                        break;
                    case "4"://日期汇总
                        sqlData = @"SELECT  CONVERT(VARCHAR(10), m.PaidDate ,120 ) PaidDateStr,        
                                            ROUND(SUM(M.RealPrice * M.Quantity)/(CASE WHEN SUM(M.Quantity)=0 THEN 1 ELSE SUM(M.Quantity) END), 2) RealPrice ,
                                            SUM(M.RealPrice * M.Quantity) Amount,
                                            SUM(M.Quantity) Quantity
                                          FROM (" + sql + ") M where 1=1 {0}  GROUP BY   M.PaidDate  {1} ORDER BY  M.PaidDate DESC  ";
                        break;
                    case "5"://仓库汇总
                        sqlData = @"SELECT  M.StoreId,s.Name StoreName,         
                                                ROUND(SUM(M.RealPrice * M.Quantity)/(CASE WHEN SUM(M.Quantity)=0 THEN 1 ELSE SUM(M.Quantity) END), 2) RealPrice ,
                                                SUM(M.RealPrice * M.Quantity) Amount,
                                                SUM(M.Quantity) Quantity
                                          FROM (" + sql + ") M  LEFT JOIN dbo.Store s ON m.StoreId = s.Id where 1=1 {0}  GROUP BY  M.StoreId,s.Name {1} ORDER BY M.StoreId DESC ";
                        break;
                    case "6"://客户汇总
                        sqlData = @"SELECT  M.Buyer,         
                                        ROUND(SUM(M.RealPrice * M.Quantity)/(CASE WHEN SUM(M.Quantity)=0 THEN 1 ELSE SUM(M.Quantity) END), 2) RealPrice ,
                                        SUM(M.RealPrice * M.Quantity) Amount,
                                        SUM(M.Quantity) Quantity
                                          FROM (" + sql + ") M  where 1=1 {0}  GROUP BY  M.Buyer {1} ORDER BY M.Buyer DESC  ";
                        break;
                    case "7"://业务员
                        sqlData = @"";
                        break;
                    case "8"://單號
                        sqlData = @"SELECT  M.SaleOrderCode,         
                                          ROUND(SUM(M.RealPrice * M.Quantity)/(CASE WHEN SUM(M.Quantity)=0 THEN 1 ELSE SUM(M.Quantity) END), 2) RealPrice ,
                                            SUM(M.RealPrice * M.Quantity) Amount,
                                            SUM(M.Quantity) Quantity
                                          FROM (" + sql + ") M  where 1=1 {0}  GROUP BY  M.SaleOrderCode {1} ORDER BY M.SaleOrderCode DESC  ";
                        break;
                }
            }



            string havingStr = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.PriceMethod))
            {
                switch (condition.PriceMethod)
                {
                    case "1":
                        havingStr = " HAVING   SUM(M.RealPrice * M.Quantity) < 500";
                        break;
                    case "2":
                        havingStr = " HAVING   SUM(M.RealPrice * M.Quantity) BETWEEN 500 AND 999";
                        break;
                    case "3":
                        havingStr = " HAVING   SUM(M.RealPrice * M.Quantity) BETWEEN 1000 AND 1999 ";
                        break;
                    case "4":
                        havingStr = " HAVING   SUM(M.RealPrice * M.Quantity)  BETWEEN 2000 AND 2999 ";
                        break;
                    case "5":
                        sqlData = " HAVING   SUM(M.RealPrice * M.Quantity) BETWEEN 3000 AND 3999";
                        break;
                    case "6":
                        havingStr = " HAVING   SUM(M.RealPrice * M.Quantity) BETWEEN 4000 AND 4999  ";
                        break;
                    case "7":
                        havingStr = " HAVING   SUM(M.RealPrice * M.Quantity) BETWEEN 5000 AND 6499  ";
                        break;
                    case "8":
                        havingStr = " HAVING   SUM(M.RealPrice * M.Quantity) >=6500";
                        break;
                }
            }
            sqlData = sqlData.FormatWith(where, havingStr);

            if (page.IsPaging)
            {
                sqlData += " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY".FormatWith(page.PageIndex * (page.PageIndex - 1), page.PageIndex * page.PageSize);
            }
            var rows = this._db.DataBase.Query<SaleOrderSummaryDto>(sqlData, param);

            if (page.IsPaging)
            {
                string sqlCount = @"select count(1) from (" + sqlData + ") M1";
                page.Total = _db.DataBase.ExecuteScalar<int>(sqlCount, param);
            }
            return rows;

        }


        public IEnumerable<OrderProfitDto> GetOrderProfitList(Pager page, SearchProfit condition)
        {
            dynamic param = new ExpandoObject();
            string where = "";

            if (!string.IsNullOrEmpty(condition.ProductCodeOrBarCode))
            {
                where += " and p.id in  @ProductCodeOrBarCode";
                param.ProductCodeOrBarCode = condition.ProductCodeOrBarCode.Split(',').ToArray();

            }
            if (!string.IsNullOrEmpty(condition.BrandId))
            {
                where += " and p.BrandId in  @BrandId ";
                param.BrandId = condition.BrandId.Split(',').ToIntArray();
            }
            if (!string.IsNullOrEmpty(condition.CategoryId))
            {
                where += " and p.CategoryId=@CategoryId ";
                param.CategoryId = condition.CategoryId;
            }
            if (!string.IsNullOrWhiteSpace(condition.Time))
            {
                var times = condition.Time.Split(',');
                var StartDate = Convert.ToDateTime(times[0]);
                var EndDate = Convert.ToDateTime(times[1]).AddDays(1);
                where += " and so.PaidDate >= '" + StartDate + "' and so.PaidDate < '" + EndDate + "'";
            }
            if (!string.IsNullOrWhiteSpace(condition.BillType))
            {
                where += " and sh.billType =" + condition.BillType;
            }

            if (!string.IsNullOrWhiteSpace(condition.Buyer))
            {
                where += " and so.buyer =@Buyer";
                param.Buyer = condition.Buyer;
            }
            if (!string.IsNullOrEmpty(condition.StoreId))
            {
                where += " and s.Id IN @StoreId ";
                param.StoreId = condition.StoreId.Split(',');
            }
            if (!string.IsNullOrWhiteSpace(condition.SupplierId))
            {
                where += " and sp.Id = " + condition.SupplierId;
            }
            string sql = @"SELECT  so.Code AS saleOrderCode ,
                                so.CreatedOn ,
                                so.PaidDate ,
                                s.Name AS storeName ,
                                b.Name AS brandName ,
                                p.Code AS productCode ,
                                p.Name AS productName ,
                                sh.SNCode ,
                                sh.Price AS costPrice ,
                                soi.RealPrice ,
                                sh.BillType ,
								--(CASE WHEN so.OrderType=2 THEN -soi.Quantity ELSE soi.Quantity END) AS Quantity,
								sh.ChangeQuantity AS Quantity,
                                c.Name AS categoryName ,
                                sp.Name AS supplierName,so.Buyer
                        FROM    dbo.SaleOrder so
                                INNER JOIN dbo.SaleOrderItem soi ON so.Id = soi.SaleOrderId
                                INNER JOIN dbo.StoreInventoryHistory sh ON sh.BillCode = so.Code AND sh.ProductId = soi.ProductId
                                INNER JOIN dbo.Store s ON s.Id = so.StoreId
                                INNER JOIN dbo.Supplier sp ON sp.Id = sh.SupplierId
                                INNER JOIN dbo.Product p ON p.Id = soi.ProductId
                                INNER JOIN dbo.Category c ON c.Id = p.CategoryId
                                INNER JOIN dbo.Brand b ON b.Id = p.BrandId  
                                WHERE 1=1    {0}    
                                ORDER BY so.PaidDate DESC ".FormatWith(where);
            if (page.IsPaging)
            {
                sql += " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY";
                sql = string.Format(sql, page.PageIndex * (page.PageIndex - 1), page.PageIndex * page.PageSize);
            }

            var rows = this._db.DataBase.Query<OrderProfitDto>(sql, param);

            string sqlCount = @"SELECT Count(1)
                    FROM   dbo.SaleOrder so
                                INNER JOIN dbo.SaleOrderItem soi ON so.Id = soi.SaleOrderId
                                INNER JOIN dbo.StoreInventoryHistory sh ON sh.BillCode = so.Code AND sh.ProductId = soi.ProductId
                                INNER JOIN dbo.Store s ON s.Id = so.StoreId
                                INNER JOIN dbo.Supplier sp ON sp.Id = sh.SupplierId
                                INNER JOIN dbo.Product p ON p.Id = soi.ProductId
                                INNER JOIN dbo.Category c ON c.Id = p.CategoryId
                                INNER JOIN dbo.Brand b ON b.Id = p.BrandId
                    WHERE  1=1  {0}".FormatWith(where);

            page.Total = _db.DataBase.ExecuteScalar<int>(sqlCount, param);
            return rows;
        }
    }
}
