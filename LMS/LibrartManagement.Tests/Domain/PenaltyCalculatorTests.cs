using LibraryManagement.Domain.Policies;
using NUnit.Framework;

namespace LibrartManagement.Tests.Domain
{
    [TestFixture]
    public sealed class PenaltyCalculatorTests
    {
        private readonly PenaltyCalculator _calculator = new();

        [TestCase(0)]
        [TestCase(-3)]
        public void No_penalty_when_not_late(int lateDays)
        {
            var penalty = _calculator.Calculate(lateDays);

            Assert.That(penalty.IsNone, Is.True);
            Assert.That(penalty.Amount.Amount, Is.EqualTo(0m));
        }

        [TestCase(1, 0.20)]
        [TestCase(5, 1.00)]
        [TestCase(49, 9.80)]
        public void Penalty_is_20_cents_per_late_day(int lateDays, decimal expected)
        {
            var penalty = _calculator.Calculate(lateDays);

            Assert.That(penalty.LateDays, Is.EqualTo(lateDays));
            Assert.That(penalty.Amount.Amount, Is.EqualTo(expected));
        }

        [TestCase(50)]  // 50 * 0.20 = 10.00 -> plafond atteint
        [TestCase(100)] // au-delà : reste plafonné
        public void Penalty_is_capped_at_10_euros(int lateDays)
        {
            var penalty = _calculator.Calculate(lateDays);

            Assert.That(penalty.Amount.Amount, Is.EqualTo(10.00m));
        }
    }
}
