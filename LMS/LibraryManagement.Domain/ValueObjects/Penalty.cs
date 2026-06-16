namespace LibraryManagement.Domain.ValueObjects
{
    public sealed record Penalty(int LateDays, Money Amount)
    {
        public static readonly Penalty None = new(0, Money.Zero);
        public bool IsNone => Amount == Money.Zero;
    }
}

