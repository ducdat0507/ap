using Validator.Internal;
using Validator.Generic;
using System.Text.RegularExpressions;
using System.Numerics;

namespace Validator 
{
    public class NumberValidator<T> : RequiredValidator<T> where T : INumber<T>
    {
        internal NumberValidator (InternalValidator<string, T> internalValidator)
        {
            InternalValidator = internalValidator;
        }

        public NumberValidator<T> GreaterThan(T value) 
        {
            InternalValidator = new InternalNumberValidators.GreaterThan<T>(InternalValidator, value);
            return this;
        }
        public NumberValidator<T> InRange(T min, T max) 
        {
            InternalValidator = new InternalNumberValidators.InRange<T>(InternalValidator, min, max);
            return this;
        }
        public NumberValidator<T> OneOf(params T[] values) 
        {
            InternalValidator = new InternalGenericValidators.OneOf<T>(InternalValidator, values);
            return this;
        }

        public OptionalNumberValidator<T> Optional() 
        {
            return new OptionalNumberValidator<T>(InternalValidator);
        }
        public OptionalNumberValidator<T> Default(T defaultValue) 
        {
            return new OptionalNumberValidator<T>(InternalValidator, defaultValue);
        }
    }

    public class OptionalNumberValidator<T> : OptionalValidator<T> where T : INumber<T>
    {
        internal OptionalNumberValidator (InternalValidator<string, T> internalValidator)
        {
            InternalValidator = internalValidator;
        }
        internal OptionalNumberValidator (InternalValidator<string, T> internalValidator, T defaultValue)
        {
            InternalValidator = internalValidator;
            DefaultValue = defaultValue;
        }
    }
}