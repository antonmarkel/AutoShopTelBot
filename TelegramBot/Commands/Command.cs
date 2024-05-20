using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Commands
{
    public class Command
    {
        public static void ExecuteCommand(string command, object handler, ChatId chat, ITelegramBotClient _botClient, TelegramBot _bot, Message message)
        {
            Type type = handler.GetType();
            var methods = type.GetMethods();
            var method = methods.FirstOrDefault(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Cast<CommandAttribute>().Any(attr => attr.Name.Equals(command, StringComparison.OrdinalIgnoreCase))) ;

            if (method != null)
            {
                method.Invoke(handler, parameters:new object[] { chat, _botClient, _bot, message });
            }
            else
            {
                _botClient.SendTextMessageAsync(chat, "Command wasn't found");
            }
        }
    }
}
