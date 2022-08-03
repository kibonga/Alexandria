using Alexandria.Api.Data;
using Alexandria.Api.Models.Author;

namespace Alexandria.Api.Repositories.Authors
{
    public interface IAuthorRepository : IGenericRepository<Author>
    {
        Task<AuthorDetailsDto> GetAuthorDetailsAsync(int id);
    }
}
