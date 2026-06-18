using LibraryManagement.Domain.Abstractions;

namespace LibrartManagement.Tests.TestDoubles
{
    /// <summary>Horloge déterministe pour rendre les tests reproductibles.</summary>
    public sealed class FixedClock : IClock
    {
        public FixedClock(DateOnly today) => Today = today;

        public DateOnly Today { get; }
    }
}
