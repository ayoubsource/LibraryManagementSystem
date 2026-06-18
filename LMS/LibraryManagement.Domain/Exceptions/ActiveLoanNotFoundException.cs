namespace LibraryManagement.Domain.Exceptions
{
    public sealed class ActiveLoanNotFoundException : DomainException
    {
        public ActiveLoanNotFoundException(string copyId) : base($"No active loan found for copy {copyId}.")
        {
        }
    }
}

