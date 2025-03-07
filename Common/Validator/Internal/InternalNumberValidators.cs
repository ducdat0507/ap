using System.Globalization;
using System.Net.Mail;
using System.Numerics;

namespace Validator.Internal
{
    internal static class InternalNumberValidators 
    {
        public class GreaterThan<T> : InternalChainedValidator<string, T, T> where T : INumber<T>
        {
            readonly T Value;

            public GreaterThan(InternalValidator<string, T> parent, T value) : base(parent)
            {
                Value = value;
            }

            protected override T ValidateMid(T input)
            {
                if (!(input > Value)) throw new ValidatorFailedException(
                    $"Input must be greater than {Value}"
                );
                return input;
            } 
        }

        public class InRange<T> : InternalChainedValidator<string, T, T> where T : INumber<T>
        {
            readonly T Min;
            readonly T Max;

            public InRange(InternalValidator<string, T> parent, T min, T max) : base(parent)
            {
                Min = min;
                Max = max;
            }

            protected override T ValidateMid(T input)
            {
                if (input < Min || input > Max) throw new ValidatorFailedException(
                    $"Input must be between {Min} and {Max}"
                );
                return input;
            } 
        }
    }
}