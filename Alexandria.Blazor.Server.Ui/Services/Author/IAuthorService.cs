using Alexandria.Blazor.Server.Ui.Services.Base;

namespace Alexandria.Blazor.Server.Ui.Services.Author
{
    public interface IAuthorService
    {
        public Task<Response<List<AuthorReadOnlyDto>>> Get();
        public Task<Response<AuthorDetailsDto>> Get(int id);
        public Task<Response<AuthorUpdateDto>> GetForUpdate(int id);
        public Task<Response<int>> Create(AuthorCreateDto author);
        public Task<Response<int>> Edit(int id, AuthorUpdateDto author);
        public Task<Response<int>> Delete(int id);
    }
}
