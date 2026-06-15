using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Policies
{
    public sealed class LoanPolicy
    {
        public int MaxLoansFor(MembershipType type) => type switch
        {
            MembershipType.Standard => 3,
            MembershipType.Student => 5,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        public int LoanDurationInDaysFor(MembershipType type) => type switch
        {
            MembershipType.Standard => 21,
            MembershipType.Student => 28,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}

