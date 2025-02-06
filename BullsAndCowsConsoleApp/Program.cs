using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BullsAndCowsConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        TelegramBotClient tgClient = new("7860010580:AAGIp8CKchbvH9XQc32wceTzZJG4JsH1vGQ");
        tgClient.StartReceiving(HandleUpdate, HandleError);

        Console.ReadKey();
    }


    private static async Task HandleUpdate(
        ITelegramBotClient client,
        Update update,
        CancellationToken token)
    {
        if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            Message? message = update.Message;

            long chatId = message.Chat.Id;
            string? text = message.Text; 

            if(text == "/start")
                await SendResponseStartCommand(client, message, chatId);
        }
        else if(update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
        {
            long chatId = update.CallbackQuery.Message.Chat.Id;
            var data = update.CallbackQuery.Data;


        }



        //await client.SendMessage(chatId, "test");
    }

    private static async Task SendResponseStartCommand(ITelegramBotClient client, Message? message, long chatId)
    {
        string answer = $"Здравствуйте {message.From.FirstName}, вы зашли в игру быки и коровы! Выберете действие";

        InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(
            [
                [InlineKeyboardButton.WithCallbackData("Правила игры", "/rules")],
                        [InlineKeyboardButton.WithCallbackData("Начать играть", "/start_game")]
            ]
            );

        await client.SendMessage(chatId, answer, replyMarkup: keyboard);
    }

    private static async Task HandleError(
        ITelegramBotClient client,
        Exception exception,
        HandleErrorSource source,
        CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
