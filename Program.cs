using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    class Program
    {
        // Токен вашего бота
        private static string BotToken = "7055767364:AAHbA2neU5q1BCRyzZ02iz_-XhRGcm2RFfw";

        // Список участников розыгрыша
        private static List<long> Participants = new List<long>();

        static void Main(string[] args)
        {
            // Создаем клиент Telegram Bot
            var botClient = new TelegramBotClient(BotToken);

            // Подписываемся на обновления от Telegram API
            botClient.OnMessage += BotOnMessageReceived;

            // Начинаем прослушивание сообщений
            botClient.StartReceiving();
            Console.WriteLine("Бот запущен. Нажмите Ctrl+C для завершения.");
            Console.ReadLine();

            // Останавливаем прослушивание сообщений
            botClient.StopReceiving();
        }

        // Обработчик сообщений
        private static async Task BotOnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;

            // Проверяем, что сообщение не от бота
            if (message.From.Id != message.Chat.Id)
            {
                // Обрабатываем команду "/start"
                if (message.Text == "/start")
                {
                    await e.Bot.SendTextMessageAsync(message.Chat.Id, "Привет! Я бот для проведения розыгрышей. " +
                        "Чтобы принять участие в розыгрыше, напишите /join. " +
                        "Чтобы узнать список участников, напишите /list. " +
                        "Чтобы провести розыгрыш, напишите /draw.");
                }
                // Обрабатываем команду "/join"
                else if (message.Text == "/join")
                {
                    if (!Participants.Contains(message.From.Id))
                    {
                        Participants.Add(message.From.Id);
                        await e.Bot.SendTextMessageAsync(message.Chat.Id, "Вы успешно добавлены в список участников.");
                    }
                    else
                    {
                        await e.Bot.SendTextMessageAsync(message.Chat.Id, "Вы уже участвуете в розыгрыше.");
                    }
                }
                // Обрабатываем команду "/list"
                else if (message.Text == "/list")
                {
                    if (Participants.Count == 0)
                    {
                        await e.Bot.SendTextMessageAsync(message.Chat.Id, "Список участников пуст.");
                    }
                    else
                    {
                        string participantsList = "Список участников:\n";
                        foreach (var participant in Participants)
                        {
                            participantsList += $"- @{e.Bot.GetChatMemberAsync(message.Chat.Id, participant).Result.User.Username}\n";
                        }
                        await e.Bot.SendTextMessageAsync(message.Chat.Id, participantsList);
                    }
                }
                // Обрабатываем команду "/draw"
                else if (message.Text == "/draw")
                {
                    if (Participants.Count == 0)
                    {
                        await e.Bot.SendTextMessageAsync(message.Chat.Id, "Список участников пуст.");
                    }
                    else
                    {
                        // Выбираем случайного победителя
                        Random random = new Random();
                        int winnerIndex = random.Next(Participants.Count);
                        long winnerId = Participants[winnerIndex];

                        // Получаем имя победителя
                        var winner = await e.Bot.GetChatMemberAsync(message.Chat.Id, winnerId);
                        string winnerName = winner.User.Username;

                        // Объявляем победителя
                        await e.Bot.SendTextMessageAsync(message.Chat.Id, $"Победитель розыгрыша: @{winnerName}!");
                    }
                }
            }
        }
    }
}