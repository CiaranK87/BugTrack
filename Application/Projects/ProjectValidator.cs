using Domain;
using FluentValidation;

namespace Application.Projects
{
    public class ProjectValidator : AbstractValidator<Project>
    {
        public ProjectValidator()
        {
            RuleFor(x => x.ProjectTitle).NotEmpty();
            RuleFor(x => x.ProjectOwner).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
        }
    }
}