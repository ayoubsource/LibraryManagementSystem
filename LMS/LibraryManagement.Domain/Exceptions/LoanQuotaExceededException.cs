namespace LibraryManagement.Domain.Exceptions
{
    public sealed class LoanQuotaExceededException : DomainException
    {
        public LoanQuotaExceededException(Guid memberId, int maxLoans)
            : base($"Member {memberId} has reached the maximum of {maxLoans} active loans.")
        {
        }
    }
}

