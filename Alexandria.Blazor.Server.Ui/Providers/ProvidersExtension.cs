using Microsoft.AspNetCore.Components.Authorization;

namespace Alexandria.Blazor.Server.Ui.Providers
{
    public static class ProvidersExtension
    {
        public static void RegisterProvidersExtension(this IServiceCollection services)
        {
            #region ApiAuthStateProvider
            services.AddScoped<ApiAuthStatePovider>();
            #endregion
            #region AuthStateProvider
            // Basically means, when someone asks for AuthenticationStateProvider we are going to provide our custom ApiAuthStateProvider (inherits)
            services.AddScoped<AuthenticationStateProvider>(p =>
                    p.GetRequiredService<ApiAuthStatePovider>()
                );
            #endregion
        }
    }
}
