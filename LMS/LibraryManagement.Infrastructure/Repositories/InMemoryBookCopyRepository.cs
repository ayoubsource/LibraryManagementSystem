using LibraryManagement.Domain.Abstractions;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Repositories
{
    public sealed class InMemoryBookCopyRepository : IBookCopyRepository
    {
        private readonly Dictionary<string, BookCopy> _copies = new Dictionary<string, BookCopy>();
        public BookCopy? GetById(string copyId) => _copies.GetValueOrDefault(copyId);
        public BookCopy? GetFirstAvailableCopy(string isbn) => _copies.Values.FirstOrDefault(c => c.BookId == isbn && c.IsAvailable);
        public int CountAvailableCopies(string isbn) => _copies.Values.Count(c => c.BookId == isbn && c.IsAvailable);
        public void Add(BookCopy copy) => _copies[copy.Id] = copy;
        public void Update(BookCopy copy) { /* objets en mémoire : rien à faire */ }
    }
}

