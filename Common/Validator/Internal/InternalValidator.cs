namespace Validator.Internal
{
    internal abstract class InternalValidator<TIn, TOut>
    {
        public abstract TOut Validate(TIn input);
    }

    internal class InternalIdentityValidator<T> : InternalValidator<T, T>
    {
        public override T Validate(T input) 
        {
            return input;
        }
    }

    internal abstract class InternalChainedValidator<TIn, TMid, TOut> : InternalValidator<TIn, TOut> 
    {
        protected InternalValidator<TIn, TMid> Parent;

        public InternalChainedValidator(InternalValidator<TIn, TMid> parent)
        {
            Parent = parent;
        }

        protected abstract TOut ValidateMid(TMid input);
        public override TOut Validate(TIn input)
        {
            return ValidateMid(Parent.Validate(input));
        }
    }
}