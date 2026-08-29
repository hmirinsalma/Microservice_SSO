using AutoMapper;
using GestionPersonnel.API.DTOs.Common;
using GestionPersonnel.API.DTOs.Employe;
using GestionPersonnel.API.Exceptions;
using GestionPersonnel.API.Models;
using GestionPersonnel.API.Repositories.Interfaces;
using GestionPersonnel.API.Services.Interfaces;

namespace GestionPersonnel.API.Services;

public class EmployeService : IEmployeService
{
    private readonly IEmployeRepository _repo;
    private readonly IDirectionRepository _directionRepo;
    private readonly IServiceRepository _serviceRepo;
    private readonly IMapper _mapper;

    public EmployeService(
        IEmployeRepository repo,
        IDirectionRepository directionRepo,
        IServiceRepository serviceRepo,
        IMapper mapper)
    {
        _repo = repo;
        _directionRepo = directionRepo;
        _serviceRepo = serviceRepo;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<EmployeDto>> GetPagedAsync(EmployeQueryDto query)
    {
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize < 1) query.PageSize = 10;
        if (query.PageSize > 100) query.PageSize = 100;

        var (items, total) = await _repo.GetPagedAsync(query);

        return new PagedResultDto<EmployeDto>
        {
            Data = _mapper.Map<IEnumerable<EmployeDto>>(items),
            TotalCount = total,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<EmployeDto> GetByIdAsync(int id)
    {
        var employe = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Employé", id);
        return _mapper.Map<EmployeDto>(employe);
    }

    public async Task<EmployeDto> CreateAsync(CreateEmployeDto dto)
    {
        _ = await _directionRepo.GetByIdAsync(dto.DirectionId)
            ?? throw new NotFoundException("Direction", dto.DirectionId);
        _ = await _serviceRepo.GetByIdAsync(dto.ServiceId)
            ?? throw new NotFoundException("Service", dto.ServiceId);

        if (await _repo.GetByMatriculeAsync(dto.Matricule) != null)
            throw new ConflictException($"Le matricule '{dto.Matricule}' est déjà utilisé.");

        if (await _repo.GetByEmailAsync(dto.Email) != null)
            throw new ConflictException($"L'email '{dto.Email}' est déjà utilisé.");

        var employe = _mapper.Map<Employe>(dto);
        var created = await _repo.CreateAsync(employe);
        var result = await _repo.GetByIdAsync(created.Id);
        return _mapper.Map<EmployeDto>(result!);
    }

    public async Task<EmployeDto> UpdateAsync(int id, UpdateEmployeDto dto)
    {
        var employe = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Employé", id);

        _ = await _directionRepo.GetByIdAsync(dto.DirectionId)
            ?? throw new NotFoundException("Direction", dto.DirectionId);
        _ = await _serviceRepo.GetByIdAsync(dto.ServiceId)
            ?? throw new NotFoundException("Service", dto.ServiceId);

        var existingEmail = await _repo.GetByEmailAsync(dto.Email);
        if (existingEmail != null && existingEmail.Id != id)
            throw new ConflictException($"L'email '{dto.Email}' est déjà utilisé.");

        _mapper.Map(dto, employe);
        var updated = await _repo.UpdateAsync(employe);
        var result = await _repo.GetByIdAsync(updated.Id);
        return _mapper.Map<EmployeDto>(result!);
    }

    public async Task DeleteAsync(int id)
    {
        var employe = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Employé", id);
        await _repo.DeleteAsync(employe);
    }
}
