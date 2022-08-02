


using Alexandria.Blazor.Server.Ui.Services.Base;
using AutoMapper;
using Blazored.LocalStorage;

namespace Alexandria.Blazor.Server.Ui.Services.Author
{
    public class AuthorService : BaseHttpClient, IAuthorService
    {
        private readonly IClient _client;
        private readonly IMapper _mapper;

        public AuthorService(IClient client, ILocalStorageService localStorage, IMapper mapper) 
            : base(client, localStorage)
        {
            _client = client;
            _mapper = mapper;
        }

        #region Create Author
        public async Task<Response<int>> Create(AuthorCreateDto author)
        {
            var response = new Response<int>();

            try
            {
                #region Add Bearer Token to Request
                await AddBearerToken();
                #endregion

                #region Create new Author via Api Endpoint using HttpClient
                await _client.AuthorsPOSTAsync(author);
                #endregion
            }
            catch(ApiException ex)
            {
                response = GlobalApiExceptions<int>(ex);
            }

            return response;
        }
        #endregion

        #region Get All Authors
        public async Task<Response<List<AuthorReadOnlyDto>>> Get()
        {
            Response<List<AuthorReadOnlyDto>> response;

            try
            {
                #region Add Bearer Token to Request
                await AddBearerToken();
                #endregion

                #region Get All Authors via Api Endpoint using HttpClient
                var data = await _client.AuthorsAllAsync();
                #endregion

                #region Create Response Object
                response = new Response<List<AuthorReadOnlyDto>>
                {
                    Data = data.ToList(),
                    Success = true
                }; 
                #endregion

            }
            catch(ApiException ex)
            {
                response = GlobalApiExceptions<List<AuthorReadOnlyDto>>(ex);
            }

            return response;
        }
        #endregion

        #region Update Author
        public async Task<Response<int>> Edit(int id, AuthorUpdateDto author)
        {
            Response<int> response = new();
            try
            {
                #region Add Bearer Token to Request
                await AddBearerToken();
                #endregion

                #region Update Author via Api Endpoint using HttpClient
                await _client.AuthorsPUTAsync(id, author);
                #endregion

            }
            catch (ApiException ex)
            {
                response = GlobalApiExceptions<int>(ex);
            }

            return response;
        }
        #endregion

        #region Get single Author for Update
        public async Task<Response<AuthorUpdateDto>> GetForUpdate(int id)
        {
            Response<AuthorUpdateDto> response = new();

            try
            {
                #region Add Bearer Token to Request
                await AddBearerToken();
                #endregion

                #region Get single Author for UPDATE via Api Endpoint using HttpClient
                var data = await _client.AuthorsGETAsync(id);
                #endregion

                #region Map Author to AuthorUpdateDto using Automapper
                response = new Response<AuthorUpdateDto>
                {
                    Data = _mapper.Map<AuthorUpdateDto>(data),
                    Success = true
                };
                #endregion
            }
            catch (ApiException ex)
            {
                response = GlobalApiExceptions<AuthorUpdateDto>(ex);
            }

            return response;
        }
        #endregion

        #region Get single Author
        public async Task<Response<AuthorDetailsDto>> Get(int id)
        {
            Response<AuthorDetailsDto> response = new();

            try
            {
                #region Add Bearer Token to Request
                await AddBearerToken();
                #endregion

                #region Get single Author via Api Endpoint using HttpClient
                var data = await _client.AuthorsGETAsync(id);
                #endregion

                #region Map Authors to AuthorDetailsDto using Automapper
                response = new Response<AuthorDetailsDto>
                {
                    Data = data,
                    Success = true
                };
                #endregion
            }
            catch (ApiException ex)
            {
                response = GlobalApiExceptions<AuthorDetailsDto>(ex);
            }

            return response;
        }
        #endregion

        #region Delete Author
        public async Task<Response<int>> Delete(int id)
        {
            Response<int> response = new();

            try
            {
                #region Add Bearer Token to Request
                await AddBearerToken();
                #endregion

                #region Delete Author via Api Endpoint using HttpClient
                await _client.AuthorsDELETEAsync(id);
                #endregion
            }
            catch (ApiException ex)
            {
                response = GlobalApiExceptions<int>(ex);
            }

            return response;
        }
        #endregion
    }
}
