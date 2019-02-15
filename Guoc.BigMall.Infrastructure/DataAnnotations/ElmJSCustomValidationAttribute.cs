using Guoc.BigMall.Infrastructure.DataAnnotations.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Guoc.BigMall.Infrastructure.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ElmJSCustomValidationAttribute : ValidationAttribute, IElmValidationAttribute
    {
        public string JSValidator { get; set; }
        public FormValueType FormValueType { get; set; }
        public RuleTrigger FormRuleTrigger { get; set; }

        public ElmJSCustomValidationAttribute(string jSValidator, RuleTrigger formRuleTrigger = RuleTrigger.Blur)
        {
            this.JSValidator = jSValidator;
            this.FormRuleTrigger = formRuleTrigger;
        }

        public override bool IsValid(object value)
        {
            return true;
        }

        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //    return null;
        //}

        public Rule TransformToRule()
        {
            return new Rule()
            {
                Validator = this.JSValidator,
                Trigger = this.FormRuleTrigger,
                //Message = this.ErrorMessage,
            };
        }
    }
}