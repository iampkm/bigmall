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
    public class ElmDataTypeAttribute : DataTypeAttribute, IElmValidationAttribute
    {
        public FormValueType FormValueType { get; set; }
        public RuleTrigger FormRuleTrigger { get; set; }

        public ElmDataTypeAttribute(DataType dataType)
            : base(dataType)
        {
        }

        public Rule TransformToRule()
        {
            var rule = new Rule()
            {
                Trigger = this.FormRuleTrigger,
                Message = this.ErrorMessage,
            };

            switch (this.DataType)
            {
                //case DataType.CreditCard:
                //    break;
                //case DataType.Currency:
                //    break;
                //case DataType.Custom:
                //    break;
                case DataType.Date:
                case DataType.DateTime:
                    rule.Type = FormValueType.Date;
                    break;
                //case DataType.Duration:
                //    break;
                case DataType.EmailAddress:
                    rule.Type = FormValueType.Email;
                    break;
                //case DataType.Html:
                //    break;
                //case DataType.ImageUrl:
                //    break;
                //case DataType.MultilineText:
                //    break;
                //case DataType.Password:
                //    break;
                //case DataType.PhoneNumber:
                //    break;
                case DataType.PostalCode:
                    break;
                case DataType.Text:
                    rule.Type = FormValueType.String;
                    break;
                case DataType.Time:
                    //.............

                    break;
                //case DataType.Upload:
                //    break;
                case DataType.Url:
                    break;
                default:
                    break;
            }

            return rule;
        }
    }
}
