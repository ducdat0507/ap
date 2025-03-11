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
        public class Callback<T> : InternalChainedValidator<string, T, T>
        {
            readonly Func<T, bool> CallbackFunc;
            readonly string Message;

            public Callback(InternalValidator<string, T> parent, Func<T, bool> callbackFunc, string meesage) : base(parent)
            {
                CallbackFunc = callbackFunc;
                Message = meesage;
            }

            protected override T ValidateMid(T input)
            {
                if (!CallbackFunc(input)) throw new ValidatorFailedException(
                    Message
                );
                return input;
            } 
        }
        public class Transform<T> : InternalChainedValidator<string, T, T>
        {
            readonly Func<T, T> CallbackFunc;

            public Transform(InternalValidator<string, T> parent, Func<T, T> callbackFunc) : base(parent)
            {
                CallbackFunc = callbackFunc;
            }

            protected override T ValidateMid(T input)
            {
                return CallbackFunc(input);
            } 
        }
    }
}