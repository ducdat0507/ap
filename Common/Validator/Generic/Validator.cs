using Validator.Internal;

namespace Validator.Generic
{
    public abstract class Validator<T> 
    {
        public abstract T Validate(string input);
        public bool TryValidate(string input, out T output) 
        {
            try 
            {
                output = Validate(input);
                return true;
            }
            catch (ValidatorFailedException)
            {
                output = default;
                return false;
            }
        }
    }

    public abstract class RequiredValidator<T, TSelf> : Validator<T> 
        where TSelf : RequiredValidator<T, TSelf>
    {
        private protected InternalValidator<string, T> InternalValidator;

        public override T Validate(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) throw new ValidatorFailedException("Field is required");
            return InternalValidator.Validate(input.Trim());
        }

        public TSelf OneOf(params T[] values) 
        {
            InternalValidator = new InternalGenericValidators.OneOf<T>(InternalValidator, values);
            return (TSelf)this;
        }
        public TSelf Callback(Func<T, bool> callback, string failMessage) 
        {
            InternalValidator = new InternalGenericValidators.Callback<T>(InternalValidator, callback, failMessage);
            return (TSelf)this;
        }
        public TSelf Transform(Func<T, T> values) 
        {
            InternalValidator = new InternalGenericValidators.Transform<T>(InternalValidator, values);
            return (TSelf)this;
        }
    }

    public abstract class OptionalValidator<T> : Validator<T?>
    {
        private protected InternalValidator<string, T> InternalValidator;
        private protected T? DefaultValue = default;

        public override T? Validate(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return DefaultValue;
            return InternalValidator.Validate(input.Trim());
        }
    }
}
