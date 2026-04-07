using Data;
using Models.DB;
using Repositories.IRepositories;

namespace Repositories;

public class ServiceRepositorie(AppDbContext context) : IServiceRepositorie
{
    private readonly AppDbContext _context = context;

    public async Task AddLog(AuditLog log)
    {
        await _context.AuditLog.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}
