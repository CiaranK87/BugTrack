using Application.DTOs;
using Application.Projects;
using AutoMapper;
using Domain;
using System.Linq;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Project mappings
            CreateMap<Project, Project>();
            CreateMap<Project, ProjectDto>()
                .ForMember(
                    dest => dest.OwnerUsername,
                    opt => opt.MapFrom(src => src.Participants
                        .FirstOrDefault(x => x.IsOwner)!.AppUser.UserName)
                )
                .ForMember(
                    dest => dest.TicketCount,
                    opt => opt.MapFrom(src => src.Tickets.Count())
                )
                .ForMember(
                    dest => dest.Participants,
                    opt => opt.MapFrom(src => src.Participants.Select(pp => pp.AppUser))
                );

            CreateMap<ProjectDto, Project>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Participants, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectOwner, opt => opt.Ignore());

            // User profile
            CreateMap<AppUser, ProfileDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                .ForMember(dest => dest.Image, opt => opt.Ignore());

            // Tickets
            CreateMap<Ticket, Ticket>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            CreateMap<Ticket, TicketDto>();
            CreateMap<TicketDto, Ticket>();
            CreateMap<EditTicketDto, Ticket>()
                .ForMember(dest => dest.Submitter, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            
            // Ticket with Project
            CreateMap<Ticket, TicketDto>()
                .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src => src.Project != null ? src.Project.ProjectTitle : null));
        }
    }
}