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
    public class ElmRequiredAttribute : RequiredAttribute, IElmValidationAttribute
    {
        public FormValueType FormValueType { get; set; }
        public RuleTrigger FormRuleTrigger { get; set; }

        public ElmRequiredAttribute(FormValueType formValueType, string errorMessage = null)
            : this(formValueType, default(RuleTrigger), errorMessage)
        {
        }

        public ElmRequiredAttribute(RuleTrigger formRuleTrigger, string errorMessage = null)
            : this(default(FormValueType), formRuleTrigger, errorMessage)
        {
        }

        public ElmRequiredAttribute(FormValueType formValueType, RuleTrigger formRuleTrigger, string errorMessage)
        {
            this.FormValueType = formValueType;
            this.FormRuleTrigger = formRuleTrigger;
            this.ErrorMessage = errorMessage;
        }

        public Rule TransformToRule()
        {
            return new Rule()
            {
                Required = true,
                Type = this.FormValueType,
                Trigger = this.FormRuleTrigger,
                Message = this.ErrorMessage,
            };
        }
    }
}
