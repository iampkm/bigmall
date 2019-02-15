using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using Dapper.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface IBrandRechargeVoucherFacade
    {
        List<BrandRechargeVoucherDto> GetBrandVoucherList(Pager page, BrandRechargeVoucherSearch searchArgs);
        void Create(CreateBrandVoucherModel model);
        void PassAudit(int id);
        void RejectAudit(int id);
        void Abort(int id);

        void ReduceBrandRechargeVoucher(IDBContext dbContext, SaleOrder order);
        void RefundBrandRechargeVoucher(IDBContext dbContext, SaleOrder order);
    }
}
