using Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class CenterProfile : BaseEntity
    {
        public string CenterName { get;set; }
        public string Address { get; set; }
        public DateTime EstablishDate { get; set; }
        public string? OwnerName { get; set; }
        public string? ContactPhoneNumber { get; set; }
        public string? ContactEmail { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<TeacherProfile> TeacherProfiles { get; set; } = new List<TeacherProfile>();
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    }
}
