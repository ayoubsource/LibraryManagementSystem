namespace LibraryManagement.Domain.ValueObjects
{
    public sealed record Money
    {
        public decimal Amount { get; }
        public Money(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(
                nameof(amount), "Un montant ne peut pas être négatif.");
            Amount = decimal.Round(amount, 2);
        }
        public static readonly Money Zero = new(0m);
        public Money Times(int factor) => new(Amount * factor);
        public static Money Min(Money a, Money b) => a.Amount <= b.Amount ? a : b;
        public override string ToString() => $"{Amount:0.00} €";
    }
}
