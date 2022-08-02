using Alexandria.Blazor.Server.Ui.Services.Base;
using AutoMapper;
using Blazored.LocalStorage;

namespace Alexandria.Blazor.Server.Ui.Services.Book
{
    public class BookService : BaseHttpClient, IBookService
    {
        private readonly IClient _client;
        private readonly IMapper _mapper;

        public BookService(IClient client, ILocalStorageService localStorage, IMapper mapper)
            : base(client, localStorage)
        {
             _client = client;
             _mapper = mapper;
        }

        #region Create Book
        public async Task<Response<int>> Create(BookCreateDto book)
        {
            Response<int> response = new();
            try
            {
                #region Add Bearer Token to Request
                await AddBearerToken();
                #endregion

                #region Create new Book via Api Endpoint using HttpClient
                await _client.BooksPOSTAsync(book);
                #endregion
            }
            catch(ApiException ex)
            {
                response = GlobalApiExceptions<int>(ex);
            }
            return response;
        }
        #endregion

        #region Delete Book
        public async Task<Response<int>> Delete(int id)
        {
            Response<int> response = new();

            try
            {
                #region Add Bearer Token to Request
                await AddBearerToken();
                #endregion

                #region Delete Book via Api Enpoint using HttpClient
                await _client.BooksDELETEAsync(id);
                #endregion
            }
            catch(ApiException ex)
            {
                response = GlobalApiExceptions<int>(ex);
            }

            return response;
        }
        #endregion

        #region Update Book
        public async Task<Response<int>> Edit(int id, BookUpdateDto book)
        {
            Response<int> response = new();

            try
            {
                #region Add Bearer Token to Request
                await AddBearerToken();
                #endregion

                #region Update Book via Api Endpoint using HttpClient
                await _client.BooksPUTAsync(id, book);
                #endregion
            }
            catch(ApiException ex)
            {
                response = GlobalApiExceptions<int>(ex);
            }

            return response;
        }
        #endregion

        #region Get All Books
        public async Task<Response<List<BookReadOnlyDto>>> Get()
        {
            Response<List<BookReadOnlyDto>> response = new();

            try
            {
                #region Add Bearer Token to Request
                await AddBearerToken();
                #endregion

                #region Get all Book via Api Endpoint using HttpClient
                var data = await _client.BooksAllAsync();
                #endregion

                #region Create Response Object
                response = new Response<List<BookReadOnlyDto>>
                {
                    Data = data.ToList(),
                    Success = true
                };
                #endregion
            }
            catch(ApiException ex)
            {
                response = GlobalApiExceptions<List<BookReadOnlyDto>>(ex);
            }

            return response;
        }
        #endregion

        #region Get single Book
        public async Task<Response<BookDetailsDto>> Get(int id)
        {
            Response<BookDetailsDto> response = new();

            try
            {
                #region Add Bearer Token to Request
                await AddBearerToken();
                #endregion

                #region Get single Book via Api Endpoint using HttpClient
                var data = await _client.BooksGETAsync(id);
                #endregion

                #region Create Response Object
                response = new Response<BookDetailsDto>
                {
                    Data = data,
                    Success =true
                };
                #endregion
            }
            catch(ApiException ex)
            {
                response = GlobalApiExceptions<BookDetailsDto>(ex);
            }

            return response;
        }
        #endregion

        #region Get single Book for Update
        public async Task<Response<BookUpdateDto>> GetForUpdate(int id)
        {
            Response<BookUpdateDto> response = new();
            
            try
            {
                #region Add Bearer Token to Request
                await AddBearerToken();
                #endregion

                #region Get single Book for Update via Api Endpoint using HttpClient
                var data = _client.BooksGETAsync(id);
                #endregion

                #region Create Response Object
                response = new Response<BookUpdateDto>
                {
                    Data = _mapper.Map<BookUpdateDto>(data),
                    Success = true
                };
                #endregion
            }
            catch(ApiException ex)
            {
                response = GlobalApiExceptions<BookUpdateDto>(ex);
            }

            return response;
        }
        #endregion
    }
}
