using TIMS.API.Entities;

namespace TIMS.API.Data;

/// <summary>
/// Seeder réaliste — 200 interventions + données métier.
/// Les mots de passe sont dans StubCredentials (table temporaire SSO Stub).
/// La table Users ne contient aucune donnée d'authentification.
/// </summary>
public static class DbSeeder
{
    private static readonly Random _rnd = new(42);

    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (db.Users.Any()) return;

        // ── Services ──────────────────────────────────────────────────────────
        var services = new[]
        {
            new Entities.Service { Name = "Direction Technique",         Description = "Coordination technique globale" },
            new Entities.Service { Name = "Maintenance Électrique",      Description = "Maintenance HT/MT/BT" },
            new Entities.Service { Name = "Maintenance Mécanique",       Description = "Équipements mécaniques et hydrauliques" },
            new Entities.Service { Name = "Réseau & Télécommunications", Description = "Infrastructure réseau ONEE" },
            new Entities.Service { Name = "Production Énergétique",      Description = "Centrales et groupes électrogènes" },
        };
        db.Services.AddRange(services);
        await db.SaveChangesAsync();

        // ── Équipes ───────────────────────────────────────────────────────────
        var equipes = new[]
        {
            new Equipe { Name="Équipe HT Nord",    ServiceId=services[1].Id, Description="Haute tension zone nord" },
            new Equipe { Name="Équipe HT Sud",     ServiceId=services[1].Id, Description="Haute tension zone sud" },
            new Equipe { Name="Équipe MT Centre",  ServiceId=services[1].Id, Description="Moyenne tension centre" },
            new Equipe { Name="Équipe Hydraulique",ServiceId=services[2].Id, Description="Pompes et circuits" },
            new Equipe { Name="Équipe Mécanique",  ServiceId=services[2].Id, Description="Moteurs et réducteurs" },
            new Equipe { Name="Équipe Fibre",      ServiceId=services[3].Id, Description="Réseau fibre optique" },
            new Equipe { Name="Équipe Télécom",    ServiceId=services[3].Id, Description="Systèmes télécom" },
            new Equipe { Name="Équipe GE",         ServiceId=services[4].Id, Description="Groupes électrogènes" },
            new Equipe { Name="Équipe Solaire",    ServiceId=services[4].Id, Description="Installations photovoltaïques" },
            new Equipe { Name="Équipe Support",    ServiceId=services[0].Id, Description="Support technique général" },
        };
        db.Equipes.AddRange(equipes);
        await db.SaveChangesAsync();

        // ── Users (données métier uniquement — aucun mot de passe) ────────────
        var admin = new User
        {
            FirstName="Khalid",   LastName="Bensouda",
            Email="admin@onee.ma",Poste="Administrateur Système",
            RoleMetier=RoleNames.AdminTechnique,
            ServiceId=services[0].Id, EquipeId=equipes[9].Id
        };
        var directeur = new User
        {
            FirstName="Mohammed", LastName="Alami",
            Email="directeur@onee.ma", Poste="Directeur Technique",
            RoleMetier=RoleNames.DirecteurTechnique,
            ServiceId=services[0].Id
        };
        var chefs = new[]
        {
            new User { FirstName="Fatima",  LastName="Benali",  Email="chef1@onee.ma", Poste="Chef de Service Électrique",  RoleMetier=RoleNames.ChefService, ServiceId=services[1].Id, EquipeId=equipes[0].Id },
            new User { FirstName="Yassine", LastName="Chraibi", Email="chef2@onee.ma", Poste="Chef de Service Mécanique",   RoleMetier=RoleNames.ChefService, ServiceId=services[2].Id, EquipeId=equipes[3].Id },
            new User { FirstName="Nadia",   LastName="Tazi",    Email="chef3@onee.ma", Poste="Chef de Service Réseau",      RoleMetier=RoleNames.ChefService, ServiceId=services[3].Id, EquipeId=equipes[5].Id },
            new User { FirstName="Hamid",   LastName="Ouali",   Email="chef4@onee.ma", Poste="Chef de Service Production",  RoleMetier=RoleNames.ChefService, ServiceId=services[4].Id, EquipeId=equipes[7].Id },
            new User { FirstName="Samira",  LastName="Idrissi", Email="chef5@onee.ma", Poste="Chef de Service Support",     RoleMetier=RoleNames.ChefService, ServiceId=services[0].Id, EquipeId=equipes[9].Id },
        };

        var techData = new[]
        {
            ("Karim","Mansouri","tech01@onee.ma",1,0),("Soufiane","Rachidi","tech02@onee.ma",1,0),
            ("Amine","Bennani","tech03@onee.ma",1,1), ("Omar","Filali","tech04@onee.ma",1,1),
            ("Reda","Tahiri","tech05@onee.ma",1,2),   ("Hicham","Bousfiha","tech06@onee.ma",1,2),
            ("Younes","Lahlou","tech07@onee.ma",2,3), ("Tariq","Mrani","tech08@onee.ma",2,3),
            ("Mehdi","Berrada","tech09@onee.ma",2,4), ("Anas","Kettani","tech10@onee.ma",2,4),
            ("Imad","Ziani","tech11@onee.ma",3,5),    ("Khalid","Sebti","tech12@onee.ma",3,5),
            ("Youssef","Amrani","tech13@onee.ma",3,6),("Nabil","Chaoui","tech14@onee.ma",3,6),
            ("Aziz","Benomar","tech15@onee.ma",4,7),  ("Said","Hajji","tech16@onee.ma",4,7),
            ("Rachid","Belhaj","tech17@onee.ma",4,8), ("Tarik","Fassi","tech18@onee.ma",4,8),
            ("Iliass","Moussaoui","tech19@onee.ma",1,0),("Othmane","Skalli","tech20@onee.ma",2,3),
            ("Brahim","Alaoui","tech21@onee.ma",3,5), ("Zakaria","Taoufik","tech22@onee.ma",4,7),
            ("Hassan","Benkirane","tech23@onee.ma",1,2),("Mouad","Rifai","tech24@onee.ma",2,4),
            ("Adil","Mekki","tech25@onee.ma",3,6),
        };
        var posteLabels = new[] { "Technicien Électricien","Technicien Mécanicien","Technicien Réseau","Technicien Énergétique" };
        var techniciens = techData.Select(t => new User
        {
            FirstName=t.Item1, LastName=t.Item2, Email=t.Item3,
            Poste=posteLabels[t.Item4-1],
            RoleMetier=RoleNames.Technicien,
            ServiceId=services[t.Item4].Id, EquipeId=equipes[t.Item5].Id
        }).ToList();

        db.Users.Add(admin); db.Users.Add(directeur);
        db.Users.AddRange(chefs); db.Users.AddRange(techniciens);
        await db.SaveChangesAsync();

        // ── UserRoles (autorisations JWT) ─────────────────────────────────────
        db.UserRoles.Add(new UserRole { UserId=admin.Id, RoleId=1 });
        db.UserRoles.Add(new UserRole { UserId=directeur.Id, RoleId=2 });
        foreach (var c in chefs)      db.UserRoles.Add(new UserRole { UserId=c.Id, RoleId=3 });
        foreach (var t in techniciens) db.UserRoles.Add(new UserRole { UserId=t.Id, RoleId=4 });
        await db.SaveChangesAsync();

        // ── StubCredentials (TEMPORAIRE — séparées des données métier) ────────
        // ⚠️ À SUPPRIMER lors de l'intégration SSO
        var allUsers = new List<User> { admin, directeur }.Concat(chefs).Concat(techniciens).ToList();
        var stubs = allUsers.Select(u => new StubCredentials
        {
            UserId       = u.Id,
            Email        = u.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                u.Email.StartsWith("admin") ? "Admin@123"     :
                u.Email.StartsWith("directeur") ? "Directeur@123" :
                u.Email.StartsWith("chef") ? "Chef@123" : "Tech@123", 12)
        }).ToList();
        db.StubCredentials.AddRange(stubs);
        await db.SaveChangesAsync();

        // ── 200 Interventions ─────────────────────────────────────────────────
        await SeedInterventionsAsync(db, admin, chefs.ToList(), techniciens, services, equipes);
    }

    private static async Task SeedInterventionsAsync(ApplicationDbContext db, User admin,
        List<User> chefs, List<User> techs, Entities.Service[] services, Equipe[] equipes)
    {
        var objets = new[]
        {
            "Panne du transformateur 63kV","Défaillance disjoncteur HT","Court-circuit jeu de barres",
            "Surtension ligne MT","Panne groupe électrogène secours","Défaut protection différentielle",
            "Remplacement câble MT défectueux","Maintenance préventive transformateur",
            "Vérification tableau BT","Inspection installation HT","Coupure alimentation secteur",
            "Panne pompe circulation eau refroidissement","Fuite hydraulique circuit primaire",
            "Maintenance pompe centrifuge","Réparation joint hydraulique",
            "Coupure réseau fibre optique","Défaut transmission données SCADA",
            "Panne routeur communication","Panne groupe électrogène GE-01",
            "Remplacement batteries onduleur","Défaut régulateur tension",
            "Inspection panneau solaire","Défaillance onduleur solaire",
            "Vérification éclairage secours","Défaut éclairage extérieur",
            "Maintenance préventive annuelle poste","Révision générale groupe électrogène",
        };
        var descs = new[]
        {
            "Intervention urgente suite à l'alarme déclenchée en salle de contrôle.",
            "Panne détectée lors de la ronde de surveillance nocturne.",
            "Maintenance planifiée dans le cadre du programme annuel de révision.",
            "Défaut signalé par le système de télécontrôle SCADA.",
            "Remplacement préventif selon les recommandations du fournisseur.",
            "Inspection réglementaire obligatoire avant la prochaine période de pointe.",
        };
        var locs  = new[] { "Poste HT Casablanca-Est","Sous-station Rabat-Centre","Poste 225kV Mohammedia","Centrale thermique Jerada","Site solaire Ouarzazate","Poste 63kV Agadir" };
        var equips= new[] { "Transformateur TR-63/22kV","Disjoncteur 63kV SF6","Groupe électrogène 500kVA","Pompe centrifuge PC-02","Câble MT 22kV XLPE","Panneau solaire 350Wc","Switch Cisco","Onduleur 100kVA" };
        var types = new[] { "Curative","Préventive","Réglementaire","Prédictive" };
        var cats  = new[] { "Électrique","Mécanique","Hydraulique","Télécom","Instrumentation" };

        var statusW  = new[] { (InterventionStatus.Nouvelle,15),(InterventionStatus.EnCours,25),(InterventionStatus.Suspendue,10),(InterventionStatus.Terminee,45),(InterventionStatus.Annulee,5) };
        var priorityW= new[] { (InterventionPriority.Faible,20),(InterventionPriority.Normale,45),(InterventionPriority.Urgente,25),(InterventionPriority.Critique,10) };

        InterventionStatus PickS() { var r=_rnd.Next(100); int c=0; foreach(var(s,w) in statusW){c+=w;if(r<c)return s;} return InterventionStatus.Terminee; }
        InterventionPriority PickP() { var r=_rnd.Next(100); int c=0; foreach(var(p,w) in priorityW){c+=w;if(r<c)return p;} return InterventionPriority.Normale; }

        var base_ = DateTime.UtcNow.AddMonths(-6);
        var interventions = new List<Intervention>();
        for (int n=1; n<=200; n++)
        {
            var status=PickS(); var prio=PickP();
            var created=base_.AddDays(_rnd.Next(0,180)).AddHours(_rnd.Next(0,24));
            var prevue=created.AddDays(_rnd.Next(1,15));
            var svcIdx=_rnd.Next(1,services.Length);
            var chef=chefs[Math.Min(svcIdx-1,chefs.Count-1)];
            var techList=techs.Where(t=>t.ServiceId==services[svcIdx].Id).ToList();
            var tech=techList.Count>0 ? techList[_rnd.Next(techList.Count)] : techs[_rnd.Next(techs.Count)];
            var eqs=equipes.Where(e=>e.ServiceId==services[svcIdx].Id).ToArray();
            var eq=eqs.Length>0 ? eqs[_rnd.Next(eqs.Length)] : equipes[0];

            interventions.Add(new Intervention
            {
                NumeroIntervention=$"INT-{created:yyyyMMdd}-{n:D4}",
                Objet=objets[_rnd.Next(objets.Length)],Description=descs[_rnd.Next(descs.Length)],
                TypeIntervention=types[_rnd.Next(types.Length)],Categorie=cats[_rnd.Next(cats.Length)],
                Localisation=locs[_rnd.Next(locs.Length)],Equipement=equips[_rnd.Next(equips.Length)],
                CreatedAt=created,DatePrevue=prevue,
                DateCloture=status==InterventionStatus.Terminee ? prevue.AddDays(_rnd.Next(0,5)) : null,
                Priority=prio,Status=status,
                ServiceId=services[svcIdx].Id,EquipeId=eq.Id,
                ResponsableId=chef.Id,ChefServiceId=chef.Id,
                TechnicienId=(status!=InterventionStatus.Nouvelle||_rnd.Next(2)==0) ? tech.Id : null,
                CreatedById=admin.Id,
                CompteRendu=status==InterventionStatus.Terminee ? $"Intervention réalisée. Durée : {_rnd.Next(1,8)}h. Équipement remis en service nominal." : null,
            });
        }
        db.Interventions.AddRange(interventions);
        await db.SaveChangesAsync();

        // Historiques
        var hist = new List<InterventionHistory>();
        var coms = new List<Comment>();
        var cmts = new[]{"Intervention démarrée.","Pièce de rechange commandée.","Problème plus complexe que prévu.","Tests en cours.","Remplacement effectué.","Équipement opérationnel."};
        foreach (var i in interventions)
        {
            hist.Add(new InterventionHistory { InterventionId=i.Id,AuthorId=admin.Id,ActionType=HistoryActionType.Creation,Description=$"Intervention {i.NumeroIntervention} créée",CreatedAt=i.CreatedAt });
            if (i.Status!=InterventionStatus.Nouvelle) hist.Add(new InterventionHistory { InterventionId=i.Id,AuthorId=i.TechnicienId??admin.Id,ActionType=HistoryActionType.ChangementStatut,FieldChanged="Status",OldValue=InterventionStatus.Nouvelle.ToString(),NewValue=i.Status.ToString(),CreatedAt=i.CreatedAt.AddHours(_rnd.Next(1,24)) });
            if (i.TechnicienId.HasValue) hist.Add(new InterventionHistory { InterventionId=i.Id,AuthorId=i.ChefServiceId??admin.Id,ActionType=HistoryActionType.Affectation,Description="Technicien affecté",CreatedAt=i.CreatedAt.AddMinutes(_rnd.Next(30,120)) });
            if (i.Status is InterventionStatus.Terminee or InterventionStatus.EnCours)
                for(int c=0;c<_rnd.Next(1,4);c++) coms.Add(new Comment{InterventionId=i.Id,AuthorId=i.TechnicienId??admin.Id,Content=cmts[_rnd.Next(cmts.Length)],CreatedAt=i.CreatedAt.AddHours(_rnd.Next(2,48))});
        }
        if(hist.Count>0){db.InterventionHistories.AddRange(hist);await db.SaveChangesAsync();}
        if(coms.Count>0){db.Comments.AddRange(coms);await db.SaveChangesAsync();}

        // Notifications
        var notifs = new List<Notification>();
        foreach (var i in interventions.TakeLast(20))
        {
            if(i.TechnicienId.HasValue) notifs.Add(new Notification{UserId=i.TechnicienId.Value,InterventionId=i.Id,Type=NotificationType.TechnicienAffecte,Title="Nouvelle affectation",Message=$"[{i.NumeroIntervention}] {i.Objet}",IsRead=false,CreatedAt=i.CreatedAt.AddMinutes(30)});
            notifs.Add(new Notification{UserId=admin.Id,InterventionId=i.Id,Type=NotificationType.InterventionCreee,Title="Nouvelle intervention",Message=$"[{i.NumeroIntervention}] {i.Objet}",IsRead=_rnd.Next(2)==0,CreatedAt=i.CreatedAt});
        }
        db.Notifications.AddRange(notifs);
        await db.SaveChangesAsync();
    }
}
