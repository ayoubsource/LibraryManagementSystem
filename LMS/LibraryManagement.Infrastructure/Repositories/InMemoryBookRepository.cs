using LibraryManagement.Domain.Abstractions;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Repositories
{
    public sealed class InMemoryBookRepository : IBookRepository
    {
        private readonly Dictionary<string, Book> _books = new();

        public Book? GetById(string isbn) => _books.GetValueOrDefault(isbn);

        public void Add(Book book) => _books[book.Id] = book;
    }
}
