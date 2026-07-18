using BusinessObjects;
using Core.Base;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICourseService
    {
        Task<(IEnumerable<CourseDetailResponse> Courses, int totalCount)> GetAllCourses(int pageNumber, int pageSize, string? subject, string? className, int? grade, TeachingMethod? teachingMethod, ClassStatus? classStatus);
        Task<(IEnumerable<CourseDetailResponse> Courses, int totalCount)> GetCoursesByCenter(Guid centerId, int pageNumber, int pageSize, string? subject, string? className, int? grade, TeachingMethod? teachingMethod, ClassStatus? classStatus);
        Task<(IEnumerable<CourseDetailResponse> Courses, int totalCount)> GetCoursesByCenterPublic(Guid centerId, int pageNumber, int pageSize, string? subject, string? className, int? grade, TeachingMethod? teachingMethod, ClassStatus? classStatus);
        Task<(IEnumerable<CourseDetailResponse> Courses, int totalCount)> GetCoursesByTeacher(Guid teacherId, int pageNumber, int pageSize, string? subject, string? className, int? grade, TeachingMethod? teachingMethod, ClassStatus? classStatus);
        Task<CourseDetailResponse> GetCourseById(Guid courseId);
        Task<CreateCourseRequest> CreateCourse(Guid centerId, CreateCourseRequest request);
        Task<CourseDetailResponse> UpdateCourse(Guid courseId, CourseUpdateRequest request);
        Task<string> ApproveCourseStatus(Guid courseId, int status);
        Task<string> UpdateCourseStatus(Guid courseId, int status);
        Task<string> DeleteCourse(Guid courseId);
    }
}
