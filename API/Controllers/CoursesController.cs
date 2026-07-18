using Core.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTO;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllCourses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5,
            string? subject = null, string? className = null, int? grade = null, TeachingMethod? teachingMethod = null, ClassStatus? classStatus = null)
        {
            var (courses, totalCount) = await _courseService.GetAllCourses(pageNumber, pageSize, subject, className, grade, teachingMethod, classStatus);
            return Ok(new { courses, totalCount });
        }

        [HttpGet("All/{centerId}")]
        public async Task<IActionResult> GetCoursesByCenter(Guid centerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5,
            string? subject = null, string? className = null, int? grade = null, TeachingMethod? teachingMethod = null, ClassStatus? classStatus = null)
        {
            var (courses, totalCount) = await _courseService.GetCoursesByCenter(centerId, pageNumber, pageSize, subject, className, grade, teachingMethod, classStatus);
            return Ok(new { courses, totalCount });
        }

        [HttpPost("{centerId}")]
        public async Task<IActionResult> CreateCourseByCenter(Guid centerId, CreateCourseRequest request)
        {
            try
            {
                var course = await _courseService.CreateCourse(centerId, request);
                return Ok(course);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("UpdateCourse/{courseId}")]
        public async Task<IActionResult> UpdateCourse(Guid courseId, CourseUpdateRequest request)
        {
            try
            {
                var course = await _courseService.UpdateCourse(courseId, request);
                return Ok(course);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Approve/{courseId}")]
        public async Task<IActionResult> ApproveCourse(Guid courseId, int status)
        {
            try
            {
                var course = await _courseService.ApproveCourseStatus(courseId, status);
                return Ok(course);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("ChangeStatus/{courseId}")]
        public async Task<IActionResult> UpdateCourseStatus(Guid courseId, int status)
        {
            try
            {
                var course = await _courseService.UpdateCourseStatus(courseId, status);
                return Ok(course);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{courseId}")]
        public async Task<IActionResult> DeleteCourse(Guid courseId)
        {
            try
            {
                var course = await _courseService.DeleteCourse(courseId);
                return Ok(course);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
