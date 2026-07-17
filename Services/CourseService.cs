using BusinessObjects;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.DTO;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateCourseRequest> CreateCourse(Guid centerId, CreateCourseRequest request)
        {
            var centerCheck = await _unitOfWork.GetRepository<CenterProfile>().Entities.FirstOrDefaultAsync(c => c.Id == centerId & !c.User.IsDeleted);
            if (centerCheck == null) throw new Exception("Center not found.");

            var course = new Course
            {
                ClassName = request.ClassName,
                Grade = request.Grade,
                Subject = request.Subject,
                ClassOpenedOn = request.ClassOpenedOn,
                TuitionFee = request.TuitionFee,
                TeachingMethod = request.TeachingMethod,
                TeacherProfileId = request.TeacherProfileId,
                CenterProfileId = centerId
            };

            await _unitOfWork.GetRepository<Course>().InsertAsync(course);
            await _unitOfWork.SaveAsync();

            return request;
        }

        public async Task<(IEnumerable<CourseDetailResponse> Courses, int totalCount)> GetAllCourses(int pageNumber, int pageSize, string? subject, string? className, int? grade, TeachingMethod? teachingMethod, ClassStatus? classStatus)
        {
            IQueryable<Course> query = _unitOfWork.GetRepository<Course>().Entities
                .Include(c => c.CenterProfile)
                .Include(t => t.TeacherProfile)
                .Where(ct => !ct.IsDeleted);

            if (!string.IsNullOrWhiteSpace(subject))
            {
                query = query.Where(u => EF.Functions.Like(u.Subject, $"%{subject}%"));
            }

            if (!string.IsNullOrWhiteSpace(className))
            {
                query = query.Where(u => EF.Functions.Like(u.ClassName, $"%{className}%"));
            }

            if (teachingMethod.HasValue)
            {
                query = query.Where(x => x.TeachingMethod == teachingMethod);
            }

            if (classStatus.HasValue)
            {
                query = query.Where(x => x.ClassStatus == classStatus);
            }

            int totalCount = await query.CountAsync();

            var courses = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(ct => new CourseDetailResponse
                {
                    Id = ct.Id,
                    ClassName = ct.ClassName,
                    Subject = ct.Subject,
                    Grade = ct.Grade,
                    ClassOpenedOn = ct.ClassOpenedOn,
                    TeachingMethod = ct.TeachingMethod.ToString(),
                    TuitionFee = ct.TuitionFee,
                    TeacherName = _unitOfWork.GetRepository<User>().Entities.Where(t => t.TeacherProfile.Id == ct.TeacherProfileId).FirstOrDefault().Fullname,
                    CenterName = _unitOfWork.GetRepository<User>().Entities.Where(t => t.CenterProfile.Id == ct.CenterProfileId).FirstOrDefault().Fullname,                    
                }).ToListAsync();

            return (courses, totalCount);
        }
    }
}
