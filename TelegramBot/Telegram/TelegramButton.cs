using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Telegram
{
    public class TelegramButton
    {
        public readonly bool IsLink;
        public readonly TelegramRoute? ToRoute;
        public readonly string? Text;   

        public TelegramButton(string? text,TelegramRoute? toRoute,bool isLink)
        {
            Text = text;
            ToRoute = toRoute;
            IsLink = isLink;
        }
    }
}
