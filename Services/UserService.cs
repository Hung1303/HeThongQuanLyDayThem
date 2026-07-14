using BusinessObjects;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.DTO;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateCenterUser> AddNewCenterAccount(CreateCenterUser request)
        {
            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(request.Email, emailPattern))
                throw new Exception("Incorrect email.");

            var namePattern = @"^([A-ZÀ-Ỹ][a-zà-ỹ]+)(\s[A-ZÀ-Ỹ][a-zà-ỹ]+)*$";
            if (string.IsNullOrWhiteSpace(request.Fullname) || !Regex.IsMatch(request.Fullname.Trim(), namePattern))
                throw new Exception("Incorrect naming.");

            var checkUser = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(u => u.Email == request.Email || u.Username == request.Username || u.PhoneNumber == request.PhoneNumber);
            if (checkUser != null)
            {
                if (checkUser.Email == request.Email)
                    throw new Exception("Duplicate email.");
                else if (checkUser.Username == request.Username)
                    throw new Exception("Duplicate username.");
                else
                    throw new Exception("Duplicate phone number.");
            }

            var checkCenter = await _unitOfWork.GetRepository<CenterProfile>().Entities.AnyAsync(c => c.ContactPhoneNumber == request.ContactPhoneNumber);
            if (checkCenter) throw new Exception("Contact phone number is being used by a different user.");

            var user = new User
            {
                Username = request.Username,
                Fullname = request.Fullname,
                Password = request.Password,
                Email = request.Email,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                UserRole = Role.Center,
                AccountStatus = AccountStatus.Pending
            };

            var center = new CenterProfile
            {
                Address = request.Address,
                CenterName = request.CenterName,
                ContactEmail = request.ContactEmail,
                ContactPhoneNumber = request.ContactPhoneNumber,
                OwnerName = user.Fullname,
                EstablishDate = request.EstablishDate,
                UserId = user.Id,
            };

            try 
            { 
                _unitOfWork.BeginTransaction();
                await _unitOfWork.GetRepository<User>().InsertAsync(user);
                await _unitOfWork.GetRepository<CenterProfile>().InsertAsync(center);
                await _unitOfWork.SaveAsync();

                _unitOfWork.CommitTransaction();
            }
            catch
            {
                _unitOfWork.RollBack();
                throw;
            }

            return request;
        }

        public async Task<CreateTeacherRequest> AddNewTeacherAccount(CreateTeacherRequest request, Guid userId)
        {
            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(request.Email, emailPattern))
                throw new Exception("Incorrect email.");

            var namePattern = @"^([A-ZÀ-Ỹ][a-zà-ỹ]+)(\s[A-ZÀ-Ỹ][a-zà-ỹ]+)*$";
            if (string.IsNullOrWhiteSpace(request.Fullname) || !Regex.IsMatch(request.Fullname.Trim(), namePattern))
                throw new Exception("Incorrect naming.");

            var checkUser = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(u => u.Email == request.Email || u.Username == request.Username || u.PhoneNumber == request.PhoneNumber);
            if (checkUser != null)
            {
                if (checkUser.Email == request.Email)
                    throw new Exception("Duplicate email.");
                else if (checkUser.Username == request.Username)
                    throw new Exception("Duplicate username.");
                else
                    throw new Exception("Duplicate phone number.");
            }

            var checkCenter = await _unitOfWork.GetRepository<CenterProfile>().Entities.FirstOrDefaultAsync(c => c.UserId == userId);
            if(checkCenter == null)
            {
                throw new Exception("Center not found.");
            }

            var user = new User
            {
                Username = request.Username,
                Fullname = request.Fullname,
                Password = request.Password,
                Email = request.Email,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                UserRole = Role.Center,
                AccountStatus = AccountStatus.Pending
            };

            var teacher = new TeacherProfile
            {
                YearOfExperience = request.YearOfExperience,
                Qualification = request.Qualification,
                CenterProfileId = checkCenter.Id,
                UserId = user.Id
            };

            try
            {
                _unitOfWork.BeginTransaction();
                await _unitOfWork.GetRepository<User>().InsertAsync(user);
                await _unitOfWork.GetRepository<TeacherProfile>().InsertAsync(teacher);
                await _unitOfWork.SaveAsync();

                _unitOfWork.CommitTransaction();
            }
            catch
            {
                _unitOfWork.RollBack();
                throw;
            }

            return request;
        }

        public async Task<string> UpdateAccountStatus(Guid userId, int status)
        {
            var result = "";
            var user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null) throw new Exception("Account not found.");

            if (status < 1 || status > 2) throw new Exception("The value must be between 1 and 2.");

            switch (status)
            {
                case 1:
                    user.AccountStatus = AccountStatus.Active;
                    result = "Account status updated to Active";
                    break;

                case 2:
                    user.AccountStatus = AccountStatus.Deactivated;
                    result = "Account status updated to Deactivated.";
                    break;
            }

            await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return result;
        }

        public async Task<(IEnumerable<UserDetailResponse> Users, int TotalCount)> GetAllUsers(int pageNumber, int pageSize, string? fullName, string? role, AccountStatus? status)
        {
            IQueryable<User> query = _unitOfWork.GetRepository<User>().Entities.Where(q => !q.IsDeleted);

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                query = query.Where(u => EF.Functions.Like(u.Fullname, $"%{fullName}%"));
            }

            if (!string.IsNullOrWhiteSpace(role) &&
                Enum.TryParse<Role>(role, true, out var parsedRole))
            {
                query = query.Where(u => u.UserRole == parsedRole);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.AccountStatus == status);
            }

            int totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDetailResponse
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Password = u.Password,
                    Fullname = u.Fullname,
                    PhoneNumber = u.PhoneNumber,
                    DateOfBirth = u.DateOfBirth,
                    Gender = u.Gender.ToString(),
                    UserRole = u.UserRole.ToString(),
                    AccountStatus = u.AccountStatus.ToString()
                }).ToListAsync();

            return (users, totalCount);
        }

        public async Task<(IEnumerable<CenterUserResponse> Centers, int TotalCount)> GetAllCenters(int pageNumber, int pageSize, string? centerName, string? address, AccountStatus? status)
        {
            IQueryable<CenterProfile> query = _unitOfWork.GetRepository<CenterProfile>().Entities.Include(u => u.User).Where(q => !q.User.IsDeleted);

            if (!string.IsNullOrWhiteSpace(centerName))
            {
                query = query.Where(u => EF.Functions.Like(u.CenterName, $"%{centerName}%"));
            }

            if (!string.IsNullOrWhiteSpace(address))
            {
                query = query.Where(u => EF.Functions.Like(u.Address, $"%{address}%"));
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.User.AccountStatus == status);
            }

            int totalCount = await query.CountAsync();

            var centers = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new CenterUserResponse
                {
                    Id = u.Id,
                    UId = u.User.Id,
                    Email = u.User.Email,
                    Fullname = u.User.Fullname,
                    PhoneNumber = u.User.PhoneNumber,
                    Gender = u.User.Gender.ToString(),
                    UserRole = u.User.UserRole.ToString(),
                    CenterName = u.CenterName,
                    Address = u.Address,
                    EstablishDate = u.EstablishDate,
                    ContactEmail = u.ContactEmail,
                    ContactPhoneNumber = u.ContactPhoneNumber,
                    Status = u.User.AccountStatus.ToString()
                }).ToListAsync();

            return (centers, totalCount);
        }

        public async Task<(IEnumerable<TeacherDetailResponse> Teachers, int TotalCount)> GetTeachersByCenter(Guid centerId, int pageNumber, int pageSize, string? fullName = null, AccountStatus? status = null)
        {
            var checkCenter = await _unitOfWork.GetRepository<CenterProfile>().Entities
                .Include(u => u.User)
                .FirstOrDefaultAsync(c => c.Id == centerId && !c.User.IsDeleted);
            if(checkCenter == null)
            {
                throw new Exception("Center not found.");
            }

            IQueryable<TeacherProfile> query = _unitOfWork.GetRepository<TeacherProfile>().Entities
                .Include(u => u.User)
                .Where(c => c.CenterProfileId == centerId && !c.User.IsDeleted);

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                query = query.Where(u => EF.Functions.Like(u.User.Fullname, $"%{fullName}%"));
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.User.AccountStatus == status);
            }

            int totalCount = await query.CountAsync();

            var teachers = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TeacherDetailResponse
                {
                    Id = t.Id,
                    UId = t.User.Id,
                    Fullname = t.User.Fullname,
                    BirthYear = t.User.DateOfBirth.Year.ToString(),
                    Gender = t.User.Gender.ToString(),
                    YearOfExperience = t.YearOfExperience,
                    Qualification = t.Qualification.ToString(),
                    Status = t.User.AccountStatus.ToString()
                }).ToListAsync();

            return (teachers, totalCount);
        }

        public async Task<CenterUserResponse> GetCenterByUserId(Guid UserId)
        {
            var center =  _unitOfWork.GetRepository<CenterProfile>().Entities
                .Include(u => u.User)
                .Where(c => c.UserId == UserId && !c.User.IsDeleted)
                .Select(u => new CenterUserResponse
                {
                    Id = u.Id,
                    UId = u.User.Id,
                    Email = u.User.Email,
                    Fullname = u.User.Fullname,
                    PhoneNumber = u.User.PhoneNumber,
                    Gender = u.User.Gender.ToString(),
                    UserRole = u.User.UserRole.ToString(),
                    CenterName = u.CenterName,
                    Address = u.Address,
                    EstablishDate = u.EstablishDate,
                    ContactEmail = u.ContactEmail,
                    ContactPhoneNumber = u.ContactPhoneNumber,
                    Status = u.User.AccountStatus.ToString()
                });

            return await center.FirstOrDefaultAsync();
        }

        public async Task<TeacherDetailResponse> GetTeacherByUserId(Guid UserId)
        {
            var teacher = _unitOfWork.GetRepository<TeacherProfile>().Entities
                .Include(u => u.User)
                .Where(t => t.UserId == UserId && !t.User.IsDeleted)
                .Select(u => new TeacherDetailResponse
                {
                    Id = u.Id,
                    UId = u.User.Id,
                    Fullname = u.User.Fullname,
                    BirthYear = u.User.DateOfBirth.Year.ToString(),
                    Gender = u.User.Gender.ToString(),
                    YearOfExperience = u.YearOfExperience,
                    Qualification = u.Qualification.ToString(),
                    Status = u.User.AccountStatus.ToString()
                });

            return await teacher.FirstOrDefaultAsync();
        }

        public async Task<CenterUserResponse> UpdateCenterInformation(Guid userId, CenterUpdateRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var center = await _unitOfWork.GetRepository<CenterProfile>().Entities
                .Include(u => u.User)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.User.IsDeleted);

            if (center == null) throw new KeyNotFoundException("Center not found.");

            // Email
            bool emailExists = await _unitOfWork.GetRepository<User>().Entities
                .AnyAsync(u =>
                    !u.IsDeleted &&
                    u.Id != userId &&
                    u.Email == request.Email);

            bool emailUsedAsContact = await _unitOfWork.GetRepository<CenterProfile>().Entities
                .AnyAsync(c =>
                    !c.IsDeleted &&
                    c.UserId != userId &&
                    c.ContactEmail == request.Email);

            if (emailExists || emailUsedAsContact)
                throw new InvalidOperationException("This email is already being used by another user.");


            // Contact Email
            bool contactEmailExists = await _unitOfWork.GetRepository<User>().Entities
                .AnyAsync(u =>
                    !u.IsDeleted &&
                    u.Id != userId &&
                    u.Email == request.ContactEmail);

            bool contactEmailUsed = await _unitOfWork.GetRepository<CenterProfile>().Entities
                .AnyAsync(c =>
                    !c.IsDeleted &&
                    c.UserId != userId &&
                    c.ContactEmail == request.ContactEmail);

            if (contactEmailExists || contactEmailUsed)
                throw new InvalidOperationException("This contact email is already being used by another user.");


            // Phone Number
            bool phoneExists = await _unitOfWork.GetRepository<User>().Entities
                .AnyAsync(u =>
                    !u.IsDeleted &&
                    u.Id != userId &&
                    u.PhoneNumber == request.PhoneNumber);

            bool phoneUsedAsContact = await _unitOfWork.GetRepository<CenterProfile>().Entities
                .AnyAsync(c =>
                    !c.IsDeleted &&
                    c.UserId != userId &&
                    c.ContactPhoneNumber == request.PhoneNumber);

            if (phoneExists || phoneUsedAsContact)
                throw new InvalidOperationException("This phone number is already being used by another user.");


            // Contact Phone Number
            bool contactPhoneExists = await _unitOfWork.GetRepository<User>().Entities
                .AnyAsync(u =>
                    !u.IsDeleted &&
                    u.Id != userId &&
                    u.PhoneNumber == request.ContactPhoneNumber);

            bool contactPhoneUsed = await _unitOfWork.GetRepository<CenterProfile>().Entities
                .AnyAsync(c =>
                    !c.IsDeleted &&
                    c.UserId != userId &&
                    c.ContactPhoneNumber == request.ContactPhoneNumber);

            if (contactPhoneExists || contactPhoneUsed)
                throw new InvalidOperationException("This contact phone number is already being used by another user.");

            center.User.Email = request.Email.Trim().ToLowerInvariant();
            center.User.Fullname = request.Fullname;
            center.User.PhoneNumber = request.PhoneNumber;
            center.User.Gender = request.Gender;
            center.CenterName = request.CenterName;
            center.Address = request.Address;
            center.ContactEmail = request.ContactEmail.Trim().ToLowerInvariant();
            center.ContactPhoneNumber = request.ContactPhoneNumber;

            await _unitOfWork.GetRepository<CenterProfile>().UpdateAsync(center);
            await _unitOfWork.SaveAsync();

            return await GetCenterByUserId(userId);
        }
    }
}
