using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord.Bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        
        [Command("test")]
        public async Task Test()
        {
            if (Context.User.Username == "BrainyXS")
            {
                await ReplyAsync("test erfolgreich, Ich bin Online :D");
                await Context.Message.DeleteAsync();
            }
        }

        [Command("tell")]
        public async Task Tell([Remainder] string msg)
        {
            if (Context.User.Username == "BrainyXS")
            {
                await Context.Message.DeleteAsync();
                await ReplyAsync(msg);
            }
        }

        [Command("soos")]
        public async Task soos()
        {
            await ReplyAsync(
                "Bitte verzieh dich mit saas, soos, sonst irgendeiner Youtube-Kacke Sprache, ich pack das null! ~Huebi, 2020");
        }

        [Command("/ban")]
        public async Task ban([Remainder] string arg)
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
                    "ALDA, CHILL DEINE BASE \n DENKST DU, DU BIST BRAINY ODER WIE?\n DAS KANNSt DU JETZT ECHT NICHT BRINGEN!!");
            }
        }

        [Command("dm")]
        public async Task dm(string id, [Remainder] string message)
        {
            if (Context.User.Username == "BrainyXS" || Context.User.Username == "Darthsven")
            {
                var user = Context.Message.MentionedUsers.First();
                var eb = new EmbedBuilder();
                eb.WithDescription($"Nachricht an {user.Username} gesendet (von {Context.User.Username})");
                await UserExtensions.SendMessageAsync(user, message);
                await Context.Message.DeleteAsync();
                var ms = await ReplyAsync("", false, eb.Build());

                var console = Context.Guild.GetTextChannel(424673909531738113);
                var cm = new EmbedBuilder();
                cm.WithTitle($"Nachricht von {Context.User.Username} an {user.Username}");
                cm.WithDescription(message);
                console.SendMessageAsync("", false, cm.Build());
                for (int i = 5; i > 0; i--)
                {
                    await Task.Delay(1500);
                    await ms.ModifyAsync(properties => { properties.Content = i.ToString(); });
                }

                await Task.Delay(1500);
                await ms.DeleteAsync();
            }
        }

        [Command("mob")]
        public async Task mob([Remainder] string name)
        {
            var toMob = name;
            if (Context.User.Username != "BrainyXS")
            {
                var sb = new StringBuilder();
                foreach (var c in name.ToCharArray())
                {
                    if (c != '*' && c != ' ' && c != '-')
                    {
                        sb.Append(c);
                    }
                }

                name = sb.ToString();
                if (name.ToLower().Contains("kaiser") || name == "Brainy" || name == "BrainyXS" || name.ToLower().Replace('l', 'i').Contains("brainy") ||
                    name.ToLower() == "manuel" || name.ToLower().Contains("brainy") || name.ToLower().Contains("manu")|| name.ToLower().Contains("gehirn")||
                    Context.Message.MentionedUsers.Contains(Context.Guild.GetUser(382248892101558274)))
                {
                    toMob = Context.User.Username;
                }
            }

            var e = new EmbedBuilder();
            e.WithDescription($"{toMob} wird von allen gemobbt.\nKeiner mag {toMob}\nGeh dich vergraben, {toMob}\n");
            await ReplyAsync("", false, e.Build());
        }

        [Command("meme")]
        public async Task Meme()
        {
            var client = new HttpClient();
            var st = await client.GetStringAsync("https://meme-api.herokuapp.com/gimme");
            st = st.Substring(st.IndexOf("url\":\"") + 6);
            var array = st.Split("\",\"nsfw\"");
            await ReplyAsync(array[0]);
        }

        [Command("search")]
        public async Task SearchAsync([Remainder] string query)
        {
            if (query.ToLower().Contains("brainy") || query.ToLower().Contains("manu") || query.ToLower().Contains("gehirn"))
            {
                await ReplyAsync("Dafür wurde der Bot nicht gemacht!");
                return;
            }
        try
            {
                query = query.Replace(' ', '+');
                var client = new HttpClient();
                var responseTask = client.GetAsync($"https://www.google.com/search?q={query}");
                var message = await ReplyAsync("Suche läuft...");
                var response = await responseTask;
                var responseString = await response.Content.ReadAsStringAsync();
                var array = responseString.Split("<div class=\"BNeawe iBp4i AP7Wnd\">");
                var resp1 = array[^1];
                array = resp1.Split("<");
                var ans = array[0];
                await message.DeleteAsync();
                await ReplyAsync(ans);
            }
            catch (Exception e)
            {
                ReplyAsync("Nicht gefunden");
            }

        }
        
        
        

        [Command("(╯°□°）╯︵ ┻━┻")]
        public async Task FlipBack()
        {
            await ReplyAsync("┬─┬ ノ( ゜-゜ノ)");
        }
    }
}