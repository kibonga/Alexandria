using Alexandria.Blazor.Server.Ui.Services.Auth;
using Alexandria.Blazor.Server.Ui.Services.Author;
using Alexandria.Blazor.Server.Ui.Services.Book;

namespace Alexandria.Blazor.Server.Ui.Services
{
    public static class ServicesExtension
    {
        public static void RegisterServicesExtension(this IServiceCollection services)
        {
            #region Register Auth
            services.AddScoped<IAuthService, AuthService>();
            #endregion

            #region Register Author services
            services.AddScoped<IAuthorService, AuthorService>();
            #endregion

            #region Register Book services
            services.AddScoped<IBookService, BookService>();
            #endregion
        }
    }
}
