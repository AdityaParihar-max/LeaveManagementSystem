using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using myLeave.Services.Interfaces;

namespace myLeave.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public SearchController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetLeavesByEmail(string email)
        {
            var leaves = await _leaveService.GetLeavesByEmailAsync(email);
            return Ok(leaves);
        }
    }
}
