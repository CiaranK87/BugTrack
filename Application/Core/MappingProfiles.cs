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
                .ForMember(dest => dest.OwnerUsername, opt => opt.MapFrom(src => src.Participants
                .FirstOrDefault(x => x.IsOwner).AppUser.UserName))
                .ForMember(dest => dest.TicketCount, opt => opt.MapFrom(src => src.Tickets.Count()));

            CreateMap<ProjectParticipant, Profiles.Profile>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.AppUser.DisplayName))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.AppUser.UserName))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.AppUser.Bio));

            CreateMap<Ticket, Ticket>();
        }
    }
}