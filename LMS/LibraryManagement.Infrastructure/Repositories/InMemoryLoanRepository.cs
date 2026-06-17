using LibraryManagement.Domain.Abstractions;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Repositories
{
    public sealed class InMemoryLoanRepository : ILoanRepository
    {
        private readonly Dictionary<Guid, Loan> _loans = new();

        public IReadOnlyList<Loan> GetActiveLoansOf(Guid memberId) =>
            _loans.Values
                .Where(l => l.MemberId == memberId && l.IsActive)
                .ToList();

        public Loan? GetActiveLoanByCopy(string copyId) =>
            _loans.Values.FirstOrDefault(l => l.CopyId == copyId && l.IsActive);

        public void Add(Loan loan) => _loans[loan.Id] = loan;

        public void Update(Loan loan) => _loans[loan.Id] = loan;
    }
}
