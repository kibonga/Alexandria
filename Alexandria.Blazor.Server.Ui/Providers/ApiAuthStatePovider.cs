using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Alexandria.Blazor.Server.Ui.Providers
{
    public class ApiAuthStatePovider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public ApiAuthStatePovider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        #region Get Auth State
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            #region User NOT logged in
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            #endregion

            #region Get Token
            var savedToken = await _localStorage.GetItemAsync<string>("accessToken");
            #endregion

            #region Token is null - no claims (user not logged in)
            if (savedToken == null)
            {
                #region No Token so no Claims
                return new AuthenticationState(user);
                #endregion
            }
            #endregion

            #region Get Token Content
            var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);
            #endregion

            #region Check Token Expiration
            if (tokenContent.ValidTo < DateTime.UtcNow)
            {
                #region Token is present but is invalid due to expiration
                return new AuthenticationState(user);
                #endregion
            }
            #endregion

            var claims = await GetClaims();

            #region Update Claims - User is present and valid
            user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            #endregion

            return new AuthenticationState(user);
        }
        #endregion

        #region Auth State Login
        public async Task LoggedIn()
        {
            var claims = await GetClaims();

            #region Create User with all of his Claims
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            #endregion

            #region Create New Auth State with User
            var authState = Task.FromResult(new AuthenticationState(user));
            #endregion

            #region Notify entire app about Change in Auth State
            NotifyAuthenticationStateChanged(authState); 
            #endregion
        }
        #endregion

        #region Auth State Logout
        public async Task LoggedOut()
        {
            #region Clear LocalStorage
            await _localStorage.RemoveItemAsync("accessToken");
            #endregion

            #region Create Non existing user with no claims
            var nobody = new ClaimsPrincipal(new ClaimsIdentity());
            #endregion

            #region Create empty auth state
            var authState = Task.FromResult(new AuthenticationState(nobody));
            #endregion

            #region Update and notify entire app about new Empty state
            NotifyAuthenticationStateChanged(authState); 
            #endregion
        }
        #endregion

        #region Get Claims
        private async Task<List<Claim>> GetClaims()
        {
            #region Get Token
            var savedToken = await _localStorage.GetItemAsync<string>("accessToken");
            #endregion

            #region Get Content from Token
            var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);
            #endregion

            #region Get list of Claims from Token
            var claims = tokenContent.Claims.ToList();
            #endregion

            #region Additional Claims (manually)
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            #endregion

            return claims;
        }
        #endregion
    }
}
