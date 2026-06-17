using LibraryManagement.Domain.Abstractions;

namespace LibraryManagement.Infrastructure.Time
{
    public sealed class SystemClock : IClock
    {
        public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    }
}

