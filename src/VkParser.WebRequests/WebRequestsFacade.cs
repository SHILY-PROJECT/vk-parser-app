namespace VkParser.WebRequests;

internal class WebRequestsFacade : IVkFacade
{
    private readonly IParser _parser;
    private readonly IAuthorizer _authorizer;

    public WebRequestsFacade(IParser parser, IAuthorizer authorizer)
    {
        _parser = parser;
        _authorizer = authorizer;
    }

    public async Task<bool> SignInAsync()
    {
        /*
         *  Add load and save profile with cookies.
         */

        var authFlag = await _authorizer.SignInAsync();

        if (authFlag)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Successful authorization.");
            Console.ResetColor();
            return true;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Failed.");
            Console.ResetColor();
            return false;
        }
    }

    public Task<IEnumerable<IUser>> ParseUsersAsync(IFilterOptions filter)
    {
        return _parser.ParseUsersAsync(filter);
    }
}