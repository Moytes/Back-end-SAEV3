using Data;
using Microsoft.EntityFrameworkCore;
using Models.DB;
using Models.Request;
using Repositories.IRepositories;
using Utilities.Abstractions;
using Utilities.Errors;

namespace Repositories;

public class UserRepositorie : IUserRepositorie
{
    private readonly AppDbContext _context;

    public UserRepositorie(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<Guid>> CreateUser(AddUserRequest request, string passwordSalt, string passwordHash)
    {
        // Validate unique email
        var existingUser = await _context.User
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
            return Result<Guid>.Failure(UserErrors.EmailAlreadyExists);

        // Validate SchoolZone exists if provided
        if (request.SchoolZoneId.HasValue)
        {
            var schoolZoneExists = await _context.SchoolZone
                .AnyAsync(sz => sz.Id == request.SchoolZoneId.Value);

            if (!schoolZoneExists)
                return Result<Guid>.Failure(SchoolErrors.SchoolZoneNotFound);
        }

        // Create user entity
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.Name,
            FatherLastName = request.FatherLastName,
            MotherLastName = request.MotherLastName,
            Role = request.Role,
            SchoolZoneId = request.SchoolZoneId,
            PhoneNumber = request.PhoneNumber,
            AvatarUrl = request.AvatarUrl,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Status = boolStatus.True,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add to database
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(user.Id);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.User
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserById(Guid userId)
    {
        return await _context.User
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
