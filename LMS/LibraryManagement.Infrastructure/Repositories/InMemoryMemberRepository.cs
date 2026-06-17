using LibraryManagement.Domain.Abstractions;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Repositories
{
    public sealed class InMemoryMemberRepository : IMemberRepository
    {
        private readonly Dictionary<Guid, Member> _members = new();

        public Member? GetById(Guid id) => _members.GetValueOrDefault(id);

        public void Add(Member member) => _members[member.Id] = member;
    }
}
