using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Abstractions;
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
    }
}
