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
    public class ElmStringLengthAttribute : StringLengthAttribute, IElmValidationAttribute
    {
        public FormValueType FormValueType { get; private set; }
        public RuleTrigger FormRuleTrigger { get; set; }

        public ElmStringLengthAttribute(int maximumLength, RuleTrigger formRuleTrigger, string errorMessage)
            : base(maximumLength)
        {
            this.FormValueType = FormValueType.String;
            this.FormRuleTrigger = formRuleTrigger;
            this.ErrorMessage = errorMessage;
        }

        public Rule TransformToRule()
        {
            return new Rule()
            {
                Len = this.MaximumLength,
                Type = this.FormValueType,
                Trigger = this.FormRuleTrigger,
                Message = this.ErrorMessage,
            };
        }
    }
}
