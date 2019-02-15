using Guoc.BigMall.Infrastructure.DataAnnotations.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoc.BigMall.Infrastructure.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ElmMaxValueAttribute : ValidationAttribute, IElmValidationAttribute
    {
        public FormValueType FormValueType { get; set; }
        public RuleTrigger FormRuleTrigger { get; set; }
        public object Maximum { get; set; }

        public ElmMaxValueAttribute(object maximum, string errorMessage = null)
            : this(maximum, FormValueType.Number, RuleTrigger.Blur, errorMessage)
        {
        }

        public ElmMaxValueAttribute(object maximum, FormValueType formValueType, RuleTrigger formRuleTrigger, string errorMessage)
        {
            if (!(maximum is IComparable))
                throw new ArgumentException("最大值参数类型必须实现System.IComparable。", "maximum");
            if (formValueType != FormValueType.Integer && formValueType != FormValueType.Float && formValueType != FormValueType.Number && formValueType != FormValueType.Date)
                throw new ArgumentException("ElmMaxValueAttribute仅支持FormValueType.Integer、FormValueType.Float、FormValueType.Number、FormValueType.Date。", "formValueType");

            this.Maximum = maximum;
            this.FormValueType = formValueType;
            this.FormRuleTrigger = formRuleTrigger;
            this.ErrorMessage = errorMessage;
        }

        public override bool IsValid(object value)
        {
            return ((IComparable)this.Maximum).CompareTo(value) >= 0;
        }

        public Rule TransformToRule()
        {
            return new Rule()
            {
                Max = this.Maximum,
                Type = this.FormValueType,
                Trigger = this.FormRuleTrigger,
                Message = this.ErrorMessage,
            };
        }
    }
}
