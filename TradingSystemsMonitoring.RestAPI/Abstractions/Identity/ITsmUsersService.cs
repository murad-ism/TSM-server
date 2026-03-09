using System.Threading;
using System.Threading.Tasks;

namespace TradingSystemsMonitoring.RestAPI.Abstractions.Identity
{
    public interface ITsmUsersService
    {
        Task<TsmUserToken> Login(TsmUserLoginData request, CancellationToken cancellationToken);
        Task<OperationResult> AddUser(TsmUserRegisterData request, CancellationToken cancellationToken);
        Task<OperationResult> DeleteUser(string userName, CancellationToken cancellationToken);
        Task Logout();
    }
}
