namespace LibraryManagement.Domain.Exceptions
{
	public sealed class LoanAlreadyReturnedException : DomainException
	{
		public LoanAlreadyReturnedException(Guid loanId) : base($"L'emprunt {loanId} est déjà clôturé.")
		{
		}
	}
}

