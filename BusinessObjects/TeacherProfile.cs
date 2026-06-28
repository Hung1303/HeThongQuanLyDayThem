using Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class TeacherProfile : BaseEntity
    {
        public int YearOfExperience { get; set; } = 0;
        public Degree Qualification { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid CenterProfileId { get; set; }
        public virtual CenterProfile CenterProfile { get; set; }

        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
