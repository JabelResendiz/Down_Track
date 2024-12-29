
using DownTrack.Application.DTO;
using DownTrack.Application.IServices;
using DownTrack.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DownTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SectionController : ControllerBase
    {
        private readonly ISectionServices _sectionService;

        public SectionController(ISectionServices sectionServices)
        {
            _sectionService = sectionServices;
        }

        [HttpPost]
        [Route("POST")]

        public async Task<IActionResult> CreateSection(SectionDto section)
        {
            await _sectionService.CreateAsync(section);

            return Ok("Section added successfully");
        }

        [HttpGet]
        [Route("GET_ALL")]

        public async Task<ActionResult<IEnumerable<Section>>> GetAllSections()
        {
            var results = await _sectionService.ListAsync();

            return Ok(results);

        }

        [HttpPut]
        [Route("PUT")]

        public async Task<IActionResult> UpdateSection(SectionDto section)
        {
            var result = await _sectionService.UpdateAsync(section);
            return Ok(result);
        }

        [HttpDelete]
        [Route("Delete")]

        public async Task<IActionResult> DeleteSection(int sectionId)
        {
            await _sectionService.DeleteAsync(sectionId);

            return Ok("Section deleted successfully");
        }
    }

}