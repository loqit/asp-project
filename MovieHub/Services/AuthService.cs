using System;
using MovieHub.Areas.Identity;
using MovieHub.Services.Abstractions;

namespace MovieHub.Services
{
    public class AuthService : IAuthService
    {
        public bool CreateUser(string email, string password, string roleName)
        {

            var user = new MHUser
            {
                Email = email.ToLower(),
            };



            return true;
        }
    }
}