using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Policies;
using NUnit.Framework;

namespace LibrartManagement.Tests.Domain
{
    [TestFixture]
    public sealed class LoanPolicyTests
    {
        private readonly LoanPolicy _policy = new();

        [TestCase(MembershipType.Standard, 3)]
        [TestCase(MembershipType.Student, 5)]
        public void MaxLoansFor_returns_quota_per_profile(MembershipType type, int expected)
        {
            Assert.That(_policy.MaxLoansFor(type), Is.EqualTo(expected));
        }

        [TestCase(MembershipType.Standard, 21)] // 3 semaines
        [TestCase(MembershipType.Student, 28)]  // 4 semaines
        public void LoanDurationInDaysFor_returns_duration_per_profile(MembershipType type, int expected)
        {
            Assert.That(_policy.LoanDurationInDaysFor(type), Is.EqualTo(expected));
        }
    }
}
