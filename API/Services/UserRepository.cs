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

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams @params)
    {
        var query = dataContext.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(@params.CurrentUsername))
        {
            query = query.Where(x => x.UserName != @params.CurrentUsername);
        }

        if (!string.IsNullOrWhiteSpace(@params.Gender))
        {
            query = query.Where(x => x.Gender == @params.Gender);
        }

        if (@params.MinAge != null)
        {
            query = query.Where(x => x.DateOfBirth <= DateOnly.FromDateTime(DateTime.Today.AddYears(-@params.MinAge.Value)));
        }

        if (@params.MaxAge != null)
        {
            query = query.Where(x => x.DateOfBirth > DateOnly.FromDateTime(DateTime.Today.AddYears(-@params.MaxAge.Value - 1)));
        }

        var pagedUsers = await PagedList<MemberDto>.CreateAsync(
            query.ProjectTo<MemberDto>(mapper.ConfigurationProvider),
            @params.PageNumber,
            @params.PageSize);
        
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