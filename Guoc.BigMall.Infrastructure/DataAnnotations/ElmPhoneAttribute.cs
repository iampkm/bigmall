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
    public class ElmPhoneAttribute : DataTypeAttribute, IElmValidationAttribute
    {
        private PhoneAttribute innerAttribute = new PhoneAttribute();
        public FormValueType FormValueType { get; private set; }
        public RuleTrigger FormRuleTrigger { get; set; }

        public ElmPhoneAttribute(RuleTrigger formRuleTrigger, string errorMessage)
            : base(DataType.PhoneNumber)
        {
            this.FormValueType = FormValueType.String;
            this.FormRuleTrigger = formRuleTrigger;
            this.ErrorMessage = errorMessage;
        }

        public override bool IsValid(object value)
        {
            return innerAttribute.IsValid(value);
        }

        public Rule TransformToRule()
        {
            return new Rule()
            {
                Type = this.FormValueType,
                Trigger = this.FormRuleTrigger,
                Pattern = "/^(\\d{11}|\\d{8})$/",
                Message = this.ErrorMessage,
            };
        }
    }
}
