using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAppSettings;
using Fermion.EntityFramework.SnapshotLogs.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fermion.EntityFramework.SnapshotLogs.Presentation.Controllers;

[ApiController]
[Route("api/snapshot-app-settings")]
public class SnapshotAppSettingController(ISnapshotAppSettingAppService snapshotAppSettingAppService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SnapshotAppSettingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SnapshotAppSettingResponseDto>> GetByIdAsync([FromRoute(Name = "id")] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await snapshotAppSettingAppService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PageableResponseDto<SnapshotAppSettingResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetPageableAndFilterAsync([FromQuery] GetListSnapshotAppSettingRequestDto request, CancellationToken cancellationToken = default)
    {
        var result = await snapshotAppSettingAppService.GetPageableAndFilterAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("cleanup")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CleanupOldSnapshotAppSettingsAsync([FromQuery] DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var result = await snapshotAppSettingAppService.CleanupOldSnapshotAppSettingAsync(olderThan, cancellationToken);
        return Ok(result);
    }
}