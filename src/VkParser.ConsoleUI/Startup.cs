namespace VkParser.ConsoleUI;

internal class Startup
{
    private readonly IServiceProvider _services;
    private readonly IVkFacade _vk;

    public Startup(IServiceProvider services, IVkFacade vk)
    {
        _services = services;
        _vk = vk;
    }

    public static void Main() =>
        CreateHostBuilder().Build().Services.GetRequiredService<Startup>().Run();

    public async void Run()
    {
        var loginPass = File.ReadAllLines(@"F:\SHILY PROJECT\Projects\vk\sec.txt", Encoding.UTF8);
        var acc = _services.GetRequiredService<IAccount>();

        acc.Login = loginPass[0];
        acc.Password = loginPass[1];

        await _vk.SignInAsync();
    }

    private static IHostBuilder CreateHostBuilder() => Host
        .CreateDefaultBuilder(Array.Empty<string>())
        .ConfigureServices(services => services.AddConsoleUIComponents());
}