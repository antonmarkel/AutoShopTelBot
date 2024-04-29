// See https://aka.ms/new-console-template for more information
using Telegram.Bot.Types;
using TelegramBot.TelegramAPI;

internal class Program
{

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        TelegramBot.TelegramAPI.TelegramBot bot = new TelegramBot.TelegramAPI.TelegramBot("7106270577:AAHNhXB5NSa6BZkTSOjQX_BsPZShneyqvU8");
        bot.Start().Wait();
    }
}