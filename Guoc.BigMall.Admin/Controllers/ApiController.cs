using Guoc.BigMall.Infrastructure.Http;
using Guoc.BigMall.Application;
using Guoc.BigMall.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Guoc.BigMall.Admin.Controllers
{
    public class ApiController : Controller
    {
        IApiFacade _iApiFacade;
        IPurchaseFacade _purchaseFacade;
        ISaleOrderFacade _saleOrderFacade;
        public ApiController(IApiFacade iApiFacade, IPurchaseFacade purchaseFacade, ISaleOrderFacade saleOrderFacade)
        {
            _iApiFacade = iApiFacade;
            _purchaseFacade = purchaseFacade;
            _saleOrderFacade = saleOrderFacade;
        }

        public JsonResult ClosePurchase()
        {
            var data = RequestProvider.GetRequestParameter();
            var code = data.GetValue("Code");
            var msg = _iApiFacade.ClosePurchase(code);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                return Json(new { success = false, data = msg }); ;
            }
            return Json(new { success = true });
        }

        public JsonResult PurchaseOrderInOrOut()
        {
            var data = RequestProvider.GetRequestParameter();
            var code = data.GetValue("Code");
            var msg = _iApiFacade.PurchaseOrderInOrOut(code);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                return Json(new { success = false, data = msg }); ;
            }
            return Json(new { success = true });
        }

        public JsonResult PurchaseOrderToSap()
        {
            var data = RequestProvider.GetRequestParameter();
            var code = data.GetValue("Code");
            var msg = _iApiFacade.PurchaseOrderToSap(code);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                return Json(new { success = false, data = msg }); ;
            }
            return Json(new { success = true });
        }

        public JsonResult TransferOrdeToSap()
        {
            var data = RequestProvider.GetRequestParameter();
            var code = data.GetValue("Code");
            var msg = _iApiFacade.TransferOrdeToSap(code);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                return Json(new { success = false, data = msg }); ;
            }
            return Json(new { success = true });
        }

        public JsonResult TransferOrderInOrOut()
        {
            var data = RequestProvider.GetRequestParameter();
            var code = data.GetValue("Code");
            var msg = _iApiFacade.TransferOrderInOrOut(code);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                return Json(new { success = false, data = msg }); ;
            }
            return Json(new { success = true });
        }

        public JsonResult SaleOrdeToSap()
        {
            var data = RequestProvider.GetRequestParameter();
            var code = data.GetValue("Code");
            var msg = _iApiFacade.SaleOrdeToSap(code);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                return Json(new { success = false, data = msg }); ;
            }
            return Json(new { success = true });
        }

        public JsonResult SaleOrderOutToSap()
        {
            var data = RequestProvider.GetRequestParameter();
            var code = data.GetValue("Code");
            var msg = _iApiFacade.SaleOrderOutToSap(code);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                return Json(new { success = false, data = msg }); ;
            }
            return Json(new { success = true });
        }

        public JsonResult SaleOrderInToSap()
        {
            var data = RequestProvider.GetRequestParameter();
            var code = data.GetValue("Code");
            var msg = _iApiFacade.SaleOrderInToSap(code);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                return Json(new { success = false, data = msg }); ;
            }
            return Json(new { success = true });
        }

        public JsonResult PreOrderReturnInStock()
        {
            var data = RequestProvider.GetRequestParameter();
            var code = data.GetValue("Code");
            var msg = _iApiFacade.PreOrderReturnInStock(code);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                return Json(new { success = false, data = msg }); ;
            }
            return Json(new { success = true });
        }

        public JsonResult PreConvertOrder()
        {
            var data = RequestProvider.GetRequestParameter();
            var code = data.GetValue("Code");
            var msg = _iApiFacade.PreConvertOrder(code);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                return Json(new { success = false, data = msg }); ;
            }
            return Json(new { success = true });
        }

        public JsonResult AbandonSaleOrder()
        {
            var data = RequestProvider.GetRequestParameter();
            var code = data.GetValue("Code");
            var msg = _iApiFacade.AbandonSaleOrder(code);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                return Json(new { success = false, data = msg }); ;
            }
            return Json(new { success = true });
        }

        /// <summary>
        /// 手工补发工具页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Tool()
        {
            return View();
        }

        /// <summary>
        /// 用excel 初始化库存
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public JsonResult InitInventory(string json, string file)
        {
            _iApiFacade.initInventory(json, file);
            return Json(new { success = true });
        }

        public JsonResult InitStockIn(string codes)
        {
            this._purchaseFacade.InitStockIn(codes);
            return Json(new { success = true });
        }


        public ActionResult RefundNoSourceSO()
        {
            return View();
        }

        public JsonResult CreateROWithNoSourceSO(string storeCode, string snCode, string productCode, int quantity, decimal realPrice)
        {
            var returnOrderCode = _iApiFacade.CreateROWithNoSourceSO(storeCode, snCode, productCode, quantity, realPrice);
            return Json(new { success = true, Code = returnOrderCode });
        }

        public JsonResult ROInStockWithNoSourceSO(string returnOrderCode, string supplierCode, decimal costPrice)
        {
            _iApiFacade.ROInStockWithNoSourceSO(returnOrderCode, supplierCode, costPrice);
            return Json(new { success = true });
        }
    }
}