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

    public abstract class RequiredValidator<T> : Validator<T> 
    {
        private protected InternalValidator<string, T> InternalValidator;

        public override T Validate(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) throw new ValidatorFailedException("Field is required");
            return InternalValidator.Validate(input.Trim());
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
