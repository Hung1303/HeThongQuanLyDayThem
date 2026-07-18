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
            var centerCheck = await _unitOfWork.GetRepository<CenterProfile>().Entities.FirstOrDefaultAsync(c => c.Id == centerId && !c.User.IsDeleted);
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
                    ClassStatus = ct.ClassStatus.ToString(),
                    TuitionFee = ct.TuitionFee,
                    TeacherName = _unitOfWork.GetRepository<User>().Entities.Where(t => t.TeacherProfile.Id == ct.TeacherProfileId).FirstOrDefault().Fullname,
                    CenterName = _unitOfWork.GetRepository<User>().Entities.Where(t => t.CenterProfile.Id == ct.CenterProfileId).FirstOrDefault().Fullname,                    
                }).ToListAsync();

            return (courses, totalCount);
        }

        public async Task<(IEnumerable<CourseDetailResponse> Courses, int totalCount)> GetCoursesByCenter(Guid centerId, int pageNumber, int pageSize, string? subject, string? className, int? grade, TeachingMethod? teachingMethod, ClassStatus? classStatus)
        {
            var centerCheck = await _unitOfWork.GetRepository<CenterProfile>().Entities.FirstOrDefaultAsync(c => c.Id == centerId && !c.User.IsDeleted);
            if (centerCheck == null) throw new Exception("Center not found.");

            IQueryable<Course> query = _unitOfWork.GetRepository<Course>().Entities
                .Include(c => c.CenterProfile)
                .Include(t => t.TeacherProfile)
                .Where(ct => ct.CenterProfileId == centerId & !ct.IsDeleted);

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
                    ClassStatus = ct.ClassStatus.ToString(),
                    TuitionFee = ct.TuitionFee,
                    TeacherName = _unitOfWork.GetRepository<User>().Entities.Where(t => t.TeacherProfile.Id == ct.TeacherProfileId).FirstOrDefault().Fullname,
                    CenterName = _unitOfWork.GetRepository<User>().Entities.Where(t => t.CenterProfile.Id == ct.CenterProfileId).FirstOrDefault().Fullname,
                }).ToListAsync();

            return (courses, totalCount);
        }

        public async Task<CourseDetailResponse> UpdateCourse(Guid courseId, CourseUpdateRequest request)
        {
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted);
            if (course == null) throw new Exception("Course not found.");

            course.ClassName = request.ClassName;
            course.Grade = request.Grade;
            course.Subject = request.Subject;
            course.ClassOpenedOn = request.ClassOpenedOn;
            course.TuitionFee = request.TuitionFee;
            course.TeacherProfileId = request.TeacherProfileId;

            await _unitOfWork.GetRepository<Course>().UpdateAsync(course);
            await _unitOfWork.SaveAsync();

            var response = new CourseDetailResponse
            {
                Id = course.Id,
                ClassName = course.ClassName,
                Subject = course.Subject,
                Grade = course.Grade,
                ClassOpenedOn = course.ClassOpenedOn,
                TeachingMethod = course.TeachingMethod.ToString(),
                ClassStatus = course.ClassStatus.ToString(),
                TuitionFee = course.TuitionFee,
                TeacherName = _unitOfWork.GetRepository<User>().Entities.Where(t => t.TeacherProfile.Id == course.TeacherProfileId).FirstOrDefault().Fullname,
                CenterName = _unitOfWork.GetRepository<User>().Entities.Where(t => t.CenterProfile.Id == course.CenterProfileId).FirstOrDefault().Fullname
            };

            return response;
        }

        public async Task<string> ApproveCourseStatus(Guid courseId, int status)
        {
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted);
            if (course == null) throw new Exception("Course not found.");
            var result = "";

            if (status == 1 || status == 5)
            {
                switch (status)
                {
                    case 1:
                        course.ClassStatus = ClassStatus.OpenForAdmission;
                        result = "Course status updated to OpenForAdmission";
                        break;

                    case 5:
                        course.ClassStatus = ClassStatus.Rejected;
                        result = "Course status updated to Rejected.";
                        break;
                }
            }
            else
            {
                throw new Exception("The value must be 1 and 5");
            }

            await _unitOfWork.GetRepository<Course>().UpdateAsync(course);
            await _unitOfWork.SaveAsync();

            return result;
        }


        public async Task<string> DeleteCourse(Guid courseId)
        {
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted);
            if (course == null) throw new Exception("Course not found.");

            course.IsDeleted = true;

            await _unitOfWork.GetRepository<Course>().UpdateAsync(course);
            await _unitOfWork.SaveAsync();

            return "This course has been deleted.";
        }

        public async Task<string> UpdateCourseStatus(Guid courseId, int status)
        {
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted && (c.ClassStatus == ClassStatus.Pending || c.ClassStatus != ClassStatus.Rejected));
            if (course == null) throw new Exception("Course not found.");
            var result = "";

            if (status < 1 || status > 4) throw new Exception("The value must be 2 and 3 and 5");
            switch (status)
            {
                case 1:
                    course.ClassStatus = ClassStatus.OpenForAdmission;
                    result = "Course status updated to OpenForAdmission";
                    break;

                case 2:
                    course.ClassStatus = ClassStatus.AdmissionCompleted;
                    result = "Course status updated to AdmissionCompleted";
                    break;

                case 3:
                    course.ClassStatus = ClassStatus.Completed;
                    result = "Course status updated to Completed.";
                    break;

                case 4:
                    course.ClassStatus = ClassStatus.Hidden;
                    result = "Course status updated to Hidden.";
                    break;
            }

            await _unitOfWork.GetRepository<Course>().UpdateAsync(course);
            await _unitOfWork.SaveAsync();

            return result;
        }

        public Task<(IEnumerable<CourseDetailResponse> Courses, int totalCount)> GetCoursesByCenterPublic(Guid centerId, int pageNumber, int pageSize, string? subject, string? className, int? grade, TeachingMethod? teachingMethod, ClassStatus? classStatus)
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable<CourseDetailResponse> Courses, int totalCount)> GetCoursesByTeacher(Guid teacherId, int pageNumber, int pageSize, string? subject, string? className, int? grade, TeachingMethod? teachingMethod, ClassStatus? classStatus)
        {
            throw new NotImplementedException();
        }

        public Task<CourseDetailResponse> GetCourseById(Guid courseId)
        {
            throw new NotImplementedException();
        }
    }
}
