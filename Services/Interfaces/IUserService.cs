using Core.Base;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<(IEnumerable<UserDetailResponse> Users, int TotalCount)> GetAllUsers(int pageNumber, int pageSize, string? fullName, string? role, AccountStatus? status);
        Task<(IEnumerable<CenterUserResponse> Centers, int TotalCount)> GetAllCenters(int pageNumber, int pageSize, string? centerName, string? address, AccountStatus? status);
        Task<(IEnumerable<TeacherDetailResponse> Teachers, int TotalCount)> GetTeachersByCenter(Guid centerId, int pageNumber, int pageSize, string? fullName, AccountStatus? status);
        Task<CenterUserResponse> GetCenterByUserId(Guid UserId);
        Task<TeacherDetailResponse> GetTeacherByUserId(Guid UserId);
        Task<CreateCenterUser> AddNewCenterAccount(CreateCenterUser request);
        Task<CreateTeacherRequest> AddNewTeacherAccount(CreateTeacherRequest request, Guid userId);
        Task<string> UpdateAccountStatus(Guid userId, int status);
        Task<CenterUserResponse> UpdateCenterInformation(Guid userId, CenterUpdateRequest request);
    }
}
