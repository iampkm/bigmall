using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application
{
    public interface ITransferOrderFacade
    {
        List<TransferOrderDto> GetPageList(Pager page, TransferOrderSearch condition);
        List<TransferOrderDetailDto> GetTransferOrderDetailList(Pager page, TransferOrderSearch condition);
        TransferOrderDto GetTransferOrderById(int id);
        void Create(TransferCreateModel model);
        void SubmitToSap(TransferOrder transferOrder);
        void Edit(TransferEditModel model);
        void ApplyAudit(int transferOrderId);
        void PassAudit(int transferOrderId, string auditRemark);
        void RejectAudit(int transferOrderId, string auditRemark);
        void OutStock(TransferStockOutModel model);
        void SapOutStock(TransferOrder transferOrder, string historyCode, DateTime stockOutDate, AccountInfo currentAccount);
        void InStock(TransferStockInModel model);
        void SapInStock(TransferOrder transferOrder, string historyCode, DateTime stockOutDate, AccountInfo currentAccount);

        List<TransferOrderPrintDto> GetPrintList(string ids);
    }
}
