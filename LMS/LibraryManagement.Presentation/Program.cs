using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Abstractions;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Policies;
using LibraryManagement.Domain.ValueObjects;
using LibraryManagement.Infrastructure.Repositories;

IBookRepository books = new InMemoryBookRepository();
IBookCopyRepository copies = new InMemoryBookCopyRepository();
IMemberRepository members = new InMemoryMemberRepository();
ILoanRepository loans = new InMemoryLoanRepository();


var clock = new DemoClock(new DateOnly(2026, 1, 5));

var library = new LibraryService(
    books, copies, members, loans,
    new LoanPolicy(),
    new PenaltyCalculator(),
    clock);

// =====================================================================
// Scénario 1 : catalogage du fonds documentaire
// =====================================================================
PrintHeading("Scénario 1 — Catalogage des ouvrages");

var etranger = library.RegisterBook("978-2070360024", "L'Étranger", "Albert Camus", copies: 2);
var petitPrince = library.RegisterBook("978-2070408504", "Le Petit Prince", "Antoine de Saint-Exupéry", copies: 1);
var fondation = library.RegisterBook("978-2070415717", "Fondation", "Isaac Asimov", copies: 4);
var dune = library.RegisterBook("978-2266320481", "Dune", "Frank Herbert", copies: 2);
var orwell = library.RegisterBook("978-2070368228", "1984", "George Orwell", copies: 2);
var germinal = library.RegisterBook("978-2070413423", "Germinal", "Émile Zola", copies: 1);

foreach (var b in new[] { etranger, petitPrince, fondation, dune, orwell, germinal })
    Console.WriteLine($"  {b.Id} — « {b.Title} » de {b.Author} : {library.CountAvailableCopies(b.Id)} exemplaire(s)");

ExpectFailure("Ré-enregistrement du même ISBN",
    () => library.RegisterBook("978-2070360024", "L'Étranger", "Albert Camus", copies: 1));

ExpectFailure("Enregistrement d'un ouvrage sans exemplaire",
    () => library.RegisterBook("978-0000000000", "Fantôme", "Personne", copies: 0));

// =====================================================================
// Scénario 2 : inscription des adhérents
// =====================================================================
PrintHeading("Scénario 2 — Inscription des adhérents");

var alice = RegisterMember("Alice", MembershipType.Standard);   // 3 prêts max, 21 jours
var bob = RegisterMember("Bob", MembershipType.Student);        // 5 prêts max, 28 jours
var chloe = RegisterMember("Chloé", MembershipType.Standard);

// =====================================================================
// Scénario 3 : emprunt puis retour dans les temps (aucune pénalité)
// =====================================================================
PrintHeading("Scénario 3 — Emprunt et retour dans les temps");

var pretAlice = library.BorrowBook(alice.Id, etranger.Id);
Console.WriteLine($"  {clock.Today:yyyy-MM-dd} : Alice emprunte « {etranger.Title} » " +
                  $"(exemplaire {pretAlice.CopyId}), à rendre le {pretAlice.DueDate:yyyy-MM-dd}.");
Console.WriteLine($"  Exemplaires disponibles : {library.CountAvailableCopies(etranger.Id)}/2");

clock.AdvanceTo(new DateOnly(2026, 1, 20));
ReturnLoan(alice, pretAlice, "retour anticipé");
Console.WriteLine($"  Exemplaires disponibles après retour : {library.CountAvailableCopies(etranger.Id)}/2");

// =====================================================================
// Scénario 4 : retour en retard (pénalité 0,20 € / jour)
// =====================================================================
PrintHeading("Scénario 4 — Retour en retard d'un étudiant");

var pretBob = library.BorrowBook(bob.Id, petitPrince.Id);
Console.WriteLine($"  {clock.Today:yyyy-MM-dd} : Bob (Student, 28 jours) emprunte « {petitPrince.Title} », " +
                  $"échéance {pretBob.DueDate:yyyy-MM-dd}.");

clock.AdvanceTo(new DateOnly(2026, 3, 1));
ReturnLoan(bob, pretBob, "12 jours de retard attendus");

// =====================================================================
// Scénario 5 : retard extrême — la pénalité est plafonnée à 10 €
// =====================================================================
PrintHeading("Scénario 5 — Plafonnement de la pénalité");

var pretRetard = library.BorrowBook(alice.Id, fondation.Id);
Console.WriteLine($"  {clock.Today:yyyy-MM-dd} : Alice emprunte « {fondation.Title} », " +
                  $"échéance {pretRetard.DueDate:yyyy-MM-dd}.");

clock.AdvanceTo(pretRetard.DueDate.AddDays(100));
ReturnLoan(alice, pretRetard, "100 jours de retard → plafond 10,00 €");

// =====================================================================
// Scénario 6 : quota de prêts d'un adhérent Standard (3 maximum)
// =====================================================================
PrintHeading("Scénario 6 — Quota de prêts atteint");

foreach (var titre in new[] { etranger, dune, orwell })
{
    var pret = library.BorrowBook(alice.Id, titre.Id);
    Console.WriteLine($"  Alice emprunte « {titre.Title} » ({pret.CopyId}).");
}

Console.WriteLine($"  Prêts actifs d'Alice : {library.GetActiveLoansOf(alice.Id).Count}/3");

ExpectFailure("Alice tente un 4ᵉ emprunt",
    () => library.BorrowBook(alice.Id, germinal.Id));

// =====================================================================
// Scénario 7 : un même adhérent ne peut pas emprunter deux fois le même titre
// =====================================================================
PrintHeading("Scénario 7 — Double emprunt du même ouvrage");

ExpectFailure("Alice réemprunte « L'Étranger » qu'elle détient déjà",
    () => library.BorrowBook(alice.Id, etranger.Id));

// =====================================================================
// Scénario 8 : plus aucun exemplaire disponible
// =====================================================================
PrintHeading("Scénario 8 — Ouvrage indisponible");

var pretChloe = library.BorrowBook(chloe.Id, petitPrince.Id);
Console.WriteLine($"  Chloé emprunte le dernier exemplaire de « {petitPrince.Title} » ({pretChloe.CopyId}).");
Console.WriteLine($"  Exemplaires disponibles : {library.CountAvailableCopies(petitPrince.Id)}/1");

ExpectFailure("Bob demande « Le Petit Prince »",
    () => library.BorrowBook(bob.Id, petitPrince.Id));

// =====================================================================
// Scénario 9 : références inconnues et retours incohérents
// =====================================================================
PrintHeading("Scénario 9 — Cas d'erreur sur des références inconnues");

ExpectFailure("Emprunt par un adhérent inexistant",
    () => library.BorrowBook(Guid.NewGuid(), etranger.Id));

ExpectFailure("Emprunt d'un ISBN inexistant",
    () => library.BorrowBook(alice.Id, "000-0000000000"));

ExpectFailure("Consultation du stock d'un ISBN inexistant",
    () => library.CountAvailableCopies("000-0000000000"));

ExpectFailure("Retour d'un exemplaire qui n'est pas en prêt",
    () => library.ReturnCopy($"{etranger.Id}-002"));

ExpectFailure("Retour d'un code-barres inconnu",
    () => library.ReturnCopy("CODE-INCONNU"));

// =====================================================================
// Scénario 10 : état des prêts en cours
// =====================================================================
PrintHeading("Scénario 10 — Prêts en cours par adhérent");

// On laisse filer le temps : les prêts encore ouverts deviennent des retards.
clock.AdvanceTo(new DateOnly(2026, 8, 15));
Console.WriteLine($"  Nous sommes le {clock.Today:yyyy-MM-dd}.");

foreach (var m in new[] { alice, bob, chloe })
{
    var actifs = library.GetActiveLoansOf(m.Id);
    Console.WriteLine($"  {m.Name} ({m.Membership}) : {actifs.Count} prêt(s) actif(s)");

    foreach (var l in actifs)
    {
        var retard = clock.Today.DayNumber - l.DueDate.DayNumber;
        var etat = retard > 0 ? $"EN RETARD de {retard} jour(s)" : $"dans les temps";
        Console.WriteLine($"      {l.CopyId} — échéance {l.DueDate:yyyy-MM-dd} ({etat})");
    }
}

// =====================================================================
// Scénario 11 : registre des pénalités accumulées depuis le démarrage
// =====================================================================
PrintHeading("Scénario 11 — Registre des pénalités");

foreach (var ligne in PenaltyLedger.History)
    Console.WriteLine($"  {ligne}");

Console.WriteLine();
foreach (var m in new[] { alice, bob, chloe })
    Console.WriteLine($"  Total dû par {m.Name} : {PenaltyLedger.TotalFor(m.Id)}");

Console.WriteLine($"  ─────────────────────────────");
Console.WriteLine($"  Total encaissé par la bibliothèque : {PenaltyLedger.Total}");

// ---- Fonctions utilitaires de la démo ----

Member RegisterMember(string nom, MembershipType type)
{
    var m = new Member(Guid.NewGuid(), nom, type);
    members.Add(m);
    Console.WriteLine($"  {m.Name} — {m.Membership} : {new LoanPolicy().MaxLoansFor(type)} prêts max, " +
                      $"{new LoanPolicy().LoanDurationInDaysFor(type)} jours par prêt");
    return m;
}

void ReturnLoan(Member membre, Loan pret, string commentaire)
{
    var penalite = library.ReturnCopy(pret.CopyId);
    PenaltyLedger.Record(membre, pret, penalite, clock.Today);

    Console.WriteLine(penalite.IsNone
        ? $"  {clock.Today:yyyy-MM-dd} : {membre.Name} rend {pret.CopyId} — aucune pénalité ({commentaire})."
        : $"  {clock.Today:yyyy-MM-dd} : {membre.Name} rend {pret.CopyId} — " +
          $"{penalite.LateDays} jour(s) de retard, pénalité {penalite.Amount} ({commentaire}).");
}

void ExpectFailure(string libelle, Action action)
{
    try
    {
        action();
        Console.WriteLine($"  [!] {libelle} : accepté alors qu'on attendait un refus.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  [x] {libelle} : {ex.GetType().Name} — {FirstLine(ex.Message)}");
    }
}

static string FirstLine(string message) => message.Split('\n')[0].Trim();

void PrintHeading(string texte)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine(texte);
    Console.WriteLine(new string('=', 70));
}


internal static class PenaltyLedger
{
    private static readonly Dictionary<Guid, decimal> TotalsByMember = new();
    private static readonly List<string> Lines = new();

    public static IReadOnlyList<string> History => Lines;

    public static Money Total => new(TotalsByMember.Values.Sum());

    public static Money TotalFor(Guid memberId) => new(TotalsByMember.GetValueOrDefault(memberId));

    public static void Record(Member membre, Loan pret, Penalty penalite, DateOnly date)
    {
        if (penalite.IsNone)
        {
            Lines.Add($"{date:yyyy-MM-dd} — {membre.Name} / {pret.CopyId} : à l'heure, rien à payer");
            return;
        }

        TotalsByMember[membre.Id] = TotalsByMember.GetValueOrDefault(membre.Id) + penalite.Amount.Amount;
        Lines.Add($"{date:yyyy-MM-dd} — {membre.Name} / {pret.CopyId} : " +
                   $"{penalite.LateDays} jour(s) de retard → {penalite.Amount}");
    }
}


internal sealed class DemoClock : IClock
{
    public DemoClock(DateOnly depart) => Today = depart;

    public DateOnly Today { get; private set; }

    public void AdvanceTo(DateOnly date)
    {
        if (date < Today)
            throw new ArgumentOutOfRangeException(nameof(date), "L'horloge de démo n'avance que vers le futur.");

        Today = date;
    }
}
