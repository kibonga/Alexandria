
using Alexandria.Blazor.Server.Ui.Services.Base;

namespace Alexandria.Blazor.Server.Ui.Services.Auth
{
    public interface IAuthService
    {
        public Task<bool> AuthAsync(LoginUserDto loginModel);
        public Task Logout();
    }
}
