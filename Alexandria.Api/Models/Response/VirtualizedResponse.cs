namespace Alexandria.Api.Models.Response
{
    public class VirtualizedResponse<T>
    {
        public List<T> Items { get; set; }
        public int Total { get; set; }
    }
}
