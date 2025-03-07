namespace Validator
{
    [Serializable]
    public class PrompterCancelledException : Exception
    {
        public PrompterCancelledException() { }
        public PrompterCancelledException(string message) : base(message) { }
        public PrompterCancelledException(string message, Exception inner) : base(message, inner) { }
    }
}