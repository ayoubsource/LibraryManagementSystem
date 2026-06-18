namespace LibraryManagement.Domain.Exceptions
{
    public sealed class BookNotFoundException : DomainException
    {
        public BookNotFoundException(string bookId)
            : base($"Ouvrage introuvable : {bookId}.")
        {
        }
    }
}

