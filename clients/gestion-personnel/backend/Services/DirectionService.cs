using AutoMapper;
using GestionPersonnel.API.DTOs.Direction;
using GestionPersonnel.API.Exceptions;
using GestionPersonnel.API.Models;
using GestionPersonnel.API.Repositories.Interfaces;
using GestionPersonnel.API.Services.Interfaces;

namespace GestionPersonnel.API.Services;

public class DirectionService : IDirectionService
{
    private readonly IDirectionRepository _repo;
    private readonly IMapper _mapper;

    public DirectionService(IDirectionRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DirectionDto>> GetAllAsync()
    {
        var directions = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<DirectionDto>>(directions);
    }

    public async Task<DirectionDto> GetByIdAsync(int id)
    {
        var direction = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Direction", id);
        return _mapper.Map<DirectionDto>(direction);
    }

    public async Task<DirectionDto> CreateAsync(CreateDirectionDto dto)
    {
        var existing = await _repo.GetByNomAsync(dto.Nom);
        if (existing != null)
            throw new ConflictException($"Une direction avec le nom '{dto.Nom}' existe déjà.");

        var direction = _mapper.Map<Direction>(dto);
        var created = await _repo.CreateAsync(direction);
        return _mapper.Map<DirectionDto>(created);
    }

    public async Task<DirectionDto> UpdateAsync(int id, UpdateDirectionDto dto)
    {
        var direction = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Direction", id);

        var existing = await _repo.GetByNomAsync(dto.Nom);
        if (existing != null && existing.Id != id)
            throw new ConflictException($"Une direction avec le nom '{dto.Nom}' existe déjà.");

        _mapper.Map(dto, direction);
        var updated = await _repo.UpdateAsync(direction);
        return _mapper.Map<DirectionDto>(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var direction = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Direction", id);

        if (await _repo.HasServicesAsync(id))
            throw new AppException("Impossible de supprimer une direction qui possède des services.", 409);

        if (await _repo.HasEmployesAsync(id))
            throw new AppException("Impossible de supprimer une direction qui possède des employés.", 409);

        await _repo.DeleteAsync(direction);
    }
}
