using System.Security.Claims;

namespace SignalRServer
{
    public interface IJwtHandler
    {
        string GenerateToken(string userId);

    }
}