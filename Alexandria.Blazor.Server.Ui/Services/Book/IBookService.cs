using Alexandria.Blazor.Server.Ui.Services.Base;

namespace Alexandria.Blazor.Server.Ui.Services.Book
{
    public interface IBookService
    {
        Task<Response<List<BookReadOnlyDto>>> Get();
        Task<Response<BookDetailsDto>> Get(int id);
        Task<Response<BookUpdateDto>> GetForUpdate(int id);
        Task<Response<int>> Create(BookCreateDto book);
        Task<Response<int>> Edit(int id, BookUpdateDto book);
        Task<Response<int>> Delete(int id);
    }
}
