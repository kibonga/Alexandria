using Alexandria.Blazor.Server.Ui.Services.Base;
using AutoMapper;

namespace Alexandria.Blazor.Server.Ui.Configurations
{
    public class MapperConfig: Profile
    {
        public MapperConfig()
        {
            #region Author
            CreateMap<AuthorReadOnlyDto, AuthorUpdateDto>().ReverseMap();
            CreateMap<AuthorDetailsDto, AuthorUpdateDto>().ReverseMap();
            #endregion
        }
    }
}
