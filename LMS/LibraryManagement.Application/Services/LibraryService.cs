using LibraryManagement.Domain.Abstractions;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Exceptions;
using LibraryManagement.Domain.Policies;

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

    public int CountAvailableCopies(string isbn)
    {
        _ = _books.GetById(isbn)
            ?? throw new BookNotFoundException(isbn);

        return _copies.CountAvailableCopies(isbn);
    }
}
