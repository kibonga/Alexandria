using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace Alexandria.Blazor.Server.Ui.Services.Base
{
    public class BaseHttpClient
    {
        private readonly IClient _client;
        private readonly ILocalStorageService _localStorage;

        public BaseHttpClient(IClient client, ILocalStorageService localStorage)
        {
            _client = client;
            _localStorage = localStorage;
        }

        #region Convert Api Exceptions - Error handling logic defined in one place
        protected Response<Guid> GlobalApiExceptions<Guid>(ApiException apiException)
        {
            #region Error - 400
            if(apiException.StatusCode == 400)
            {
                return new Response<Guid>() { Message = "There are Validation errors", ValidationErrors = apiException.Response, Success = false };
            }
            #endregion

            #region Error - Not Found
            if (apiException.StatusCode == 404)
            {
                return new Response<Guid>() { Message = "The requested resource could not be found.", Success = false };
            }
            #endregion

            #region Success
            if (apiException.StatusCode >= 200 && apiException.StatusCode <= 299)
            {
                return new Response<Guid>() { Message = "The requested was a success.", Success = true };
            }
            #endregion

            return new Response<Guid>() { Message = "Something went wrong, please try again.", Success = false };
        }
        #endregion

        #region Add HTTP Bearer Header - Get Bearer Token from LocalStorage and Set it - needed for every request
        protected async Task AddBearerToken()
        {
            #region Get Token from LocalStorage
            var token = await _localStorage.GetItemAsync<string>("accessToken");
            #endregion

            #region Set Token
            if(token != null)
            {
                _client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            #endregion
        }
        #endregion
    }
}
