using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AIBitrix.Controllers;

public class TelegramController
{
    private readonly TelegramBotClient _botClient;

    private ILogger _logger;
    
    public TelegramController(string token, ILogger logger)
    {

        _logger = logger;
        _botClient = new TelegramBotClient(token);
        
        using CancellationTokenSource cts = new ();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions receiverOptions = new ()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Message is not { } message)
            return;
        // Only process text messages
        if (message.Text is not { } messageText)
            return;

        try
        {
            var chatId = message.Chat.Id;
        
            if (messageText == "/start")
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Привет! \ud83e\udd17\ud83c\udf89 Мы рады приветствовать вас в нашем Telegram-боте - AI-ассистент компании A&K Educational Consulting! " +
                          "\n\nЗдесь вы можете задать любой вопрос к искусственному интеллекту, который предобучен по базе знаний компании.\n\nС помощью этого бота вы сможете:\n\n1\ufe0f\u20e3 Найти ответы на часто задаваемые вопросы клиентов.\n2\ufe0f\u20e3 Сгенерировать быстрые ответы клиентам.\n3\ufe0f\u20e3 Получить информацию о наших продуктах и услугах.\n\nМы постоянно работаем над улучшением нашего бота, поэтому, пожалуйста, делитесь своим мнением и предложениями руководству. Ваше мнение очень важно для нас! \n\nВремя генерации ответа может превышать 1 минуту!",
                    cancellationToken: cancellationToken);
                return;
            }

            var messageChange = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "\u267b\ufe0f Генерируем ответ! Подождите...",
                cancellationToken: cancellationToken);
        
            var answer = await AIController.Generate(messageText);
        
            // Echo received message text
            await botClient.EditMessageTextAsync(
                chatId: chatId,
                text: answer,
                messageId: messageChange.MessageId,
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}