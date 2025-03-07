using System.Globalization;
using System.Net.Mail;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Validator.Internal
{
    internal static class InternalGenericValidators 
    {
        public class OneOf<T> : InternalChainedValidator<string, T, T>
        {
            readonly T[] Values;

            public OneOf(InternalValidator<string, T> parent, T[] values) : base(parent)
            {
                Values = values;
            }

            protected override T ValidateMid(T input)
            {
                if (!Values.Contains(input)) throw new ValidatorFailedException(
                    $"Values must be one of the following: {string.Join(", ", Values)}"
                );
                return input;
            } 
        }
    }
}