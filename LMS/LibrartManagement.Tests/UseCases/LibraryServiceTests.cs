using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Abstractions;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;
using LibraryManagement.Domain.Policies;
using LibraryManagement.Infrastructure.Repositories;
using LibrartManagement.Tests.TestDoubles;
using NUnit.Framework;

namespace LibrartManagement.Tests.UseCases
{
    [TestFixture]
    public sealed class LibraryServiceTests
    {
        private static readonly DateOnly Today = new(2026, 1, 1);

        private IBookRepository _books = null!;
        private IBookCopyRepository _copies = null!;
        private IMemberRepository _members = null!;
        private ILoanRepository _loans = null!;
        private LibraryService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _books = new InMemoryBookRepository();
            _copies = new InMemoryBookCopyRepository();
            _members = new InMemoryMemberRepository();
            _loans = new InMemoryLoanRepository();

            _service = new LibraryService(
                _books, _copies, _members, _loans,
                new LoanPolicy(), new PenaltyCalculator(),
                new FixedClock(Today));
        }

        private Book SeedBook(string isbn, int copies)
        {
            var book = new Book(isbn, $"Titre {isbn}", "Auteur");
            _books.Add(book);
            for (var i = 0; i < copies; i++)
                _copies.Add(new BookCopy($"{isbn}-CB{i}", isbn));
            return book;
        }

        private Member SeedMember(MembershipType type)
        {
            var member = new Member(Guid.NewGuid(), "Membre", type);
            _members.Add(member);
            return member;
        }

        [Test]
        public void RegisterBook_adds_the_book_with_the_requested_number_of_copies()
        {
            _service.RegisterBook("ISBN-1", "L'Étranger", "Albert Camus", copies: 3);

            Assert.That(_service.CountAvailableCopies("ISBN-1"), Is.EqualTo(3));
        }

        [Test]
        public void RegisterBook_throws_when_copies_below_one()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => _service.RegisterBook("ISBN-1", "Titre", "Auteur", copies: 0));
        }

        [Test]
        public void RegisterBook_throws_when_isbn_already_registered()
        {
            _service.RegisterBook("ISBN-1", "Titre", "Auteur", copies: 1);

            Assert.Throws<BookAlreadyExistsException>(
                () => _service.RegisterBook("ISBN-1", "Titre", "Auteur", copies: 1));
        }

        [Test]
        public void Borrow_marks_a_copy_on_loan_and_decreases_availability()
        {
            var member = SeedMember(MembershipType.Standard);
            SeedBook("ISBN-1", copies: 2);

            var loan = _service.BorrowBook(member.Id, "ISBN-1");

            Assert.That(loan.MemberId, Is.EqualTo(member.Id));
            Assert.That(loan.DueDate, Is.EqualTo(Today.AddDays(21)));
            Assert.That(_service.CountAvailableCopies("ISBN-1"), Is.EqualTo(1));
        }

        [Test]
        public void Borrow_throws_when_no_copy_available()
        {
            var member = SeedMember(MembershipType.Standard);
            SeedBook("ISBN-1", copies: 1);
            _service.BorrowBook(member.Id, "ISBN-1"); // le seul exemplaire part

            var other = SeedMember(MembershipType.Standard);
            Assert.Throws<BookUnavailableException>(
                () => _service.BorrowBook(other.Id, "ISBN-1"));
        }

        [Test]
        public void Borrow_throws_when_member_reaches_quota()
        {
            var member = SeedMember(MembershipType.Standard); // quota = 3
            for (var i = 1; i <= 4; i++)
                SeedBook($"ISBN-{i}", copies: 1);

            _service.BorrowBook(member.Id, "ISBN-1");
            _service.BorrowBook(member.Id, "ISBN-2");
            _service.BorrowBook(member.Id, "ISBN-3");

            Assert.Throws<LoanQuotaExceededException>(
                () => _service.BorrowBook(member.Id, "ISBN-4"));
        }

        [Test]
        public void Student_quota_allows_five_simultaneous_loans()
        {
            var member = SeedMember(MembershipType.Student); // quota = 5
            for (var i = 1; i <= 5; i++)
                SeedBook($"ISBN-{i}", copies: 1);

            for (var i = 1; i <= 5; i++)
                Assert.DoesNotThrow(() => _service.BorrowBook(member.Id, $"ISBN-{i}"));

            Assert.That(_service.GetActiveLoansOf(member.Id), Has.Count.EqualTo(5));
        }

        [Test]
        public void Borrow_throws_when_member_unknown()
        {
            SeedBook("ISBN-1", copies: 1);

            Assert.Throws<MemberNotFoundException>(
                () => _service.BorrowBook(Guid.NewGuid(), "ISBN-1"));
        }

        [Test]
        public void Borrow_throws_when_book_unknown()
        {
            var member = SeedMember(MembershipType.Standard);

            Assert.Throws<BookNotFoundException>(
                () => _service.BorrowBook(member.Id, "UNKNOWN"));
        }

        [Test]
        public void Return_frees_the_copy_again()
        {
            var member = SeedMember(MembershipType.Standard);
            SeedBook("ISBN-1", copies: 1);
            var loan = _service.BorrowBook(member.Id, "ISBN-1");

            var penalty = _service.ReturnCopy(loan.CopyId);

            Assert.That(penalty.IsNone, Is.True); // rendu le jour même
            Assert.That(_service.CountAvailableCopies("ISBN-1"), Is.EqualTo(1));
            Assert.That(_service.GetActiveLoansOf(member.Id), Is.Empty);
        }

        [Test]
        public void Return_unknown_copy_throws()
        {
            Assert.Throws<ActiveLoanNotFoundException>(
                () => _service.ReturnCopy("does-not-exist"));
        }
    }
}
