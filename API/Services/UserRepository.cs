using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class UserRepository(DataContext dataContext, IMapper mapper) : IUserRepository
{
    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await dataContext.Users
            .Include(x => x.Photos)
            .ToListAsync();
    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await dataContext.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        return await dataContext.Users
            .Include(x => x.Photos)
            .SingleOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower());
    }

    public void Update(AppUser user)
    {
        dataContext.Entry(user).State = EntityState.Modified;
    }
    
    public async Task<bool> SaveAllAsync()
    {
        return await dataContext.SaveChangesAsync() > 0;
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(PaginationParams @params)
    {
        var query = dataContext.Users
            .ProjectTo<MemberDto>(mapper.ConfigurationProvider);

        var pagedUsers = await PagedList<MemberDto>
            .CreateAsync(query, @params.PageNumber, @params.PageSize);
        
        return pagedUsers;
    }

    public async Task<MemberDto?> GetMemberAsync(string username)
    {
        return await dataContext.Users
            .Where(x => x.UserName.ToLower() == username.ToLower())
            .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }
}