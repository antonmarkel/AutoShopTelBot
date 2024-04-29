// See https://aka.ms/new-console-template for more information
using Telegram.Bot.Types;
using TelegramBot.TelegramAPI;

internal class Program
{

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        TelegramBot.TelegramAPI.TelegramBot bot = new TelegramBot.TelegramAPI.TelegramBot("6861626082:AAEOxim8Z-ObAgaorFw0EtkFe7Y-MdyfkIA");
        bot.Start().Wait();

    }
}