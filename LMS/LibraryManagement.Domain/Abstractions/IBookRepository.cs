using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Abstractions
{
	public interface IBookRepository
	{
        Book? GetById(string isbn);
        void Add(Book book);
    }
}

