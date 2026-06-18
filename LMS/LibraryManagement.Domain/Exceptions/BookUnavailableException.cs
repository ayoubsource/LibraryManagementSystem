namespace LibraryManagement.Domain.Exceptions
{
    public sealed class BookUnavailableException : DomainException
    {
        public BookUnavailableException(string isbn) : base($"No available copies for book {isbn}.")
        {
        }
    }
}

