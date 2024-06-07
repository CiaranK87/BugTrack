using Domain;
using FluentValidation;

namespace Application.Tickets
{
    public class TicketValidator : AbstractValidator<Ticket>
    {
        public TicketValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Submitter).NotEmpty();
            RuleFor(x => x.Assigned).NotEmpty();
            RuleFor(x => x.Priority).NotEmpty();
            RuleFor(x => x.Severity).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.Updated).NotEmpty();
        }
    }
}