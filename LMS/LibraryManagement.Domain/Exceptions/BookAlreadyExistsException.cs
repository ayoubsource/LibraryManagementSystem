namespace LibraryManagement.Domain.Exceptions
{
    public sealed class BookAlreadyExistsException : DomainException
    {
        public BookAlreadyExistsException(string bookId)
            : base($"Un ouvrage existe déjà pour l'ISBN : {bookId}.")
        {
        }
    }
}
