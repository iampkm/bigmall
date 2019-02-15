using Guoc.BigMall.Application;
using Guoc.BigMall.Application.DTO;
using Guoc.BigMall.Application.ViewObject;
using Guoc.BigMall.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Guoc.BigMall.Admin.Controllers
{
    public class CardVoucherController : Controller
    {
        IContextFacade _context;
        ICategoryRechargeVoucherFacade _categoryRechargeVoucherFacade;
        IBrandRechargeVoucherFacade _brandRechargeVoucherFacade;
        public CardVoucherController(IContextFacade context, ICategoryRechargeVoucherFacade categoryRechargeVoucherFacade, IBrandRechargeVoucherFacade brandRechargeVoucherFacade)
        {
            this._context = context;
            this._categoryRechargeVoucherFacade = categoryRechargeVoucherFacade;
            this._brandRechargeVoucherFacade = brandRechargeVoucherFacade;
        }

        #region 品类充值券

        public ActionResult CategoryVoucherIndex()
        {
            return View();
        }

        public JsonResult LoadCategoryVoucherList(Pager page, CategoryRechargeVoucherSearch searchArgs)
        {
            var rows = _categoryRechargeVoucherFacade.GetCategoryVoucherList(page, searchArgs);
            return Json(new { success = true, data = rows, total = page.Total });
        }

        public ActionResult CreateCategoryVoucherIndex()
        {
            return View();
        }

        public JsonResult LoadCreateCategoryVoucherList(Pager page, CategoryRechargeVoucherSearch searchArgs)
        {
            searchArgs.Status = RechargeVoucherStatus.WaitAudit;
            var rows = _categoryRechargeVoucherFacade.GetCategoryVoucherList(page, searchArgs);
            return Json(new { success = true, data = rows, total = page.Total });
        }

        public ActionResult CreateCategoryVoucher()
        {
            return View();
        }

        public JsonResult LoadStoreCardNumbers(int storeId)
        {
            var cards = _categoryRechargeVoucherFacade.GetStoreCardNumbers(storeId);
            return Json(new { success = true, data = cards });
        }

        [HttpPost]
        public JsonResult CreateCategoryVoucher(CreateCategoryVoucherModel model)
        {
            _categoryRechargeVoucherFacade.Create(model);
            return Json(new { success = true });
        }

        public ActionResult AuditCategoryVoucherIndex()
        {
            return View();
        }

        public JsonResult LoadCategoryVoucherWaitAuditList(Pager page, CategoryRechargeVoucherSearch searchArgs)
        {
            searchArgs.Status = RechargeVoucherStatus.WaitAudit;
            var rows = _categoryRechargeVoucherFacade.GetCategoryVoucherList(page, searchArgs);
            return Json(new { success = true, data = rows, total = page.Total });
        }

        public JsonResult PassAuditCategoryVoucher(int id)
        {
            _categoryRechargeVoucherFacade.PassAudit(id);
            return Json(new { success = true });
        }

        public JsonResult RejectAuditCategoryVoucher(int id)
        {
            _categoryRechargeVoucherFacade.RejectAudit(id);
            return Json(new { success = true });
        }

        public JsonResult AbortCategoryVoucher(int id)
        {
            _categoryRechargeVoucherFacade.Abort(id);
            return Json(new { success = true });
        }

        #endregion

        #region 品牌充值券

        public ActionResult BrandVoucherIndex()
        {
            return View();
        }

        public JsonResult LoadBrandVoucherList(Pager page, BrandRechargeVoucherSearch searchArgs)
        {
            var rows = _brandRechargeVoucherFacade.GetBrandVoucherList(page, searchArgs);
            return Json(new { success = true, data = rows, total = page.Total });
        }

        public ActionResult CreateBrandVoucherIndex()
        {
            return View();
        }

        public JsonResult LoadCreateBrandVoucherList(Pager page, BrandRechargeVoucherSearch searchArgs)
        {
            searchArgs.Status = RechargeVoucherStatus.WaitAudit;
            var rows = _brandRechargeVoucherFacade.GetBrandVoucherList(page, searchArgs);
            return Json(new { success = true, data = rows, total = page.Total });
        }

        public ActionResult CreateBrandVoucher()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateBrandVoucher(CreateBrandVoucherModel model)
        {
            _brandRechargeVoucherFacade.Create(model);
            return Json(new { success = true });
        }

        public ActionResult AuditBrandVoucherIndex()
        {
            return View();
        }

        public JsonResult LoadBrandVoucherWaitAuditList(Pager page, BrandRechargeVoucherSearch searchArgs)
        {
            searchArgs.Status = RechargeVoucherStatus.WaitAudit;
            var rows = _brandRechargeVoucherFacade.GetBrandVoucherList(page, searchArgs);
            return Json(new { success = true, data = rows, total = page.Total });
        }

        public JsonResult PassAuditBrandVoucher(int id)
        {
            _brandRechargeVoucherFacade.PassAudit(id);
            return Json(new { success = true });
        }

        public JsonResult RejectAuditBrandVoucher(int id)
        {
            _brandRechargeVoucherFacade.RejectAudit(id);
            return Json(new { success = true });
        }

        public JsonResult AbortBrandVoucher(int id)
        {
            _brandRechargeVoucherFacade.Abort(id);
            return Json(new { success = true });
        }

        #endregion

    }
}