using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using System.Collections.Generic;

namespace Guoc.BigMall.Application
{
    public interface ICategoryRechargeVoucherFacade
    {
        List<CategoryRechargeVoucherDto> GetCategoryVoucherList(Pager page, CategoryRechargeVoucherSearch searchArgs);
        List<CategoryCard> GetStoreCardNumbers(int storeId);
        void Create(CreateCategoryVoucherModel model);
        void PassAudit(int id);
        void RejectAudit(int id);
        void Abort(int id);

        void ReduceCategoryRechargeVoucher(IDBContext dbContext, SaleOrder order);
        void RefundCategoryRechargeVoucher(IDBContext dbContext, SaleOrder order);
    }
}
