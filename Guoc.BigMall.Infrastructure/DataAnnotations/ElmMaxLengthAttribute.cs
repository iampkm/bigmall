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
    public class ElmMaxLengthAttribute : MaxLengthAttribute, IElmValidationAttribute
    {
        public FormValueType FormValueType { get; set; }
        public RuleTrigger FormRuleTrigger { get; set; }

        public ElmMaxLengthAttribute(int length, FormValueType formValueType, RuleTrigger formRuleTrigger, string errorMessage)
            : base(length)
        {
            this.FormValueType = formValueType;
            this.FormRuleTrigger = formRuleTrigger;
            this.ErrorMessage = errorMessage;
        }

        public Rule TransformToRule()
        {
            return new Rule()
            {
                Max = this.Length,
                Type = this.FormValueType,
                Trigger = this.FormRuleTrigger,
                Message = this.ErrorMessage,
            };
        }
    }
}
