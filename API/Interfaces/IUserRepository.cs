using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);
    void Update(AppUser user);
    Task<PagedList<MemberDto>> GetMembersAsync(UserParams @params);
    Task<MemberDto?> GetMemberAsync(string username, bool isCurrentUser);
    Task<AppUser?> GetUserByPhotoIdAsync(int photoId);
}