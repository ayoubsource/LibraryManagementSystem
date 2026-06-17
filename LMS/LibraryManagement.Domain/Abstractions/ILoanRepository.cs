using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Abstractions
{
	public interface ILoanRepository
	{
        IReadOnlyList<Loan> GetActiveLoansOf(Guid memberId);
        Loan? GetActiveLoanByCopy(string copyId);
        void Add(Loan loan);
        void Update(Loan loan);
    }
}

