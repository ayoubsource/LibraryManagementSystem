using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;

namespace LibraryManagement.Domain.Entities
{
	public sealed class BookCopy
	{
        public string Id { get; } 
        public string BookId { get; }
        public CopyStatus Status { get; private set; }
        public bool IsAvailable => Status == CopyStatus.Available;


        public BookCopy(string id, string bookId, CopyStatus status = CopyStatus.Available)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Le code-barres est obligatoire.", nameof(id));
            if (string.IsNullOrWhiteSpace(bookId))
                throw new ArgumentException("L'ISBN est obligatoire.", nameof(bookId));
            Id = id;
            BookId = bookId;
            Status = status;
        }

        public void MarkOnLoan()
        {
            if (!IsAvailable) throw new CopyUnavailableException(Id);
            Status = CopyStatus.OnLoan;
        }

        public void MarkReturned()
        {
            if (Status != CopyStatus.OnLoan)
                throw new InvalidOperationException(
                $"Incohérence : l'exemplaire {Id} n'est pas en prêt.");
            Status = CopyStatus.Available;
        }
    }
}

