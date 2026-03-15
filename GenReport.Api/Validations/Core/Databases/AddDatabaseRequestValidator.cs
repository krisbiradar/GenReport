using FastEndpoints;
using FluentValidation;
using GenReport.Infrastructure.Models.HttpRequests.Core.Databases;

namespace GenReport.Api.Validations.Core.Databases
{
    public class AddDatabaseRequestValidator : Validator<AddDatabaseRequest>
    {
        public AddDatabaseRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Database Name is required.");
            RuleFor(x => x.Type).NotEmpty().WithMessage("Database Type is required.");
            RuleFor(x => x.ConnectionString).NotEmpty().WithMessage("Database Connection String is required.");
        }
    }
}
