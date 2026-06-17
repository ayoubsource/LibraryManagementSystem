using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Abstractions
{
	public interface IMemberRepository
	{
        Member? GetById(Guid id);
        void Add(Member member);
    }
}

