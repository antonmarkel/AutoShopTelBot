// See https://aka.ms/new-console-template for more information
using Telegram.Bot.Types;
using TelegramBot.TelegramAPI;

internal class Program
{

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        TelegramBot.TelegramAPI.TelegramBot bot = new TelegramBot.TelegramAPI.TelegramBot("7169965872:AAEf3TXPVkg2td9CC1cA9o5OWZ9psR3JUUU");
        TelegramBot.TelegramAPI.TelegramBot.CurrentBot = bot;
        bot.Start().Wait();

    }
}