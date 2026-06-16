using LibraryManagement.Domain.Exceptions;
using LibraryManagement.Domain.Policies;
using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Domain.Entities
{
    public sealed class Loan
    {
        public Guid Id { get; }
        public Guid MemberId { get; }
        public string BookId { get; } 
        public string CopyId { get; }
        public DateOnly LoanDate { get; }
        public DateOnly DueDate { get; }
        public DateOnly? ReturnDate { get; private set; }
        public bool IsActive => ReturnDate is null;

        private Loan(Guid id, Guid memberId, string bookId, string copyId, DateOnly loanDate, DateOnly dueDate)
        {
            Id = id; MemberId = memberId; BookId = bookId; CopyId = copyId;
            LoanDate = loanDate; DueDate = dueDate;
        }

        public static Loan Create(Member member, BookCopy copy, DateOnly loanDate, LoanPolicy policy)
        {
            var duration = policy.LoanDurationInDaysFor(member.Membership);
            return new Loan(Guid.NewGuid(), member.Id, copy.BookId, copy.Id,
            loanDate, loanDate.AddDays(duration));
        }


        public Penalty Return(DateOnly returnDate, PenaltyCalculator calculator)
        {
            if (!IsActive) throw new LoanAlreadyReturnedException(Id);
            if (returnDate < LoanDate)
                throw new ArgumentOutOfRangeException(nameof(returnDate), "La date de retour ne peut pas précéder la date d'emprunt.");
            ReturnDate = returnDate;
            var lateDays = Math.Max(0, returnDate.DayNumber - DueDate.DayNumber);
            return calculator.Calculate(lateDays);
        }

    }
}

