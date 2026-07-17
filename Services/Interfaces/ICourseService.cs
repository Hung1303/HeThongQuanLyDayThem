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
        Task<CreateCourseRequest> CreateCourse(Guid centerId, CreateCourseRequest request);
    }
}
