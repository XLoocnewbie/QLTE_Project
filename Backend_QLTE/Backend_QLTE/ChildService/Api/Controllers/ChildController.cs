using Backend_QLTE.ChildService.Application.DTOs.Client.Child;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_QLTE.ChildService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChildController : ControllerBase
    {
        private readonly IChildService _childService;

        public ChildController(IChildService childService)
        {
            _childService = childService;
        }

        [HttpGet("GetAllParentsWithChildren")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllParentsWithChildren()
        {
            var result = await _childService.GetAllParentsWithChildrenAsync();
            return Ok(result);
        }

        [HttpGet("GetChildrenByUserId")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> GetChildrenByUserId(string userId)
        {
            var result = await _childService.GetChildrenByUserIdAsync(userId);
            return Ok(result);
        }


        [HttpPost("CreateChild")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> CreateChild ([FromForm]CreateChildRequestDTO request)
        {
            var result = await _childService.CreateChildAsync(request);
            return Ok(result);
        }

        [HttpPut("UpdateChild")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> UpdateChild([FromForm] UpdateChildRequestDTO request)
        {
            var result = await _childService.UpdateChildAsync(request);
            return Ok(result);
        }

        [HttpDelete("DeleteChild")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> DeleteChild(Guid childrenId)
        {
            var result = await _childService.DeleteChildAsync(childrenId);
            return Ok(result);
        }

        [HttpGet("GetChildByUserId")]
        [Authorize(Roles = "Parent,Children,Admin")]
        public async Task<IActionResult> GetChildByUserId(string userId)
        {
            var result = await _childService.GetChildByUserIdAsync(userId);
            return Ok(result);
        }
    }
}
