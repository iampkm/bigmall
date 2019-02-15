using Guoc.BigMall.Infrastructure.DataAnnotations;
using Guoc.BigMall.Infrastructure.DataAnnotations.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Guoc.BigMall.Admin
{
    public static class FormRules
    {
        private static ConcurrentDictionary<Type, dynamic> _cache = new ConcurrentDictionary<Type, dynamic>();
        public static string GetRules<TModel>()
        {
            return FormRules.GetRules(typeof(TModel));
        }

        public static string GetRules(Type modelType)
        {
            if (_cache.ContainsKey(modelType))
                return ToJsonString(_cache[modelType]);

            var ruleContainer = (new ExpandoObject()) as IDictionary<string, object>;

            modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList().ForEach(prop =>
            {
                var rules = new List<Rule>();
                prop.GetCustomAttributes<ValidationAttribute>(true).ToList().ForEach(attr =>
                {
                    var rule = CreateRule(attr);
                    if (rule != null) rules.Add(rule);
                });
                if (rules.Count > 0)
                    ruleContainer[prop.Name] = rules;
            });
            _cache.TryAdd(modelType, ruleContainer);

            return ToJsonString(ruleContainer);
        }

        private static Rule CreateRule(ValidationAttribute attr)
        {
            if (attr is IElmValidationAttribute)
                return (attr as IElmValidationAttribute).TransformToRule();

            var rule = attr.IsRequired();

            if (rule != null) return rule;

            rule = attr.IsMaxLength();

            if (rule != null) return rule;

            rule = attr.IsMinLength();

            if (rule != null) return rule;

            rule = attr.IsStringLength();

            if (rule != null) return rule;

            rule = attr.IsRange();

            if (rule != null) return rule;

            rule = attr.IsPhone();

            if (rule != null) return rule;

            rule = attr.IsEmailAddress();

            if (rule != null) return rule;

            rule = attr.IsRegularExpression();

            if (rule != null) return rule;

            rule = attr.IsFileExtensions();

            if (rule != null) return rule;

            rule = attr.IsDataType();

            if (rule != null) return rule;

            rule = attr.IsEnumDataType();

            if (rule != null) return rule;

            return null;
        }

        #region 扩展方法

        private static Rule IsRequired(this ValidationAttribute attr)
        {
            if (attr is RequiredAttribute)
            {
                return new Rule()
                {
                    Required = true,
                    Message = attr.ErrorMessage,
                };
            }
            return null;
        }

        private static Rule IsMaxLength(this ValidationAttribute attr)
        {
            if (attr is MaxLengthAttribute)
            {
                var actualAttr = attr as MaxLengthAttribute;
                return new Rule()
                {
                    Max = actualAttr.Length,
                    Message = actualAttr.ErrorMessage,
                };
            }
            return null;
        }

        private static Rule IsMinLength(this ValidationAttribute attr)
        {
            if (attr is MinLengthAttribute)
            {
                var actualAttr = attr as MinLengthAttribute;
                return new Rule()
                {
                    Min = actualAttr.Length,
                    Message = actualAttr.ErrorMessage,
                };
            }
            return null;
        }

        private static Rule IsStringLength(this ValidationAttribute attr)
        {
            if (attr is StringLengthAttribute)
            {
                var actualAttr = attr as StringLengthAttribute;
                return new Rule()
                {
                    //Min = actualAttr.MinimumLength,
                    //Max = actualAttr.MaximumLength,
                    Len = actualAttr.MaximumLength,
                    Message = actualAttr.ErrorMessage,
                };
            }
            return null;
        }

        private static Rule IsRange(this ValidationAttribute attr)
        {
            if (attr is RangeAttribute)
            {
                var actualAttr = attr as RangeAttribute;
                return new Rule()
                {
                    Min = actualAttr.Minimum,
                    Max = actualAttr.Maximum,
                    Message = actualAttr.ErrorMessage,
                };
            }
            return null;
        }

        private static Rule IsPhone(this ValidationAttribute attr)
        {
            if (attr is PhoneAttribute)
            {
                return new Rule()
                {
                    Pattern = "/^(\\d{11}|\\d{8})$/",//
                    Message = attr.ErrorMessage,
                };
            }
            return null;
        }

        private static Rule IsEmailAddress(this ValidationAttribute attr)
        {
            if (attr is EmailAddressAttribute)
            {
                var actualAttr = attr as EmailAddressAttribute;
                return new Rule()
                {
                    Type = FormValueType.Email,
                    //Pattern = "/^\\S+@\\S+\\.(cn|com)$/i",
                    Message = actualAttr.ErrorMessage,
                };
            }
            return null;
        }

        private static Rule IsRegularExpression(this ValidationAttribute attr)
        {
            if (attr is RegularExpressionAttribute)
            {
                var actualAttr = attr as RegularExpressionAttribute;
                return new Rule()
                {
                    Pattern = actualAttr.Pattern,
                    Message = actualAttr.ErrorMessage,
                };
            }
            return null;
        }

        private static Rule IsFileExtensions(this ValidationAttribute attr)
        {
            if (attr is FileExtensionsAttribute)
            {
                var actualAttr = attr as FileExtensionsAttribute;
                return new Rule()
                {
                    Pattern = string.Format("/\\.({0})$/", actualAttr.Extensions),
                    Message = actualAttr.ErrorMessage,
                };
            }
            return null;
        }

        private static Rule IsDataType(this ValidationAttribute attr)
        {
            if (attr is DataTypeAttribute)
            {
                var rule = new Rule();
                var actualAttr = attr as DataTypeAttribute;
                rule.Message = actualAttr.ErrorMessage;
                switch (actualAttr.DataType)
                {
                    //case DataType.CreditCard:
                    //    break;
                    //case DataType.Currency:
                    //    break;
                    //case DataType.Custom:
                    //    break;
                    case DataType.Time:
                        rule.Type = FormValueType.Date;
                        rule.Pattern = "/^\\d{2}:\\d{2}:\\d{2}$/";
                        break;
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
                    //    dataType = FormValueType.String;
                    //    break;
                    //case DataType.Password:
                    //    break;
                    //case DataType.PhoneNumber:
                    //    break;
                    //case DataType.PostalCode:
                    //    break;
                    //case DataType.Text:
                    //    break;
                    //case DataType.Upload:
                    //    break;
                    //case DataType.Url:
                    //    break;
                    default:
                        rule.Type = FormValueType.String;
                        break;
                }
                return rule;
            }
            return null;
        }

        private static Rule IsEnumDataType(this ValidationAttribute attr)
        {
            if (attr is EnumDataTypeAttribute)
            {
                var actualAttr = attr as EnumDataTypeAttribute;
                var fields = actualAttr.EnumType.GetFields().Select(f => f.Name).ToArray();
                return new Rule()
                {
                    Type = FormValueType.Enum,
                    Enum = fields,
                    Message = actualAttr.ErrorMessage,
                };
            }
            return null;
        }

        #endregion

        private static string ToJsonString(object obj)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Converters.Add(new StringEnumConverter(true));
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}