using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Domain.Policies
{
    public sealed class PenaltyCalculator
    {
        private static readonly Money DailyRate = new(0.20m);
        private static readonly Money MaxPenalty = new(10.00m);

        public Penalty Calculate(int lateDays)
        {
            if (lateDays <= 0) return Penalty.None;
            var amount = Money.Min(DailyRate.Times(lateDays), MaxPenalty);
            return new Penalty(lateDays, amount);
        }
    }
}

