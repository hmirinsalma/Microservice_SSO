using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.API.Authorization;
using System.ComponentModel.DataAnnotations;

namespace ONEE.SSO.API.Pages.Users;

[SsoAdminRequired]
public class EditModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        IUserService userService,
        IRoleService roleService,
        IUserRoleRepository userRoleRepository,
        ILogger<EditModel> logger)
    {
        _userService = userService;
        _roleService = roleService;
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<RoleDto> AvailableRoles { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères")]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        [Display(Name = "Nom")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [StringLength(255, ErrorMessage = "L'email ne peut pas dépasser 255 caractères")]
        [Display(Name = "Adresse Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Rôles")]
        [MinLength(1, ErrorMessage = "Vous devez sélectionner au moins un rôle")]
        public List<Guid> SelectedRoleIds { get; set; } = new();

        [Display(Name = "Compte Actif")]
        public bool IsActive { get; set; } = true;

        // Champs optionnels pour changement de mot de passe
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 128 caractères")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?"":;'{}|<>]).{8,}$",
            ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères, une majuscule, un chiffre et un caractère spécial")]
        [Display(Name = "Nouveau Mot de Passe (optionnel)")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas")]
        [Display(Name = "Confirmer le Nouveau Mot de Passe")]
        [DataType(DataType.Password)]
        public string? ConfirmNewPassword { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            // Charger l'utilisateur
            var user = await _userService.GetByIdAsync(Id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Utilisateur introuvable.";
                return RedirectToPage("./Index");
            }

            // Charger tous les rôles disponibles
            var roles = await _roleService.GetAllAsync();
            AvailableRoles = roles.ToList();

            // Charger les rôles actuels de l'utilisateur
            var userRoles = await _userRoleRepository.GetByUserIdAsync(Id);
            var userRoleIds = userRoles.Select(ur => ur.RoleId).ToList();

            // Pré-remplir le formulaire
            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                SelectedRoleIds = userRoleIds
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement de la page d'édition d'utilisateur {UserId}", Id);
            TempData["ErrorMessage"] = "Une erreur est survenue lors du chargement de la page.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            // Recharger les rôles pour l'affichage en cas d'erreur
            var roles = await _roleService.GetAllAsync();
            AvailableRoles = roles.ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Vérifier qu'au moins un rôle est sélectionné
            if (Input.SelectedRoleIds == null || !Input.SelectedRoleIds.Any())
            {
                ModelState.AddModelError("Input.SelectedRoleIds", "Vous devez sélectionner au moins un rôle");
                return Page();
            }

            // Vérifier si l'utilisateur existe
            var existingUser = await _userService.GetByIdAsync(Id);
            if (existingUser == null)
            {
                TempData["ErrorMessage"] = "Utilisateur introuvable.";
                return RedirectToPage("./Index");
            }

            // Vérifier si l'email est déjà utilisé par un autre utilisateur
            if (Input.Email != existingUser.Email)
            {
                var emailExists = await _userService.ExistsByEmailAsync(Input.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Input.Email", $"Un autre utilisateur avec l'email {Input.Email} existe déjà");
                    return Page();
                }
            }

            // Créer le DTO de mise à jour
            var updateDto = new UpdateUserDto
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                RoleIds = Input.SelectedRoleIds,
                IsActive = Input.IsActive
            };

            // Si un nouveau mot de passe est fourni, l'ajouter
            if (!string.IsNullOrWhiteSpace(Input.NewPassword))
            {
                updateDto.Password = Input.NewPassword;
            }

            // Mettre à jour l'utilisateur
            var updatedUser = await _userService.UpdateAsync(Id, updateDto);

            _logger.LogInformation("Utilisateur modifié avec succès: {Email} (ID: {UserId})", 
                updatedUser.Email, updatedUser.Id);

            TempData["SuccessMessage"] = $"L'utilisateur {updatedUser.FirstName} {updatedUser.LastName} a été modifié avec succès!";

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la modification de l'utilisateur: {UserId}", Id);
            
            // Afficher le détail de l'erreur pour le debug
            var errorMessage = $"Une erreur est survenue lors de la modification de l'utilisateur: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $" | Détail: {ex.InnerException.Message}";
            }
            
            ModelState.AddModelError(string.Empty, errorMessage);
            
            return Page();
        }
    }
}
