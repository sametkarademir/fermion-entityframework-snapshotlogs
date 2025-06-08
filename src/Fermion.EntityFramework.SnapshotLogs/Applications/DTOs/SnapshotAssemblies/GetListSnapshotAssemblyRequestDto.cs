using System.Text.Json.Serialization;
using Fermion.EntityFramework.Shared.DTOs.Sorting;
using FluentValidation;

namespace Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAssemblies;

public class GetListSnapshotAssemblyRequestDto
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 25;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortOrderTypes Order { get; set; } = SortOrderTypes.Desc;
    public string? Field { get; set; } = null;
    public string? Search { get; set; } = null;

    public Guid? SnapshotLogId { get; set; }
}

public class GetListSnapshotAssemblyRequestValidator : AbstractValidator<GetListSnapshotAssemblyRequestDto>
{
    public GetListSnapshotAssemblyRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than or equal to 1.");

        RuleFor(x => x.PerPage)
            .InclusiveBetween(1, 100).WithMessage("Items per page must be between 1 and 100.");

        RuleFor(x => x.Field)
            .MaximumLength(100).WithMessage("Field name cannot exceed 100 characters.").Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Field name can only contain alphanumeric characters and underscores.");

        RuleFor(x => x.Order)
            .IsInEnum().WithMessage("Order must be either Asc or Desc.");

        RuleFor(x => x.Search)
            .MaximumLength(256).WithMessage("Search term cannot be longer than 256 characters.")
            .Matches(@"^[a-zA-Z0-9\s]*$").WithMessage("Search term can only contain alphanumeric characters and spaces.");
    }
}