using Core.Base;
using Microsoft.AspNetCore.Mvc;
using Services.DTO;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? fullName = null, [FromQuery] string? role = null, AccountStatus? status = null)
        {
            var (users, totalCount) = await _userService.GetAllUsers(pageNumber, pageSize, fullName, role, status);
            return Ok(new { users, totalCount });
        }

        [HttpGet("Centers")]
        public async Task<IActionResult> GetAllCenters([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? centerName = null, [FromQuery] string? address = null, AccountStatus? status = null)
        {
            var (centers, totalCount) = await _userService.GetAllCenters(pageNumber, pageSize, centerName, address, status);
            return Ok(new { centers, totalCount });
        }

        [HttpGet("Teachers/{centerId}")]
        public async Task<IActionResult> GetTeachersByCenterId(Guid centerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string? fullName = null, AccountStatus? status = null)
        {
            var (teachers, totalCount) = await _userService.GetTeachersByCenter(centerId, pageNumber, pageSize, fullName, status);
            return Ok(new { teachers, totalCount });
        }

        [HttpGet("Center/{UserId}")]
        public async Task<IActionResult> GetCenterByUserId(Guid UserId)
        {
            try
            {
                var center = await _userService.GetCenterByUserId(UserId);
                return Ok(center);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("Teacher/{UserId}")]
        public async Task<IActionResult> GetTeacherByUserId(Guid UserId)
        {
            try
            {
                var teacher = await _userService.GetTeacherByUserId(UserId);
                return Ok(teacher);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("CreateCenter")]
        public async Task<IActionResult> CreateCenter(CreateCenterUser request)
        {
            try
            {
                var center = await _userService.AddNewCenterAccount(request);
                return Ok(center);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("CreateTeacher/{userId}")]
        public async Task<IActionResult> CreateTeacher(CreateTeacherRequest request, Guid userId)
        {
            try
            {
                var teacherRequest = await _userService.AddNewTeacherAccount(request, userId);
                return Ok(teacherRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Status/{userId}")]
        public async Task<IActionResult> UpdateAccountStatus(Guid userId, int status)
        {
            try
            {
                var user = await _userService.UpdateAccountStatus(userId, status);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("UpdateCenter/{userId}")]
        public async Task<IActionResult> UpdateCenter(Guid userId, CenterUpdateRequest request)
        {
            try
            {
                var center = await _userService.UpdateCenterInformation(userId, request);
                return Ok(center);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("UpdateTeacher/{userId}")]
        public async Task<IActionResult> UpdateTeacher(Guid userId, TeacherUpdateRequest request)
        {
            try
            {
                var center = await _userService.UpdateTeacherInformation(userId, request);
                return Ok(center);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                var user = await _userService.DeleteUser(userId);
                return Ok(user);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
