namespace LibraryManagement.Domain.Exceptions
{
	public sealed class CopyUnavailableException : DomainException
	{
		public CopyUnavailableException(string copyId) : base($"L'exemplaire {copyId} n'est pas disponible.")
		{
		}
	}
}