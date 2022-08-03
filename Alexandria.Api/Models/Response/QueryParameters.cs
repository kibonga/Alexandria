namespace Alexandria.Api.Models.Response
{
    public class QueryParameters
    {
        private static int PER_PAGE = 15;
        private static int DEFAULT_INDEX = 1;

        public int PerPage { get; set; } = PER_PAGE;
        public int StartIndex { get; } = DEFAULT_INDEX;
    }
}
