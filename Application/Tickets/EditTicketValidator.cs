using FluentValidation;

public class EditTicketValidator : AbstractValidator<EditTicketDto>
{
    public EditTicketValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
