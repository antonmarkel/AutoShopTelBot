using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Commands
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CommandAttribute: Attribute
    {
        public string Name {  get; set; }
        public CommandAttribute(string commandName) { this.Name = commandName; }
    }
    public class UserCommand
    {
      
        [CommandAttribute("load_picture")]
        public async void loadPict(ChatId chat,ITelegramBotClient _botClient, TelegramBot _bot,Message message)
        {
            if(message.Photo != null)
            {

                Console.WriteLine(message.Photo[0].FileId);
                
            }
        }

        [CommandAttribute("show_pictures")]
        public async void showPicts(ChatId chat, ITelegramBotClient _botClient, TelegramBot _bot, Message message)
        {
           foreach(var item in _bot.Pictures)
            {
                await _botClient.SendPhotoAsync(chat,item);
            }
        }
    }
}
