using Microsoft.AspNetCore.Mvc;
using myLeave.Services.Interfaces;
using myLeave.Dtos;
using Microsoft.AspNetCore.Authorization;
namespace myLeave.Controllers

{
    [Authorize(Roles = "Admin,Employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class LeavesController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeavesController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpGet("byEmail/{email}")]
        public async Task<IActionResult> GetLeavesByEmail(string email)
        {
            try
            {
                var leaves = await _leaveService.GetLeavesByEmailAsync(email);

                if (leaves == null || !leaves.Any())
                    return Ok(new List<Leave>());

                return Ok(leaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching leave records", error = ex.Message });
            }
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyLeave([FromBody] LeaveCreateDto dto)
        {
            try
            {
                var success = await _leaveService.ApplyLeaveAsync(dto);
                if (!success)
                    return BadRequest(new { message = "Invalid Employee ID or duplicate leave entry" });

                return Ok(new { message = "Leave applied successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error applying leave", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLeaves()
        {
            try
            {
                var leaves = await _leaveService.GetAllLeavesAsync();
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching leaves", error = ex.Message });
            }
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingLeaves()
        {
            try
            {
                var pendingLeaves = await _leaveService.GetPendingLeavesAsync();
                return Ok(pendingLeaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching pending leaves", error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateLeaveStatus(int id, [FromBody] string status)
        {
            try
            {
                var success = await _leaveService.UpdateLeaveStatusAsync(id, status);
                if (!success)
                    return NotFound(new { message = "Leave not found or already updated" });

                return Ok(new { message = $"Leave {status.ToLower()} successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating leave status", error = ex.Message });
            }
        }
    }

}
