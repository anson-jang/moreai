using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moreai.web.Models;

namespace moreai.web.Services
{
    public interface ITokenService
    {
        string GenerateToken(Member member);
        bool ValidateToken(string token);
        int? GetUserIdFromToken(string token);
    }
}