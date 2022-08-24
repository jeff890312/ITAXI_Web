using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITAXI.Utilities
{
    public class NotEqualToAttribute : ValidationAttribute
    {
        private readonly string _otherPropertyDisplayName;

        public NotEqualToAttribute(string otherPropertyDisplayName)
        {
            _otherPropertyDisplayName = otherPropertyDisplayName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherProperty = validationContext.ObjectType.GetProperty(_otherPropertyDisplayName);
            if (otherProperty == null) throw new ArgumentException("other property with the display name not found");
            var otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance);

            if (Object.Equals(value, otherPropertyValue))
                return new ValidationResult(ErrorMessageString);
            else
                return ValidationResult.Success;
        }

    }
}