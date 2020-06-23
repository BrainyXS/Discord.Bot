using System.Linq;
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

        
    }
}