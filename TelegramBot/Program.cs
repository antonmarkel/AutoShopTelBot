// See https://aka.ms/new-console-template for more information
using Telegram.Bot.Types;

internal class Program
{

 

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var bot = new TelegramBot.TelegramBot("7106270577:AAHNhXB5NSa6BZkTSOjQX_BsPZShneyqvU8");
        bot.Start().Wait();
    }
}