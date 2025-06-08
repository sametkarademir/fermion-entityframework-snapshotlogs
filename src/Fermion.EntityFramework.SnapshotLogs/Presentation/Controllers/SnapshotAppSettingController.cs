using Fermion.EntityFramework.Shared.DTOs.Pagination;
using Fermion.EntityFramework.SnapshotLogs.Applications.DTOs.SnapshotAppSettings;
using Fermion.EntityFramework.SnapshotLogs.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fermion.EntityFramework.SnapshotLogs.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SnapshotAppSettingController(ISnapshotAppSettingAppService snapshotAppSettingAppService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SnapshotAppSettingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SnapshotAppSettingResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await snapshotAppSettingAppService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PageableResponseDto<SnapshotAppSettingResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPageableAndFilterAsync(
        [FromQuery] GetListSnapshotAppSettingRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await snapshotAppSettingAppService.GetPageableAndFilterAsync(request, cancellationToken);
        return Ok(result);
    }
}