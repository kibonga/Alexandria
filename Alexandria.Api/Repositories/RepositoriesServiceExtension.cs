using Alexandria.Api.Repositories.Authors;
using Alexandria.Api.Repositories.Books;

namespace Alexandria.Api.Repositories
{
    public static class RepositoriesServiceExtension
    {
        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
        }
    }
}
