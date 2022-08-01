namespace Alexandria.Blazor.Server.Ui.Services.Base
{
    // Allows us to write additional logic but in a separate file/class/interface
    // All Partial classes will be compiled in one common class
    public partial interface IClient
    {
        public HttpClient HttpClient { get; }
    }
}
