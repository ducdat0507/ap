using System.Globalization;
using System.Net.Mail;
using System.Numerics;

namespace Validator.Internal
{
    internal static class InternalStringValidators 
    {
        public class MinLength : InternalChainedValidator<string, string, string> 
        {
            readonly int Length;

            public MinLength(InternalValidator<string, string> parent, int length) : base(parent)
            {
                Length = length;
            }

            protected override string ValidateMid(string input)
            {
                if (input.Length < Length) throw new ValidatorFailedException(
                    $"Input must be at least {Length} characters long"
                );
                return input;
            } 
        }
        public class MaxLength : InternalChainedValidator<string, string, string> 
        {
            readonly int Length;

            public MaxLength(InternalValidator<string, string> parent, int length) : base(parent)
            {
                Length = length;
            }

            protected override string ValidateMid(string input)
            {
                if (input.Length > Length) throw new ValidatorFailedException(
                    $"Input can not be longer than {Length} characters"
                );
                return input;
            } 
        }
        public class Email : InternalChainedValidator<string, string, string> 
        {
            public Email(InternalValidator<string, string> parent) : base(parent) {}

            protected override string ValidateMid(string input)
            {
                try 
                { 
                    return new MailAddress(input).Address;
                }
                catch (FormatException e) 
                {
                    throw new ValidatorFailedException("Input must be a valid email address", e);
                }
            } 
        }
        public class Regex : InternalChainedValidator<string, string, string> 
        {
            readonly System.Text.RegularExpressions.Regex Pattern;

            public Regex (InternalValidator<string, string> parent, System.Text.RegularExpressions.Regex pattern) : base(parent)
            {
                Pattern = pattern;
            }

            protected override string ValidateMid(string input)
            {
                if (!Pattern.IsMatch(input)) throw new ValidatorFailedException(
                    $"Input must match the following regex pattern: {Pattern}"
                );
                return input;
            } 
        }
        public class WholeNumber<TNumber> : InternalChainedValidator<string, string, TNumber> where TNumber : INumber<TNumber>
        {
            public WholeNumber (InternalValidator<string, string> parent) : base(parent) {}

            protected override TNumber ValidateMid(string input)
            {
                try 
                { 
                    return TNumber.Parse(input, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
                catch (FormatException e) 
                {
                    throw new ValidatorFailedException("Input must be a valid whole number", e);
                }
                catch (OverflowException e) 
                {
                    throw new ValidatorFailedException($"Input is outside domain of representable numbers", e);
                }
            } 
        }
        public class Number<TNumber> : InternalChainedValidator<string, string, TNumber> where TNumber : IFloatingPoint<TNumber>
        {
            public Number (InternalValidator<string, string> parent) : base(parent) {}

            protected override TNumber ValidateMid(string input)
            {
                try 
                { 
                    return TNumber.Parse(input, NumberStyles.Number, CultureInfo.InvariantCulture);
                }
                catch (FormatException e) 
                {
                    throw new ValidatorFailedException("Input must be a valid number", e);
                }
                catch (OverflowException e) 
                {
                    throw new ValidatorFailedException($"Input is outside domain of representable numbers", e);
                }
            } 
        }
    }
}