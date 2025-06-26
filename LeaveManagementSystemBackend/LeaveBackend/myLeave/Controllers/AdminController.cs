
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using myLeave.Services.Interfaces;

namespace myLeave.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public AdminController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingLeaves()
        {
            var leaves = await _leaveService.GetPendingLeavesAsync();
            return Ok(leaves);
        }


        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateLeaveStatus(int id, [FromBody] StatusUpdateDto dto)
        {
            var success = await _leaveService.UpdateLeaveStatusAsync(id, dto.Status);
            if (!success) return NotFound();
            return Ok(new { message = "Status updated successfully" });
        }
    }

    public class StatusUpdateDto
    {
        public string Status { get; set; } = string.Empty;
    }

}

