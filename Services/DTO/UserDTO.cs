using Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class UserDetailResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string UserRole { get; set; }
        public string AccountStatus { get; set; }
    }

    public class TeacherDetailResponse
    {
        public Guid Id { get; set; }
        public Guid UId { get; set; }
        public string Fullname { get; set; }
        public string BirthYear { get; set; }
        public string Gender { get; set; }
        public int YearOfExperience { get; set; }
        public string Qualification { get; set; }
        public string Status { get; set; }
    }

    public class CreateCenterUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string CenterName { get; set; }
        public string Address { get; set; }
        public DateTime EstablishDate { get; set; }
        public string? ContactPhoneNumber { get; set; }
        public string? ContactEmail { get; set; }
    }

    public class CreateTeacherRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public int YearOfExperience { get; set; }
        public Degree Qualification { get; set; }

    }

    public class CenterUserResponse
    {
        public Guid Id { get; set; }
        public Guid UId { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string? PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string UserRole { get; set; }
        public string CenterName { get; set; }
        public string Address { get; set; }
        public DateTime EstablishDate { get; set; }
        public string? ContactPhoneNumber { get; set; }
        public string? ContactEmail { get; set; }
        public string Status { get; set; }
    }

    public class CenterUpdateRequest
    {
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string? PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public string CenterName { get; set; }
        public string Address { get; set; }
        public string? ContactPhoneNumber { get; set; }
        public string? ContactEmail { get; set; }
    }
}
