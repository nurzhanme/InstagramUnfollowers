namespace InstagramUnfollowers
{
    public class InstaException : Exception
    {
        public InstaException() { }
        public InstaException(string message) : base(message) { }
        public InstaException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
