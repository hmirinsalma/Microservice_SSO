using AutoMapper;
using GestionPersonnel.API.DTOs.Service;
using GestionPersonnel.API.Exceptions;
using GestionPersonnel.API.Repositories.Interfaces;
using GestionPersonnel.API.Services.Interfaces;
using ServiceModel = GestionPersonnel.API.Models.Service;

namespace GestionPersonnel.API.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _repo;
    private readonly IDirectionRepository _directionRepo;
    private readonly IMapper _mapper;

    public ServiceService(IServiceRepository repo, IDirectionRepository directionRepo, IMapper mapper)
    {
        _repo = repo;
        _directionRepo = directionRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ServiceDto>> GetAllAsync()
    {
        var services = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<ServiceDto>>(services);
    }

    public async Task<IEnumerable<ServiceDto>> GetByDirectionAsync(int directionId)
    {
        _ = await _directionRepo.GetByIdAsync(directionId)
            ?? throw new NotFoundException("Direction", directionId);
        var services = await _repo.GetByDirectionAsync(directionId);
        return _mapper.Map<IEnumerable<ServiceDto>>(services);
    }

    public async Task<ServiceDto> GetByIdAsync(int id)
    {
        var service = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Service", id);
        return _mapper.Map<ServiceDto>(service);
    }

    public async Task<ServiceDto> CreateAsync(CreateServiceDto dto)
    {
        _ = await _directionRepo.GetByIdAsync(dto.DirectionId)
            ?? throw new NotFoundException("Direction", dto.DirectionId);

        var existing = await _repo.GetByNomAndDirectionAsync(dto.Nom, dto.DirectionId);
        if (existing != null)
            throw new ConflictException($"Un service '{dto.Nom}' existe déjà dans cette direction.");

        var service = _mapper.Map<ServiceModel>(dto);
        var created = await _repo.CreateAsync(service);

        // Reload with includes
        var result = await _repo.GetByIdAsync(created.Id);
        return _mapper.Map<ServiceDto>(result!);
    }

    public async Task<ServiceDto> UpdateAsync(int id, UpdateServiceDto dto)
    {
        var service = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Service", id);

        _ = await _directionRepo.GetByIdAsync(dto.DirectionId)
            ?? throw new NotFoundException("Direction", dto.DirectionId);

        var existing = await _repo.GetByNomAndDirectionAsync(dto.Nom, dto.DirectionId);
        if (existing != null && existing.Id != id)
            throw new ConflictException($"Un service '{dto.Nom}' existe déjà dans cette direction.");

        _mapper.Map(dto, service);
        var updated = await _repo.UpdateAsync(service);
        var result = await _repo.GetByIdAsync(updated.Id);
        return _mapper.Map<ServiceDto>(result!);
    }

    public async Task DeleteAsync(int id)
    {
        var service = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Service", id);

        if (await _repo.HasEmployesAsync(id))
            throw new AppException("Impossible de supprimer un service qui possède des employés.", 409);

        await _repo.DeleteAsync(service);
    }
}
