using FastEndpoints;
using FluentValidation;
using GenReport.Infrastructure.Models.HttpRequests.Core.Databases;

namespace GenReport.Api.Validations.Core.Databases
{
    public class EditDatabaseRequestValidator : Validator<EditDatabaseRequest>
    {
        public EditDatabaseRequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid Database ID.");
        }
    }
}
