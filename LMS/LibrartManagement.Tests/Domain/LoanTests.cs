using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;
using LibraryManagement.Domain.Policies;
using NUnit.Framework;

namespace LibrartManagement.Tests.Domain
{
    [TestFixture]
    public sealed class LoanTests
    {
        private readonly LoanPolicy _policy = new();
        private readonly PenaltyCalculator _calculator = new();
        private static readonly DateOnly LoanDate = new(2026, 1, 1);

        private Loan CreateStandardLoan()
        {
            var member = new Member(Guid.NewGuid(), "Alice", MembershipType.Standard);
            var copy = new BookCopy("CB-1", "ISBN-1");
            return Loan.Create(member, copy, LoanDate, _policy);
        }

        [Test]
        public void Create_sets_due_date_from_membership_duration()
        {
            var loan = CreateStandardLoan();

            // Standard : 21 jours de prêt
            Assert.That(loan.DueDate, Is.EqualTo(LoanDate.AddDays(21)));
            Assert.That(loan.IsActive, Is.True);
        }

        [Test]
        public void Return_on_time_produces_no_penalty()
        {
            var loan = CreateStandardLoan();

            var penalty = loan.Return(loan.DueDate, _calculator);

            Assert.That(penalty.IsNone, Is.True);
            Assert.That(loan.IsActive, Is.False);
        }

        [Test]
        public void Return_late_computes_late_days_and_penalty()
        {
            var loan = CreateStandardLoan();

            var penalty = loan.Return(loan.DueDate.AddDays(3), _calculator);

            Assert.That(penalty.LateDays, Is.EqualTo(3));
            Assert.That(penalty.Amount.Amount, Is.EqualTo(0.60m));
        }

        [Test]
        public void Return_twice_throws()
        {
            var loan = CreateStandardLoan();
            loan.Return(loan.DueDate, _calculator);

            Assert.Throws<LoanAlreadyReturnedException>(
                () => loan.Return(loan.DueDate, _calculator));
        }

        [Test]
        public void Return_before_loan_date_throws()
        {
            var loan = CreateStandardLoan();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => loan.Return(LoanDate.AddDays(-1), _calculator));
        }
    }
}
