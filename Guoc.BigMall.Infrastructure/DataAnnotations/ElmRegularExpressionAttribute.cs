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
    public class ElmRegularExpressionAttribute : RegularExpressionAttribute, IElmValidationAttribute
    {
        public FormValueType FormValueType { get; set; }
        public RuleTrigger FormRuleTrigger { get; set; }

        public ElmRegularExpressionAttribute(string pattern, FormValueType formValueType, RuleTrigger ruleTrigger, string errorMessage)
            : base(pattern)
        {
            this.FormValueType = formValueType;
            this.FormRuleTrigger = ruleTrigger;
            this.ErrorMessage = errorMessage;
        }

        public Rule TransformToRule()
        {
            return new Rule()
            {
                Type = this.FormValueType,
                Pattern = string.Format("/^{0}$/", this.Pattern),
                Trigger = this.FormRuleTrigger,
                Message = this.ErrorMessage,
            };
        }
    }
}
