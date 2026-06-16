using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities
{
    public sealed class Member
    {
        public Guid Id { get; }
        public string Name { get; }
        public MembershipType Membership { get; }
        public Member(Guid id, string name, MembershipType membership)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Le nom est obligatoire.", nameof(name));
            Id = id;
            Name = name;
            Membership = membership;
        }
    }
}
