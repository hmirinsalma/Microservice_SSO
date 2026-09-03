using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.API.Authorization;
using System.ComponentModel.DataAnnotations;

namespace ONEE.SSO.API.Pages.Users;

[SsoAdminRequired]
public class CreateModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(
        IUserService userService,
        IRoleService roleService,
        ILogger<CreateModel> logger)
    {
        _userService = userService;
        _roleService = roleService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<RoleDto> AvailableRoles { get; set; } = new();

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

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 128 caractères")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?"":;'{}|<>]).{8,}$",
            ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères, une majuscule, un chiffre et un caractère spécial")]
        [Display(Name = "Mot de Passe")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmation du mot de passe est requise")]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas")]
        [Display(Name = "Confirmer le Mot de Passe")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Rôles")]
        [MinLength(1, ErrorMessage = "Vous devez sélectionner au moins un rôle")]
        public List<Guid> SelectedRoleIds { get; set; } = new();

        [Display(Name = "Compte Actif")]
        public bool IsActive { get; set; } = true;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            // Charger tous les rôles disponibles
            var roles = await _roleService.GetAllAsync();
            AvailableRoles = roles.ToList();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement de la page de création d'utilisateur");
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

            // Vérifier si l'email existe déjà
            var emailExists = await _userService.ExistsByEmailAsync(Input.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Input.Email", $"Un utilisateur avec l'email {Input.Email} existe déjà");
                return Page();
            }

            // Créer le DTO
            var createDto = new CreateUserDto
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                Password = Input.Password,
                RoleIds = Input.SelectedRoleIds,
                IsActive = Input.IsActive
            };

            // Créer l'utilisateur
            var createdUser = await _userService.CreateAsync(createDto);

            _logger.LogInformation("Utilisateur créé avec succès: {Email} (ID: {UserId})", 
                createdUser.Email, createdUser.Id);

            TempData["SuccessMessage"] = $"L'utilisateur {createdUser.FirstName} {createdUser.LastName} a été créé avec succès!";

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création de l'utilisateur: {Email}", Input.Email);
            
            ModelState.AddModelError(string.Empty, 
                "Une erreur est survenue lors de la création de l'utilisateur. Veuillez réessayer.");
            
            return Page();
        }
    }
}
