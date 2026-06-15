using LibraryManagement.Domain.Entities;
using NUnit.Framework;

namespace LibrartManagement.Tests.Domain
{
    [TestFixture]
    public sealed class BookTests
    {
        [Test]
        public void Valid_book_exposes_its_fields()
        {
            var book = new Book("ISBN-1", "L'Étranger", "Albert Camus");

            Assert.That(book.Id, Is.EqualTo("ISBN-1"));
            Assert.That(book.Title, Is.EqualTo("L'Étranger"));
            Assert.That(book.Author, Is.EqualTo("Albert Camus"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Missing_isbn_throws(string? isbn)
        {
            Assert.Throws<ArgumentException>(() => new Book(isbn!, "Titre", "Auteur"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Missing_title_throws(string? title)
        {
            Assert.Throws<ArgumentException>(() => new Book("ISBN-1", title!, "Auteur"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Missing_author_throws(string? author)
        {
            Assert.Throws<ArgumentException>(() => new Book("ISBN-1", "Titre", author!));
        }
    }
}
