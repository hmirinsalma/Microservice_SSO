using Microsoft.EntityFrameworkCore;
using ONEE.EAMS.Domain.Entities;
using ONEE.EAMS.Domain.Enums;
using ONEE.EAMS.Infrastructure.Data;

namespace ONEE.EAMS.Infrastructure.Seed;

/// <summary>
/// Initialise les données de démonstration de l'application EAMS.
///
/// Note SSO : Les utilisateurs sont créés sans mot de passe (PasswordHash supprimé).
/// L'authentification sera gérée par le microservice SSO.
/// Le champ SsoId est laissé null — il sera renseigné lors de la liaison avec le SSO.
/// </summary>
public class DataSeeder
{
    private readonly AppDbContext _ctx;
    private static readonly Random _rng = new(42);

    public DataSeeder(AppDbContext ctx) => _ctx = ctx;

    public async Task SeedAsync()
    {
        if (await _ctx.Users.AnyAsync()) return; // déjà seedé

        // ── Services ───────────────────────────────────────────────────────────
        var services = new[]
        {
            new ServiceEntity { Id = Guid.NewGuid(), Nom = "Direction Technique",         Code = "DT" },
            new ServiceEntity { Id = Guid.NewGuid(), Nom = "Distribution Électrique",     Code = "DE" },
            new ServiceEntity { Id = Guid.NewGuid(), Nom = "Production & Énergie",        Code = "PE" },
            new ServiceEntity { Id = Guid.NewGuid(), Nom = "Réseau & Télécommunications", Code = "RT" },
            new ServiceEntity { Id = Guid.NewGuid(), Nom = "Maintenance Générale",        Code = "MG" }
        };
        _ctx.Services.AddRange(services);

        // ── Catégories ─────────────────────────────────────────────────────────
        var categories = new[]
        {
            Cat("Transformateur",    "TRF", "ElectricMeter",       "#1565C0"),
            Cat("Compteur",          "CPT", "Speed",               "#2E7D32"),
            Cat("Véhicule",          "VEH", "DirectionsCar",       "#E65100"),
            Cat("Pompe",             "PMP", "Water",               "#00838F"),
            Cat("Poste électrique",  "PST", "Bolt",                "#6A1B9A"),
            Cat("Groupe électrogène","GEG", "Power",               "#558B2F"),
            Cat("Tableau électrique","TAB", "DeveloperBoard",      "#4527A0"),
            Cat("Câble HT/BT",       "CAB", "Cable",               "#37474F"),
            Cat("Onduleur",          "OND", "BatteryChargingFull", "#0277BD"),
            Cat("Armoire électrique","ARM", "Kitchen",             "#283593"),
            Cat("Éclairage public",  "ECL", "Lightbulb",           "#F9A825"),
            Cat("Capteur",           "CAP", "Sensors",             "#00695C"),
            Cat("Serveur",           "SRV", "Storage",             "#37474F"),
            Cat("Switch réseau",     "SWT", "Router",              "#1B5E20"),
        };
        _ctx.Categories.AddRange(categories);

        // ── Utilisateurs (sans mot de passe — SSO) ─────────────────────────────
        var admin     = MakeUser("Ahmed",  "Benali", "admin@onee.ma",      "Administrateur", UserRole.Admin_Patrimoine, null);
        var directeur = MakeUser("Fatima", "Zahra",  "directeur@onee.ma",  "Directeur",      UserRole.Directeur,        null);

        var chefs = services.Select((s, i) => MakeUser(
            ChefNoms[i], ChefPrenoms[i],
            $"chef.{s.Code.ToLower()}@onee.ma",
            "Chef de Service", UserRole.Chef_de_Service, s.Id)).ToList();

        var techniciens = Enumerable.Range(0, 20).Select(i => MakeUser(
            TechNoms[i % TechNoms.Length],
            TechPrenoms[i % TechPrenoms.Length],
            $"tech{i + 1:D2}@onee.ma",
            "Technicien de Maintenance", UserRole.Technicien,
            services[i % services.Length].Id)).ToList();

        _ctx.Users.Add(admin);
        _ctx.Users.Add(directeur);
        _ctx.Users.AddRange(chefs);
        _ctx.Users.AddRange(techniciens);
        await _ctx.SaveChangesAsync();

        // ── Équipements ────────────────────────────────────────────────────────
        var equipements = new List<Equipement>();
        int seq = 1;
        for (int i = 0; i < 300; i++)
        {
            var cat         = categories[i % categories.Length];
            var svc         = services[i % services.Length];
            var responsable = chefs[i % chefs.Count];
            var installDate = DateTime.UtcNow.AddDays(-_rng.Next(30, 1500));
            var etat        = EtatsPool[i % EtatsPool.Length];

            equipements.Add(new Equipement
            {
                Id               = Guid.NewGuid(),
                Reference        = $"{cat.Code}-{DateTime.UtcNow.Year}-{seq++:D5}",
                Nom              = $"{cat.Nom} {Marques[i % Marques.Length]} #{i + 1}",
                CategorieId      = cat.Id,
                Type             = Types[i % Types.Length],
                Marque           = Marques[i % Marques.Length],
                Modele           = $"MOD-{(char)('A' + i % 26)}{i % 100:D2}",
                NumeroSerie      = $"SN-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                Localisation     = Localisations[i % Localisations.Length],
                ServiceId        = svc.Id,
                ResponsableId    = responsable.Id,
                DateInstallation = installDate,
                DateMiseEnService = installDate.AddDays(_rng.Next(1, 30)),
                Etat             = etat,
                DateFinGarantie  = installDate.AddYears(3),
                ValeurAcquisition = _rng.Next(5000, 500000),
                Fournisseur      = Fournisseurs[i % Fournisseurs.Length],
                Description      = $"Équipement {cat.Nom} de type {Types[i % Types.Length]} installé à {Localisations[i % Localisations.Length]}.",
                CreatedAt        = installDate,
                UpdatedAt        = installDate
            });
        }
        _ctx.Equipements.AddRange(equipements);
        await _ctx.SaveChangesAsync();

        // ── Affectations techniciens ───────────────────────────────────────────
        var affectations = equipements.Select((eq, i) => new TechnicienEquipement
        {
            TechnicienId = techniciens[i % techniciens.Count].Id,
            EquipementId = eq.Id,
            AffectedAt   = DateTime.UtcNow.AddDays(-_rng.Next(1, 365)),
            AffectedById = admin.Id
        }).ToList();
        _ctx.TechnicienEquipements.AddRange(affectations);
        await _ctx.SaveChangesAsync();

        // ── Maintenances ───────────────────────────────────────────────────────
        var maintenances = new List<Maintenance>();
        for (int i = 0; i < 800; i++)
        {
            var eq           = equipements[i % equipements.Count];
            var tech         = techniciens[i % techniciens.Count];
            var daysOffset   = _rng.Next(-400, 120);
            var datePlanifiee = DateTime.UtcNow.AddDays(daysOffset);
            var isPast       = daysOffset < 0;
            var statut       = isPast
                ? (i % 5 == 0 ? MaintenanceStatut.En_retard : MaintenanceStatut.Terminee)
                : (i % 10 == 0 ? MaintenanceStatut.Annulee : MaintenanceStatut.Planifiee);

            maintenances.Add(new Maintenance
            {
                Id                  = Guid.NewGuid(),
                EquipementId        = eq.Id,
                TechnicienId        = tech.Id,
                Type                = (MaintenanceType)(i % 3),
                Statut              = statut,
                DatePlanifiee       = datePlanifiee,
                DateDebut           = isPast ? datePlanifiee : null,
                DateCloture         = statut == MaintenanceStatut.Terminee ? datePlanifiee.AddHours(_rng.Next(1, 8)) : null,
                DureeMinutes        = isPast ? _rng.Next(30, 480) : null,
                EtatAvant           = isPast ? EquipementEtat.En_maintenance : null,
                EtatApres           = statut == MaintenanceStatut.Terminee ? EquipementEtat.Disponible : null,
                Observations        = isPast ? ObservationsPool[i % ObservationsPool.Length] : null,
                PiecesRemplacees    = statut == MaintenanceStatut.Terminee ? PiecesPool[i % PiecesPool.Length] : null,
                CoutEstime          = _rng.Next(500, 50000),
                CoutReel            = statut == MaintenanceStatut.Terminee ? _rng.Next(400, 55000) : null,
                ProchaineMaintenance = statut == MaintenanceStatut.Terminee ? datePlanifiee.AddMonths(6) : null,
                CreatedAt           = datePlanifiee.AddDays(-5),
                UpdatedAt           = datePlanifiee
            });
        }
        _ctx.Maintenances.AddRange(maintenances);
        await _ctx.SaveChangesAsync();

        // ── Historique de création des équipements ─────────────────────────────
        var historique = equipements.Select(e => new HistoriqueEntry
        {
            Id             = Guid.NewGuid(),
            EntiteId       = e.Id,
            EntiteType     = "Equipement",
            TypeEvenement  = "Creation",
            ValeurAvant    = null,
            ValeurApres    = $"{{\"nom\":\"{e.Nom}\",\"reference\":\"{e.Reference}\"}}",
            AuteurId       = admin.Id,
            HorodatageUtc  = e.CreatedAt
        }).ToList();
        _ctx.HistoriqueEntries.AddRange(historique);
        await _ctx.SaveChangesAsync();
    }

    // ── Factory helpers ────────────────────────────────────────────────────────

    private static Categorie Cat(string nom, string code, string icone, string couleur) => new()
    {
        Id = Guid.NewGuid(), Nom = nom, Code = code, Icone = icone, Couleur = couleur,
        Description = $"Catégorie {nom} du patrimoine technique ONEE."
    };

    /// <summary>
    /// Crée un utilisateur métier SANS mot de passe.
    /// Le SsoId sera renseigné lors de la liaison avec le microservice SSO.
    /// </summary>
    private static User MakeUser(string nom, string prenom, string email, string poste,
        UserRole role, Guid? serviceId) => new()
    {
        Id         = Guid.NewGuid(),
        SsoId      = null,
        Nom        = nom,
        Prenom     = prenom,
        Email      = email,
        Telephone  = $"+212 6{_rng.Next(10000000, 99999999)}",
        Poste      = poste,
        RoleMetier = role,   // données métier — pas utilisé pour l'autorisation
        ServiceId  = serviceId,
        IsActive   = true,
        CreatedAt  = DateTime.UtcNow
    };

    // ── Data pools ─────────────────────────────────────────────────────────────
    private static readonly string[] ChefNoms    = ["Khalid", "Nadia",  "Youssef", "Sanae", "Omar"];
    private static readonly string[] ChefPrenoms = ["Alaoui", "Tazi",   "Benkirane", "Chraibi", "Fassi"];
    private static readonly string[] TechNoms    = ["Hassan", "Moussa", "Amine",  "Rachid", "Tariq", "Bilal",    "Karim",    "Samir",   "Mehdi",  "Ilyas"];
    private static readonly string[] TechPrenoms = ["Idrissi","Hajji",  "Ziani",  "Berrada","Lamrani","Kabbaj",  "Sefrioui", "Tahiri",  "Naciri", "Ouali"];

    private static readonly EquipementEtat[] EtatsPool =
    [
        EquipementEtat.Disponible, EquipementEtat.Disponible, EquipementEtat.Disponible,
        EquipementEtat.En_maintenance, EquipementEtat.En_panne,
        EquipementEtat.Hors_service, EquipementEtat.Reserve
    ];

    private static readonly string[] Types        = ["Haute tension", "Basse tension", "Moyenne tension", "Industriel", "Résidentiel", "Numérique", "Analogique"];
    private static readonly string[] Marques      = ["ABB", "Schneider Electric", "Siemens", "Legrand", "General Electric", "Eaton", "Socomec", "Cisco", "HP", "Dell"];
    private static readonly string[] Localisations = ["Casablanca - Ain Sebaa", "Rabat - Agdal", "Fès - Médina", "Marrakech - Guéliz", "Tanger - Port", "Agadir - Secteur Sud", "Meknès - Centre", "Oujda - Est", "Settat - Zone industrielle", "Kénitra - Nord"];
    private static readonly string[] Fournisseurs  = ["Maroc Électricité SARL", "TechnoElec Maroc", "Électrotech Distribution", "SOMEX Industrie", "PowerTech Morocco", "Énergie Solutions MA"];
    private static readonly string[] ObservationsPool =
    [
        "Inspection visuelle complète effectuée. Aucune anomalie détectée.",
        "Remplacement des joints d'étanchéité et vérification du câblage.",
        "Nettoyage complet et test de fonctionnement validé.",
        "Recalibrage des capteurs et mise à jour du firmware.",
        "Remplacement du fusible principal et test de charge effectué.",
        "Vérification des connexions électriques et serrage des borniers.",
        "Lubrification des parties mécaniques et contrôle thermique.",
        "Remplacement du filtre à huile et vidange effectuée."
    ];
    private static readonly string[] PiecesPool =
    [
        "Fusible 63A", "Joint torique", "Contacteur 25A", "Disjoncteur différentiel",
        "Capteur de température", "Relais de protection", "Filtre à air", "Courroie de transmission"
    ];
}
