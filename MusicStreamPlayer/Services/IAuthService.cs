using MusicStreamPlayer.Models;
using Microsoft.AspNetCore.Identity;

namespace MusicStreamPlayer.Services
{
    public interface IAuthService
    {
        Task<(IdentityResult result, ApplicationUser user)> RegisterUserAsync(RegisterViewModel model);
        Task<SignInResult> LoginUserAsync(LoginViewModel model);
        Task LogoutAsync();
    }
}