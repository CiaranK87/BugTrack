using Application.DTOs;
using Application.Projects;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            
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
                    opt => opt.MapFrom(src => src.Participants
                        .Select(pp => pp.AppUser))
                );


            CreateMap<AppUser, ProfileDto>()
                .ForMember(
                    dest => dest.Username,
                    opt => opt.MapFrom(src => src.UserName)
                )
                .ForMember(
                    dest => dest.DisplayName,
                    opt => opt.MapFrom(src => src.DisplayName)
                )
                .ForMember(
                    dest => dest.Bio,
                    opt => opt.MapFrom(src => src.Bio)
                )
                .ForMember(
                    dest => dest.Image,
                    opt => opt.Ignore()
                );

            CreateMap<Ticket, Ticket>();

            CreateMap<Ticket, TicketDto>();
        }
    }
}