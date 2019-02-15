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
    public class ElmFileExtensionsAttribute : DataTypeAttribute, IElmValidationAttribute
    {
        private FileExtensionsAttribute innerAttribute = new FileExtensionsAttribute();
        public FormValueType FormValueType { get; private set; }
        public RuleTrigger FormRuleTrigger { get; set; }
        public string Extensions { get; set; }

        public ElmFileExtensionsAttribute(string extensions, RuleTrigger ruleTrigger, string errorMessage)
            : base(DataType.Custom)
        {
            innerAttribute.Extensions = extensions;
            this.Extensions = extensions;
            this.FormValueType = FormValueType.String;
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
                Pattern = string.IsNullOrEmpty(this.Extensions) ? "/\\.(png|jpg|jpeg|gif)$/" : string.Format("/\\.({0})$/", this.Extensions),
                Trigger = this.FormRuleTrigger,
                Message = this.ErrorMessage,
            };
        }
    }
}
