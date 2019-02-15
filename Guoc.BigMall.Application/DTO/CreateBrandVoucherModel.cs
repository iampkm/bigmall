using Guoc.BigMall.Infrastructure.DataAnnotations;
using Guoc.BigMall.Infrastructure.DataAnnotations.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Application.DTO
{
    public class CreateBrandVoucherModel
    {
        [ElmRequired(RuleTrigger.Change, "必须选择一个门店。")]
        //[ElmMinValue(1, "门店无效。")]
        public int StoreId { get; set; }

        [ElmRequired(RuleTrigger.Change, "必须选择一个品牌。")]
        public int BrandId { get; set; }

        [ElmRequired(FormValueType.Array, RuleTrigger.Change, "请选择活动起止时间。")]
        public string DateRange { get; set; }

        [ElmRequired(RuleTrigger.Change, "至少选择一个品类。")]
        public string CategoryIds { get; set; }

        [ElmRequired(FormValueType.Number, "卡券总金额不能为空。")]
        [ElmMinValue(0.01, "卡券总金额必须 ≥0.01。")]
        public decimal Amount { get; set; }

        [ElmRequired(FormValueType.Number, "必须指定单品限额。")]
        [ElmRange(0.01, 100, ErrorMessage = "单品限额必须介于 0.01% ~ 100% 之间。")]
        public decimal Limit { get; set; }

        public int[] ExceptProductIds { get; set; }
    }
}
