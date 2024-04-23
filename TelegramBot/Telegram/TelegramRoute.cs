using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Telegram
{
    public class TelegramRoute
    {
        public readonly string Page;
        public TelegramRoute(string route)
        {
            Page = route;
        }

        public TelegramRoute Back()
        {
            if (Page == "main") return this;
            int newSize = 0;
            for (int i = Page.Length - 1; i > -1; i--)
            {
                if (Page[i] == '/') { newSize = i; break; }
            }
            return new TelegramRoute(Page[0..(newSize + 1)]);
        }

        public TelegramRoute Next(string path)
        {
            return new TelegramRoute(Page + '/' + path);
        }
    
    }
}
