using Alexandria.Blazor.Server.Ui.Providers;
using Alexandria.Blazor.Server.Ui.Services.Base;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Alexandria.Blazor.Server.Ui.Services.Auth
{
    public class AuthService: IAuthService
    {
        private readonly IClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthService(IClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        #region Auth Async
        public async Task<bool> AuthAsync(LoginUserDto loginModel)
        {
            #region Login Async
            var response = await _httpClient.LoginAsync(loginModel);
            #endregion

            #region Store Token (from response object)
            await _localStorage.SetItemAsync("accessToken", response.Token);
            #endregion

            #region Change Auth State of Application (needs cast)
            await ((ApiAuthStatePovider)_authenticationStateProvider).LoggedIn();
            #endregion

            return true;
        }
        #endregion

        #region Logout
        public async Task Logout()
        {
            #region Change Auth State of Application (needs cast)
            await ((ApiAuthStatePovider)_authenticationStateProvider).LoggedOut();
            #endregion
        }
        #endregion
    }
}
