# LibraryManagementSystem

Système de gestion des emprunts d'une bibliothèque municipale (kata technique, C#).

## Architecture

Solution découpée par responsabilités :

- **Domain** — entités, value objects et règles métier (politique de prêt, calcul des pénalités).
- **Application** — orchestration des cas d'usage (`LibraryService`).
- **Infrastructure** — implémentations de persistance (repositories in-memory).
- **Presentation** — programme de démonstration déroulant les scénarios.
- **Tests** — suite de tests unitaires (Domain + cas d'usage).

## Lancer la démo

```bash
dotnet run --project LMS/LibraryManagement.Presentation
```

## Lancer les tests

```bash
dotnet test LMS/LMS.sln
```

## Hypothèses métier

Le besoin laisse plusieurs règles implicites. Les choix retenus :

- **Semaines = jours calendaires** : durée standard = 21 jours, étudiant = 28 jours (pas d'exclusion des week-ends/fériés).
- **Identité d'un ouvrage** : un ISBN identifie un titre de façon unique ; réenregistrer le même ISBN est refusé.
- **Au moins un exemplaire** : un ouvrage doit être enregistré avec un nombre d'exemplaires ≥ 1.
- **Un titre par adhérent** : un adhérent ne peut pas détenir simultanément deux exemplaires du même titre.
- **Exemplaire physique assigné** : l'emprunt réserve un exemplaire précis, libéré au retour.
- **Retard** : nombre de jours strictement postérieurs à l'échéance ; un retour le jour même de l'échéance n'est pas en retard.
- **Pénalité** : 0,20 € par jour de retard, plafonnée à 10 €, calculée au moment du retour.
- **« Pénalités en cours »** : cumul des pénalités constatées aux retours en retard, agrégé par adhérent.
