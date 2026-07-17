using Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class CreateCourseRequest
    {
        public string ClassName { get; set; }
        [Range(6, 12, ErrorMessage = "Only from grade 6 to 12")]
        public int Grade { get; set; }
        public string Subject { get; set; }
        public DateTime ClassOpenedOn { get; set; }
        public TeachingMethod TeachingMethod { get; set; }
        public decimal TuitionFee { get; set; }
        public Guid TeacherProfileId { get; set; }
    }

    public class CourseDetailResponse
    {
        public Guid Id { get; set; }
        public string ClassName { get; set; }
        [Range(6, 12, ErrorMessage = "Only from grade 6 to 12")]
        public int Grade { get; set; }
        public string Subject { get; set; }
        public DateTime ClassOpenedOn { get; set; }
        public string TeachingMethod { get; set; }
        public decimal TuitionFee { get; set; }
        public string TeacherName { get; set; }
        public string CenterName { get; set; }
    }
}
