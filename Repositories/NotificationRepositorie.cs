using Data;
using Microsoft.EntityFrameworkCore;
using Models.DB;
using Models.Dto;
using Models.Request;
using Repositories.IRepositories;
using Utilities.Abstractions;
using Utilities.Errors;

namespace Repositories;

public class NotificationRepositorie : INotificationRepositorie
{
    private readonly AppDbContext _context;

    public NotificationRepositorie(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NotificationItemDto>> GetNotificationsByUser(Guid userId)
    {
        return await _context.Notification
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new NotificationItemDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Type = x.Type,
                Title = x.Title,
                Message = x.Message,
                Read = x.Read,
                DestinationUrl = x.DestinationUrl,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<Result<Guid>> CreateNotification(AddNotificationRequest request)
    {
        var userExists = await _context.User.AnyAsync(x => x.Id == request.UserId);
        if (!userExists)
            return Result<Guid>.Failure(UserErrors.UserNotFound);

        var entity = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Type = request.Type,
            Title = request.Title,
            Message = request.Message,
            DestinationUrl = request.DestinationUrl,
            Read = boolStatus.False,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Notification.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<Guid>.Success(entity.Id);
    }

    public async Task<Result<bool>> MarkNotificationAsRead(Guid notificationId)
    {
        var entity = await _context.Notification.FirstOrDefaultAsync(x => x.Id == notificationId);

        if (entity == null)
            return Result<bool>.Failure(NotificationErrors.NotificationNotFound);

        entity.Read = boolStatus.True;

        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Notification?> GetNotificationById(Guid notificationId)
    {
        return await _context.Notification.FirstOrDefaultAsync(x => x.Id == notificationId);
    }
}