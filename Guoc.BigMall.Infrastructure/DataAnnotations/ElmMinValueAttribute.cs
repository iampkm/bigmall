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
    public class ElmMinValueAttribute : ValidationAttribute, IElmValidationAttribute
    {
        public FormValueType FormValueType { get; set; }
        public RuleTrigger FormRuleTrigger { get; set; }
        public object Minimum { get; set; }

        public ElmMinValueAttribute(object minimum, string errorMessage = null)
            : this(minimum, FormValueType.Number, RuleTrigger.Blur, errorMessage)
        {
        }

        public ElmMinValueAttribute(object minimum, FormValueType formValueType, RuleTrigger formRuleTrigger, string errorMessage)
        {
            if (!(minimum is IComparable))
                throw new ArgumentException("最小值参数类型必须实现System.IComparable。", "minimum");
            if (formValueType != FormValueType.Integer && formValueType != FormValueType.Float && formValueType != FormValueType.Number && formValueType != FormValueType.Date)
                throw new ArgumentException("ElmMinValueAttribute仅支持FormValueType.Integer、FormValueType.Float、FormValueType.Number、FormValueType.Date。", "formValueType");

            this.Minimum = minimum;
            this.FormValueType = formValueType;
            this.FormRuleTrigger = formRuleTrigger;
            this.ErrorMessage = errorMessage;
        }

        public override bool IsValid(object value)
        {
            if (value == null) return false;

            double val;
            if (value.GetType().IsValueType && double.TryParse(Convert.ToString(value), out val))
                return ((IComparable)Convert.ToDouble(this.Minimum)).CompareTo(val) <= 0;

            return ((IComparable)this.Minimum).CompareTo(value) <= 0;
        }

        public Rule TransformToRule()
        {
            return new Rule()
            {
                Min = this.Minimum,
                Type = this.FormValueType,
                Trigger = this.FormRuleTrigger,
                Message = this.ErrorMessage,
            };
        }
    }
}
