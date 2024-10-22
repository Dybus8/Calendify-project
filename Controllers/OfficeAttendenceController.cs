using Microsoft.AspNetCore.Mvc;
using StarterKit.Models;
using StarterKit.Services;

namespace StarterKit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfficeAttendanceController : ControllerBase
    {
        private readonly IOfficeAttendanceService _officeAttendanceService;

        public OfficeAttendanceController(IOfficeAttendanceService officeAttendanceService)
        {
            _officeAttendanceService = officeAttendanceService;
        }

        [HttpPost]
        public IActionResult BookOfficeAttendance([FromBody] OfficeAttendanceModel model)
        {
            var result = _officeAttendanceService.BookOfficeAttendance(model);
            if (result.Success)
            {
                return Ok(new { message = "Office attendance booked successfully" });
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpPut]
        public IActionResult UpdateOfficeAttendance([FromBody] OfficeAttendanceModel model)
        {
            var result = _officeAttendanceService.UpdateOfficeAttendance(model);
            if (result.Success)
            {
                return Ok(new { message = "Office attendance updated successfully" });
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpDelete]
        public IActionResult CancelOfficeAttendance([FromBody] OfficeAttendanceModel model)
        {
            var result = _officeAttendanceService.CancelOfficeAttendance(model);
            if (result.Success)
            {
                return Ok(new { message = "Office attendance cancelled successfully" });
            }
            return BadRequest(new { message = result.Message });
        }
    }
}