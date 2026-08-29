namespace TIMS.API.Entities;

public enum InterventionStatus
{
    Nouvelle = 1,
    EnCours = 2,
    Suspendue = 3,
    Terminee = 4,
    Annulee = 5
}

public enum InterventionPriority
{
    Faible = 1,
    Normale = 2,
    Urgente = 3,
    Critique = 4
}

public enum HistoryActionType
{
    Creation = 1,
    Modification = 2,
    ChangementTechnicien = 3,
    ChangementResponsable = 4,
    ChangementStatut = 5,
    ChangementPriorite = 6,
    AjoutCommentaire = 7,
    AjoutPieceJointe = 8,
    Affectation = 9,
    RetraitAffectation = 10,
    AjoutCompteRendu = 11
}

public enum NotificationType
{
    InterventionCreee = 1,
    TechnicienAffecte = 2,
    ChangementTechnicien = 3,
    ChangementResponsable = 4,
    ChangementPriorite = 5,
    ChangementStatut = 6,
    InterventionTerminee = 7
}

public static class RoleNames
{
    public const string AdminTechnique = "Administrateur_Technique";
    public const string DirecteurTechnique = "Directeur_Technique";
    public const string ChefService = "Chef_de_Service";
    public const string Technicien = "Technicien";
}
