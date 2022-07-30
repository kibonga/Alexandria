using Alexandria.Api.Data;
using Alexandria.Api.Models.Author;
using Alexandria.Api.Models.Book;
using AutoMapper;

namespace Alexandria.Api.Configurations
{
    public class MapperConfig: Profile
    {
        public MapperConfig()
        {
            #region Author
            CreateMap<AuthorCreateDto, Author>().ReverseMap(); 
            CreateMap<AuthorUpdateDto, Author>().ReverseMap(); 
            CreateMap<AuthorReadOnlyDto, Author>().ReverseMap();
            #endregion

            #region Book
            // Comment: Advanced mapping
            CreateMap<Book, BookReadOnlyDto>()
                .ForMember(bookDto => bookDto.AuthorName, book => book.MapFrom(b => $"{b.Author.FirstName} {b.Author.LastName}"))
                .ReverseMap();
            CreateMap<Book, BookDetailsDto>()
                .ForMember(bookDto => bookDto.AuthorName, book => book.MapFrom(b => $"{b.Author.FirstName} {b.Author.LastName}"))
                .ReverseMap();
            CreateMap<BookUpdateDto, Book>().ReverseMap();
            CreateMap<BookCreateDto, Book>().ReverseMap();
            #endregion
        }
    }
}
