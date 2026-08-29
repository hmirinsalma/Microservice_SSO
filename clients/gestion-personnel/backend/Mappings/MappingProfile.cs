using AutoMapper;
using GestionPersonnel.API.DTOs.Direction;
using GestionPersonnel.API.DTOs.Employe;
using GestionPersonnel.API.DTOs.Service;
using GestionPersonnel.API.Models;
using ServiceModel = GestionPersonnel.API.Models.Service;

namespace GestionPersonnel.API.Mappings;

public class MappingProfile : Profile
{
    private static StatutEmploye ParseStatut(string statut)
        => Enum.TryParse<StatutEmploye>(statut, out var r) ? r : StatutEmploye.Actif;

    public MappingProfile()
    {
        // Direction
        CreateMap<Direction, DirectionDto>()
            .ForMember(d => d.NombreServices, opt => opt.MapFrom(s => s.Services.Count))
            .ForMember(d => d.NombreEmployes, opt => opt.MapFrom(s => s.Employes.Count));
        CreateMap<CreateDirectionDto, Direction>();
        CreateMap<UpdateDirectionDto, Direction>();

        // Service
        CreateMap<ServiceModel, ServiceDto>()
            .ForMember(d => d.DirectionNom,   opt => opt.MapFrom(s => s.Direction.Nom))
            .ForMember(d => d.NombreEmployes, opt => opt.MapFrom(s => s.Employes.Count));
        CreateMap<CreateServiceDto, ServiceModel>();
        CreateMap<UpdateServiceDto, ServiceModel>();

        // Employe — incluant les nouveaux champs + responsable
        CreateMap<Employe, EmployeDto>()
            .ForMember(d => d.DirectionNom,       opt => opt.MapFrom(s => s.Direction.Nom))
            .ForMember(d => d.ServiceNom,         opt => opt.MapFrom(s => s.Service.Nom))
            .ForMember(d => d.Statut,             opt => opt.MapFrom(s => s.Statut.ToString()))
            .ForMember(d => d.Adresse,            opt => opt.MapFrom(s => s.Adresse))
            .ForMember(d => d.PhotoUrl,           opt => opt.MapFrom(s => s.PhotoUrl))
            .ForMember(d => d.ResponsableNom,     opt => opt.MapFrom(s =>
                s.Responsable != null ? $"{s.Responsable.Nom} {s.Responsable.Prenom}" : null))
            .ForMember(d => d.UserId,             opt => opt.MapFrom(s => s.UserId));

        CreateMap<CreateEmployeDto, Employe>()
            .ForMember(d => d.Statut, opt => opt.MapFrom(s => ParseStatut(s.Statut)));
        CreateMap<UpdateEmployeDto, Employe>()
            .ForMember(d => d.Statut, opt => opt.MapFrom(s => ParseStatut(s.Statut)));
    }
}
