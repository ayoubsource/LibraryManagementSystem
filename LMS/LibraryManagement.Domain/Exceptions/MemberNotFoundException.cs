namespace LibraryManagement.Domain.Exceptions
{
    public sealed class MemberNotFoundException : DomainException
    {
        public MemberNotFoundException(Guid memberId) : base($"Membre introuvable : {memberId}.")
        {
        }
    }
}

