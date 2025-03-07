namespace Validator
{
    [Serializable]
    public class ValidatorFailedException : Exception
    {
        public ValidatorFailedException() { }
        public ValidatorFailedException(string message) : base(message) { }
        public ValidatorFailedException(string message, Exception inner) : base(message, inner) { }
    }
}