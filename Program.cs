using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Microsoft.Data.Sqlite;
using Telegram.Bot.Exceptions;
using System.Collections;
using NLog;
using System.IO;

namespace TelegramBotExperiments
{

    class Program
    {
        static string kod = System.IO.File.ReadAllText(@"token.txt");
        static ITelegramBotClient bot = new TelegramBotClient(kod.ToString());
        private static Logger logger = LogManager.GetCurrentClassLogger();




        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            //Start/Info/Help
            logger.Debug("log {0}", "Start/Help/Info");

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                System.IO.File.WriteAllText("text.txt", $"Message:{message.Text}, message_id:{message.MessageId}");

                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать в Telegram-бот по подсчету БЖУ в продуктах!");
                    return;
                }
                if (message.Text.ToLower() == "/info")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Данный бот позволит рассчитать БЖУ продуктов на 100гр");
                    return;
                }
                if (message.Text.ToLower() == "/help")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Для работы с ботом, необходимо воспользоваться кнопками: Выбрать категорию продукта, затем сам продукт.");
                    return;
                }

                await botClient.SendTextMessageAsync(message.Chat, "Не понимаю, обратитесь в /help!");


            }

        }


        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));

        }


        static void Main(string[] args)
        {


            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
            logger.Debug("log {0}", "Event handler");

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            //подключение событий
            {
                AllowedUpdates = { }, // получать все типы обновлений
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
            //Подключение бд к основе
            using (var connection = new SqliteConnection("Data Source=BGY.db"))
            {
                connection.Open();
            }
            Console.Read();


        }

    }
}