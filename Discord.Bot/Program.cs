 using System;
 using System.Linq;
 using System.Reflection;
 using System.Threading.Tasks;
 using Discord.Commands;
 using Discord.WebSocket;
 using Microsoft.Extensions.DependencyInjection;
 using Victoria;

 namespace Discord.Bot
{
    class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _service;
        private string token = Secret.getToken;
        private Random _rnd;
        static void Main(string[] args)
        {
            new Program().RunBotAsync().GetAwaiter().GetResult();
            Console.WriteLine("Starting Bot!");
            
        }
        public async Task RunBotAsync()
        {
            _rnd = new Random();
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _service = ConfigureServices();
            _client.Ready += Ready;
            _client.Log += ClientLog;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Ready()
        {
            var node = _service.GetService(typeof(LavaNode)) as LavaNode;
            await node.ConnectAsync();
            Console.WriteLine($"Music Connection is Ready with state {node.IsConnected}");
            await _client.SetGameAsync("Geht auf brainyxs.com");
            Console.WriteLine("Logged in as " + _client.CurrentUser.Username);
            var message = await (_client.GetChannel(708713001141928079) as IMessageChannel).SendMessageAsync("Hey :D Ich wurde soeben hochgefahren :D <:BrainyXS:709125859788980235>");
            Discord.UserExtensions.SendMessageAsync(_client.Guilds.First().GetUser(382248892101558274), "Hallo Brainy, ich bin Online");
            await Task.Delay(15000);
            
            await message.DeleteAsync();

        }


        private Task ClientLog(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _service);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            if (message != null && message.Author.IsBot)
            {
                return;
            }

            int argPos = 0;
            if (message.HasStringPrefix("", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _service);
                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(_client).AddSingleton(_commands)
                .AddSingleton<LavaNode>()
                .AddSingleton<LavaConfig>().BuildServiceProvider();
        }
        
    }
}