using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAssemblies;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fermion.EntityFramework.SnapshotLogs.Presentation.Controllers;

[ApiController]
[Route("api/snapshot-assemblies")]
public class SnapshotAssemblyController(ISnapshotAssemblyAppService snapshotAssemblyAppService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SnapshotAssemblyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SnapshotAssemblyResponseDto>> GetByIdAsync([FromRoute(Name = "id")] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await snapshotAssemblyAppService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PageableResponseDto<SnapshotAssemblyResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetPageableAndFilterAsync([FromQuery] GetListSnapshotAssemblyRequestDto request, CancellationToken cancellationToken = default)
    {
        var result = await snapshotAssemblyAppService.GetPageableAndFilterAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("cleanup")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CleanupOldSnapshotAssembliesAsync([FromQuery] DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var result = await snapshotAssemblyAppService.CleanupOldSnapshotAssemblyAsync(olderThan, cancellationToken);
        return Ok(result);
    }
}