using Alexandria.Api.Models.Book;

namespace Alexandria.Api.Models.Author
{
    public class AuthorDetailsDto: AuthorReadOnlyDto
    {
        public List<BookReadOnlyDto> Books { get; set; }
    }
}
