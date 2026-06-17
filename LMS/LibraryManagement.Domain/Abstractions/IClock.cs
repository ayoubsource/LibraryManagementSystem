namespace LibraryManagement.Domain.Abstractions
{
	public interface IClock
	{
        DateOnly Today { get; }
    }
}

