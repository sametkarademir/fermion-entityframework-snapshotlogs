using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAssemblies;
using Fermion.EntityFramework.SnapshotLogs.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fermion.EntityFramework.SnapshotLogs.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SnapshotAssemblyController(ISnapshotAssemblyAppService snapshotAssemblyAppService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SnapshotAssemblyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SnapshotAssemblyResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await snapshotAssemblyAppService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PageableResponseDto<SnapshotAssemblyResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPageableAndFilterAsync(
        [FromQuery] GetListSnapshotAssemblyRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await snapshotAssemblyAppService.GetPageableAndFilterAsync(request, cancellationToken);
        return Ok(result);
    }
}