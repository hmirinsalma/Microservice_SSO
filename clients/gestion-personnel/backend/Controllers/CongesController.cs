using GestionPersonnel.API.Data;
using GestionPersonnel.API.DTOs.Conge;
using GestionPersonnel.API.Exceptions;
using GestionPersonnel.API.Infrastructure;
using GestionPersonnel.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CongesController : ControllerBase
{
    private readonly ICongeService _service;
    private readonly AppDbContext  _db;

    public CongesController(ICongeService service, AppDbContext db)
    {
        _service = service;
        _db      = db;
    }

    // ── GET — liste selon le rôle du claim JWT ───────────────
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CongeQueryDto query)
    {
        var role = ClaimsHelper.GetRole(User);

        if (role == "AdministrateurRH")
            return Ok(await _service.GetAllAsync(query));

        var userId  = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
        var employe = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId);
        if (employe == null) return Ok(Array.Empty<object>());

        return role switch
        {
            "Directeur"    => Ok(await _service.GetByDirectionAsync(employe.DirectionId, query.Statut)),
            "ChefDeService"=> Ok(await _service.GetByServiceAsync(employe.ServiceId, query.Statut)),
            _              => Ok(await _service.GetMyCongesAsync(employe.Id)),
        };
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _service.GetByIdAsync(id));

    // ── POST — créer une demande ─────────────────────────────
    [HttpPost]
    [Authorize(Roles = "Employe,ChefDeService,Directeur")]
    public async Task<IActionResult> Create([FromBody] CreateCongeDto dto)
    {
        var userId  = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
        var employe = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId)
            ?? throw new AppException("Aucune fiche employé liée à ce compte.", 404);

        var created = await _service.CreateAsync(employe.Id, dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // ── PATCH — traiter par chef de service ──────────────────
    [HttpPatch("{id:int}/traiter-chef")]
    [Authorize(Roles = "ChefDeService")]
    public async Task<IActionResult> TraiterChef(int id, [FromBody] TraiterCongeDto dto)
    {
        var userId  = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
        var employe = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId)
            ?? throw new AppException("Aucune fiche employé liée à ce compte.", 404);

        return Ok(await _service.TraiterParChefAsync(id, employe.Id, dto));
    }

    // ── PATCH — traiter par directeur ────────────────────────
    [HttpPatch("{id:int}/traiter-directeur")]
    [Authorize(Roles = "Directeur")]
    public async Task<IActionResult> TraiterDirecteur(int id, [FromBody] TraiterCongeDto dto)
    {
        var userId  = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
        var employe = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId)
            ?? throw new AppException("Aucune fiche employé liée à ce compte.", 404);

        return Ok(await _service.TraiterParDirecteurAsync(id, employe.Id, dto));
    }

    // ── DELETE — annuler ─────────────────────────────────────
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Annuler(int id)
    {
        var userId  = await ClaimsHelper.ResolveLocalUserIdAsync(User, _db);
        var employe = await _db.Employes.FirstOrDefaultAsync(e => e.UserId == userId)
            ?? throw new AppException("Aucune fiche employé liée à ce compte.", 404);

        await _service.AnnulerAsync(id, employe.Id);
        return NoContent();
    }
}
