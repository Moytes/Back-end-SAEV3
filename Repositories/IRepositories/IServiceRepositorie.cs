using Models.DB;

namespace Repositories.IRepositories;

public interface IServiceRepositorie
{
    Task AddLog(AuditLog log);
}
