namespace LibraryManagement.Domain.Exceptions
{
    public sealed class BookAlreadyBorrowedException : DomainException
    {
        public BookAlreadyBorrowedException(Guid memberId, string isbn) : base($"Member {memberId} has already borrowed book {isbn}.")
        {
        }
    }
}

