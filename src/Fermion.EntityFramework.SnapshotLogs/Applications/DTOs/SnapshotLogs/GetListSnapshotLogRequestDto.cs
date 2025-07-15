using System.Text.Json.Serialization;
using Fermion.EntityFramework.Shared.DTOs.Sorting;
using FluentValidation;

namespace Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotLogs;

public class GetListSnapshotLogRequestDto
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 25;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortOrderTypes Order { get; set; } = SortOrderTypes.Desc;
    public string? Field { get; set; } = null;
    public string? Search { get; set; } = null;
}

public class GetListSnapshotLogRequestValidator : AbstractValidator<GetListSnapshotLogRequestDto>
{
    public GetListSnapshotLogRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PerPage)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Field)
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z0-9_]+$");

        RuleFor(x => x.Order)
            .IsInEnum();

        RuleFor(x => x.Search)
            .MaximumLength(256)
            .Matches(@"^[a-zA-Z0-9\s]*$");
    }
}