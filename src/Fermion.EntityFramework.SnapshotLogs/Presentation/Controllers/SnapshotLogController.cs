using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotLogs;
using Fermion.EntityFramework.SnapshotLogs.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fermion.EntityFramework.SnapshotLogs.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SnapshotLogController(ISnapshotLogAppService snapshotLogAppService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SnapshotLogResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SnapshotLogResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await snapshotLogAppService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PageableResponseDto<SnapshotLogResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPageableAndFilterAsync(
        [FromQuery] GetListSnapshotLogRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await snapshotLogAppService.GetPageableAndFilterAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("cleanup")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CleanupOldSnapshotLogsAsync(
        [FromQuery] DateTime olderThan,
        CancellationToken cancellationToken = default)
    {
        if (olderThan == default)
        {
            return BadRequest("Invalid date provided.");
        }

        await snapshotLogAppService.CleanupOldSnapshotLogAsync(olderThan, cancellationToken);

        return NoContent();
    }
}