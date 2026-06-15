namespace LibraryManagement.Domain.Entities
{
    public sealed class Book
    {
        public string Id { get; }
        public string Title { get; }
        public string Author { get; }
        public Book(string id, string title, string author)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("L'ISBN est obligatoire.", nameof(id));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Le titre est obligatoire.", nameof(title));
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("L'auteur est obligatoire.", nameof(author));
            Id = id;
            Title = title;
            Author = author;
        }
    }
}

