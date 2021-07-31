using System;

namespace Middleware
{
    public interface IJwtBuilder
    {
        string GetToken(string userId);
        Guid ValidateToken(string token);
    }
}
