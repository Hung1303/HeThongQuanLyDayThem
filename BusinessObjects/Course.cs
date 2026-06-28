using Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Course : BaseEntity
    {
        public string ClassName { get; set; }
        [Range(6, 12, ErrorMessage ="Only from grade 6 to 12")]
        public int Grade { get; set; }
        public string Subject { get; set; }
        public DateTime ClassOpenedOn { get; set; }
        public TeachingMethod TeachingMethod { get; set; }
        public decimal TuitionFee { get; set; }
        public ClassStatus ClassStatus { get; set; } = 0;

        public Guid TeacherProfileId { get; set; }
        public virtual TeacherProfile TeacherProfile { get; set; }
        public Guid CenterProfileId { get; set; }
        public virtual CenterProfile CenterProfile { get; set; }
    }
}
