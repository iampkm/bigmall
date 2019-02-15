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
    public class ElmEmailAddressAttribute : DataTypeAttribute, IElmValidationAttribute
    {
        private EmailAddressAttribute innerAttribute = new EmailAddressAttribute();
        public FormValueType FormValueType { get; private set; }
        public RuleTrigger FormRuleTrigger { get; set; }

        public ElmEmailAddressAttribute(RuleTrigger ruleTrigger, string errorMessage)
            : base(DataType.EmailAddress)
        {
            this.FormValueType = FormValueType.Email;
            this.FormRuleTrigger = ruleTrigger;
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
                Message = this.ErrorMessage,
            };
        }
    }
}
