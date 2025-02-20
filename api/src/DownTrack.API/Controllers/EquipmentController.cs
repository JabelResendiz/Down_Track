using DownTrack.Application.DTO;
using DownTrack.Application.DTO.Paged;
using DownTrack.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace DownTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentQueryServices _equipmentQueryService;
    private readonly IEquipmentCommandServices _equipmentCommandService;

    public EquipmentController(IEquipmentCommandServices equipmentCommandServices,
                                IEquipmentQueryServices equipmentQueryServices)
    {
        _equipmentQueryService = equipmentQueryServices;
        _equipmentCommandService = equipmentCommandServices;
    }

    #region Command

    [HttpPost]
    [Route("POST")]
    public async Task<IActionResult> CreateEquipment(EquipmentDto equipment)
    {
        await _equipmentCommandService.CreateAsync(equipment);

        return Ok("Equipment added successfully");
    }

    [HttpPut]
    [Route("PUT")]

    public async Task<IActionResult> UpdateEquipment(EquipmentDto equipment)
    {
        var result = await _equipmentCommandService.UpdateAsync(equipment);
        return Ok(result);
    }

    [HttpDelete]
    [Route("{equipmentId}")]

    public async Task<IActionResult> DeleteEquipment(int equipmentId)
    {
        await _equipmentCommandService.DeleteAsync(equipmentId);

        return Ok("Equipment deleted successfully");
    }

    #endregion

    #region Query

    [HttpGet]
    [Route("GET")]

    public async Task<ActionResult<GetEquipmentDto>> GetUserById(int equipmentId)
    {
        var result = await _equipmentQueryService.GetByIdAsync(equipmentId);

        if (result == null)
            return NotFound($"Equipment with ID {equipmentId} not found");

        return Ok(result);

    }

    [HttpGet]
    [Route("GetPaged")]

    public async Task<IActionResult> GetPagedEquipment([FromQuery] PagedRequestDto paged)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var result = await _equipmentQueryService.GetAllPagedResultAsync(paged);

        return Ok(result);

    }

    [HttpGet("equipments/section-manager/{sectionManagerId}")]
    public async Task<IActionResult> GetPagedEquipmentsBySectionManagerId([FromQuery] PagedRequestDto paged, int sectionManagerId)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var result = await _equipmentQueryService.GetPagedEquipmentsBySectionManagerIdAsync(paged, sectionManagerId);

        return Ok(result);
    }

    [HttpGet("equipments/section/{sectionId}")]
    public async Task<IActionResult> GetPagedEquipmentsBySectionId([FromQuery] PagedRequestDto paged, int sectionId)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var result = await _equipmentQueryService.GetPagedEquipmentsBySectionIdAsync(paged, sectionId);

        return Ok(result);
    }

    [HttpGet("equipments/department/{departmentId}")]
    public async Task<IActionResult> GetPagedEquipmentsByDepartmentId([FromQuery] PagedRequestDto paged, int departmentId)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var result = await _equipmentQueryService.GetPagedEquipmentsByDepartmentIdAsync(paged, departmentId);

        return Ok(result);
    }

    [HttpGet("active equipment")]
    public async Task<IActionResult> GetActiveEquipment([FromQuery] PagedRequestDto paged)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var result = await _equipmentQueryService.GetActiveEquipment(paged);

        return Ok(result);
    }

    [HttpGet("SearchByName")]
    public async Task<IActionResult> GetPagedAllEquipmentsByName([FromQuery] PagedRequestDto paged, string equipmentName)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var result = await _equipmentQueryService.GetPagedEquipmentsByNameAsync(paged, equipmentName);

        return Ok(result);
    }

    [HttpGet("SearchByNameAndBySectionManagerId/{sectionManagerId}")]
    public async Task<IActionResult> GetPagedAllEquipmentsByNameAndSectionManagerId([FromQuery] PagedRequestDto paged, string equipmentName, int sectionManagerId)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var result = await _equipmentQueryService.GetPagedEquipmentsByNameAndSectionManagerAsync(paged, equipmentName, sectionManagerId);

        return Ok(result);
    }

    [HttpGet("Equipment_With_More_Than_Three_Maintenances_In_Last_Year")]
    public async Task<IActionResult> GetPagedEquipmentWithMoreThan3Maintenances([FromQuery] PagedRequestDto paged)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var result = await _equipmentQueryService.GetPagedEquipmentsWith3MaintenancesAsync(paged);

        return Ok(result);

    }

    [HttpGet]
    [Route("Get_Transferred_Equipments_By_DepartmentId")]

    public async Task<IActionResult> GetTransferredEquipmentsByDepartment([FromQuery] PagedRequestDto paged, int departmentId)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var result = await _equipmentQueryService.GetTransferredEquipmentsByDepartmentAsync(paged, departmentId);

        return Ok(result);

    }

    #endregion


}