using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BullsAndCowsConsoleApp;

internal class Program
{
    static string hiddenNumber;

    static void Main(string[] args)
    {
        TelegramBotClient tgClient = new("7860010580:AAGIp8CKchbvH9XQc32wceTzZJG4JsH1vGQ");
        tgClient.StartReceiving(HandleUpdate, HandleError);
        Console.WriteLine("Bot started");
        Console.ReadKey();
    }


    private static async Task HandleUpdate(
        ITelegramBotClient client,
        Update update,
        CancellationToken token)
    {
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            Message? message = update.Message;

            long chatId = message!.Chat.Id;
            string? text = message.Text;

            if (text == "/start")
                await SendResponseStartGameCommand(client, message);
            else if(text!.Length > 0)
            {
                await SendResponseHowManyBullsAndCowsCommand(client, chatId, text);
            }
            else
                await client.SendMessage(chatId, "Не правильный ввод");
        }
        else if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
        {
            long chatId = update!.CallbackQuery!.Message!.Chat.Id;
            var data = update.CallbackQuery.Data;

            switch (data)
            {
                case "/rules":
                    {
                        await SendResponseRulesCommand(client, chatId);
                        break;
                    }
                case "/start_game":
                    {
                        await SendResponseStartGame(client, chatId);
                        break;
                    }

                default: 
                    break;
            }
        }
    }

    private static async Task SendResponseHowManyBullsAndCowsCommand(ITelegramBotClient client, long chatId, string text)
    {
        // когда пользователь пытается угадать число

        (int bullsCount, int cowsCount) = CalculateBullsAndCowsCount(text);

        string answer = $"Число: {text} \n " +
            $"- Быков: {bullsCount} \n" +
            $"- Коров: {cowsCount} \n\n";

        await client.SendMessage(chatId, answer);

        if (bullsCount == 4)
            await client.SendMessage(chatId, "Ура! Вы выиграли! Красавчик!");
    }

    private static async Task SendResponseStartGame(ITelegramBotClient client, long chatId)
    {
        hiddenNumber = GenerateHiddenNumber();

        string text = $"Бот загадал число {hiddenNumber}! \n Введите ваше число:";
        await client.SendMessage(chatId, text);
    }

    private static (int bullsCount, int cowsCount) CalculateBullsAndCowsCount(string text)
    {
        int bullsCount = 0;
        int cowsCount = 0;

        for(int i = 0; i < text.Length; i++)
        {
            for(int j = 0; j < hiddenNumber.Length; j++)
            {
                if(i == j)
                    bullsCount++;
                else
                    cowsCount++;
            }
        }
        return (bullsCount, cowsCount);
    }

    private static string GenerateHiddenNumber()
    {
        List<int> digits = Enumerable.Range(1, 10).ToList();
        Random rnd = new();

        string randomNumber = string.Empty;

        while (randomNumber.Length != 4)
        {
            int randomIndex = rnd.Next(digits.Count);
            randomNumber += digits[randomIndex];
            digits.RemoveAt(randomIndex);
        }
        return randomNumber;
    }

    private static async Task SendResponseRulesCommand(ITelegramBotClient client, long chatId)
    {
        string text = Game.GetRules();

        InlineKeyboardMarkup keyBoard = new([
            [
                InlineKeyboardButton.WithCallbackData("Играть", "/start_game")
            ]]);

        await client.SendMessage(chatId, text, replyMarkup: keyBoard);
    }

    private static async Task SendResponseStartGameCommand(ITelegramBotClient client, Message? message)
    {
        long chatId = message!.Chat.Id;

        string answer = $"Здравствуйте {message!.From!.FirstName}, вы зашли в игру быки и коровы! Выберете действие";

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
        Console.WriteLine(exception.Message);



        //await client.SendMessage(message.Chat.Id, "Ошибка");
    }
}
