using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Abstractions
{
	public interface IBookCopyRepository
	{
        BookCopy? GetById(string copyId);
        BookCopy? GetFirstAvailableCopy(string isbn);
        int CountAvailableCopies(string isbn);
        void Add(BookCopy copy);
        void Update(BookCopy copy);
    }
}

