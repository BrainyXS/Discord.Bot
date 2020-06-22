using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Victoria;

namespace Discord.Bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _player;

        public Commands(LavaNode node)
        {
            _player = node;
        }

        [Command("test")]
        public async Task Test()
        {
            if (Context.User.Username == "BrainyXS")
            {
                await ReplyAsync("test erfolgreich, Ich bin Online :D");
                await Context.Message.DeleteAsync();
            }
        }

        [Command("soos")]
        public async Task soos()
        {
            await ReplyAsync(
                "Bitte verzieh dich mit saas, soos, sonst irgendeiner Youtube-Kacke Sprache, ich pack das null! ~Huebi, 2020");
        }

        [Command("ban")]
        public async Task ban(string arg)
        {
            var user = Context.User as SocketGuildUser;
            var role = (Context.User as IGuildUser).Guild.Roles.Single(
                role => role.Name == "Bewohner des Kaiser-Hauses");
            var role2 = (Context.User as IGuildUser).Guild.Roles.Single(
                role => role.Name == "Kaiser");
            if (!user.Roles.Contains(role) && !user.Roles.Contains(role2))
            {
                await Context.Message.DeleteAsync();
                await ReplyAsync(
                    "ALDA, CHILL DEINE BASE \n DENKST DU, DU BIST BRAINY ODER WIE?\n DAS KANNSZ DU JETZT ECHT NICHT BRINGEN!!");
            }
        }

        [Command("join", RunMode = RunMode.Async)]
        public async Task join(IVoiceChannel channel = null)
        {
            await Context.Message.DeleteAsync();
            channel ??= (Context.User as IGuildUser)?.VoiceChannel;
            if (channel != null)
            {
                var audioClient = await channel.ConnectAsync();
                ;

                var path = "E:\\Manuel\\Videos\\musics\\Animal Crossing - Bubblegum K.K. [Remix].mp3";
                CreateStream(path);
                using (var ffmpeg = CreateStream(path))
                    using (var output = ffmpeg.StandardOutput.BaseStream)
                        using (var discord = audioClient.CreatePCMStream(AudioApplication.Mixed))
                        {
                            try
                            {
                                await output.CopyToAsync(discord);
                            }
                            finally
                            {
                                await discord.FlushAsync();
                                await audioClient.StopAsync();
                            }
                        }
            }
        }


        [Command("leave")]
        [Alias("stop")]
        public async Task stop()
        {
            await Context.Message.DeleteAsync();
            var channel = (Context.User as IGuildUser)?.VoiceChannel;
            await channel.DisconnectAsync();
        }


        private static Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }
    }
}