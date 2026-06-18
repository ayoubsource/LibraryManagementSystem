using LibraryManagement.Domain.Abstractions;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Exceptions;
using LibraryManagement.Domain.Policies;
using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Application.Services;

public sealed class LibraryService
{
    private readonly IBookRepository _books;
    private readonly IBookCopyRepository _copies;
    private readonly IMemberRepository _members;
    private readonly ILoanRepository _loans;
    private readonly LoanPolicy _policy;
    private readonly PenaltyCalculator _penaltyCalculator;
    private readonly IClock _clock;

    public LibraryService(
        IBookRepository books,
        IBookCopyRepository copies,
        IMemberRepository members,
        ILoanRepository loans,
        LoanPolicy policy,
        PenaltyCalculator penaltyCalculator,
        IClock clock)
    {
        _books = books;
        _copies = copies;
        _members = members;
        _loans = loans;
        _policy = policy;
        _penaltyCalculator = penaltyCalculator;
        _clock = clock;
    }


    public Book RegisterBook(string isbn, string title, string author, int copies)
    {
        if (copies < 1)
            throw new ArgumentOutOfRangeException(
                nameof(copies), "Un ouvrage doit avoir au moins un exemplaire.");

        if (_books.GetById(isbn) is not null)
            throw new BookAlreadyExistsException(isbn);

        var book = new Book(isbn, title, author);
        _books.Add(book);

        for (var i = 1; i <= copies; i++)
            _copies.Add(new BookCopy($"{isbn}-{i:D3}", isbn));

        return book;
    }

    public Loan BorrowBook(Guid memberId, string isbn)
    {
        var member = _members.GetById(memberId)
            ?? throw new MemberNotFoundException(memberId);

        _ = _books.GetById(isbn)
            ?? throw new BookNotFoundException(isbn);

        var activeLoans = _loans.GetActiveLoansOf(memberId);

        if (activeLoans.Any(l => l.BookId == isbn))
            throw new BookAlreadyBorrowedException(memberId, isbn);

        var maxLoans = _policy.MaxLoansFor(member.Membership);

        if (activeLoans.Count >= maxLoans)
            throw new LoanQuotaExceededException(memberId, maxLoans);

        var copy = _copies.GetFirstAvailableCopy(isbn)
            ?? throw new BookUnavailableException(isbn);

        copy.MarkOnLoan();

        var loan = Loan.Create(member, copy, _clock.Today, _policy);

        _loans.Add(loan);
        _copies.Update(copy);

        return loan;
    }

    public Penalty ReturnCopy(string copyId)
    {
        var loan = _loans.GetActiveLoanByCopy(copyId)
            ?? throw new ActiveLoanNotFoundException(copyId);

        var penalty = loan.Return(_clock.Today, _penaltyCalculator);

        var copy = _copies.GetById(copyId)
            ?? throw new CopyNotFoundException(copyId);

        copy.MarkReturned();

        _loans.Update(loan);
        _copies.Update(copy);

        return penalty;
    }

    public int CountAvailableCopies(string isbn)
    {
        _ = _books.GetById(isbn)
            ?? throw new BookNotFoundException(isbn);

        return _copies.CountAvailableCopies(isbn);
    }

    public IReadOnlyList<Loan> GetActiveLoansOf(Guid memberId)
    {
        _ = _members.GetById(memberId)
            ?? throw new MemberNotFoundException(memberId);

        return _loans.GetActiveLoansOf(memberId);
    }
}