namespace LibraryManagement.Domain.Exceptions
{
    public sealed class CopyNotFoundException : DomainException
    {
        public CopyNotFoundException(string copyId) : base($"Copy not found: {copyId}.")
        {
        }
    }
}

