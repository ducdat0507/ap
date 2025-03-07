using Validator.Internal;
using Validator.Generic;
using System.Text.RegularExpressions;
using System.Numerics;

namespace Validator 
{
    public class Validator : RequiredValidator<string>
    {
        public Validator() 
        {
            InternalValidator = new InternalIdentityValidator<string>();
        }

        public Validator MinLength(int length) 
        {
            InternalValidator = new InternalStringValidators.MinLength(InternalValidator, length);
            return this;
        }
        public Validator MaxLength(int length) 
        {
            InternalValidator = new InternalStringValidators.MaxLength(InternalValidator, length);
            return this;
        }
        public Validator Email() 
        {
            InternalValidator = new InternalStringValidators.Email(InternalValidator);
            return this;
        }
        public Validator Regex(Regex pattern) 
        {
            InternalValidator = new InternalStringValidators.Regex(InternalValidator, pattern);
            return this;
        }
        public Validator OneOf(params string[] values) 
        {
            InternalValidator = new InternalGenericValidators.OneOf<string>(InternalValidator, values);
            return this;
        }

        public OptionalValidator Optional() 
        {
            return new OptionalValidator(InternalValidator);
        }
        public OptionalValidator Default(string defaultValue) 
        {
            return new OptionalValidator(InternalValidator, defaultValue);
        }

        public NumberValidator<T> Number<T>() where T : IFloatingPoint<T>
        {
            return new NumberValidator<T>(new InternalStringValidators.Number<T>(InternalValidator));
        }
        public NumberValidator<T> WholeNumber<T>() where T : INumber<T>
        {
            return new NumberValidator<T>(new InternalStringValidators.WholeNumber<T>(InternalValidator));
        }
    }

    public class OptionalValidator : OptionalValidator<string> 
    {
        internal OptionalValidator (InternalValidator<string, string> internalValidator)
        {
            InternalValidator = internalValidator;
        }
        internal OptionalValidator (InternalValidator<string, string> internalValidator, string defaultValue)
        {
            InternalValidator = internalValidator;
            DefaultValue = defaultValue;
        }
    }
}