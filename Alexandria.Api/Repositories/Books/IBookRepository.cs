using Alexandria.Api.Data;
using Alexandria.Api.Models.Book;

namespace Alexandria.Api.Repositories.Books
{
    public interface IBookRepository: IGenericRepository<Book>
    {
        Task<BookDetailsDto> GetBookAsync(int id);
        Task<List<BookReadOnlyDto>> GetAllBooksAsync();
    }
}
