using Microsoft.AspNetCore.Http;
using System.Globalization;
using System;
using System.Linq;
using System.Security.Claims;

namespace WebReport.Services
{
    public interface IUserService
    {
        int GetCurrentUserId();
    }

    public class UserService : IUserService
    {
        readonly IHttpContextAccessor contextAccessor;

        public UserService(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor
                ?? throw new ArgumentNullException(nameof(contextAccessor));
        }

        public int GetCurrentUserId()
        {
            var sidStr = contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Sid);
            return Convert.ToInt32(sidStr.Value, CultureInfo.InvariantCulture);
        }
    }
}
