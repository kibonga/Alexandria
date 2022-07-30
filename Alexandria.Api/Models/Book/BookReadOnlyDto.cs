namespace Alexandria.Api.Models.Book
{
    public class BookReadOnlyDto: BaseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int? AuthorId { get; set; }
        public string? AuthorName { get; set; }
    }
}
