using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions;
using Telegram.Bot.Polling;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;

namespace TelegramBotAllrequests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            {
                //2100763550:AAFgGtUdbO5IDcf-OpiHJ-0Xe8fZhABVb6Y
                TelegramBotClient botClient = new TelegramBotClient("2100763550:AAFgGtUdbO5IDcf-OpiHJ-0Xe8fZhABVb6Y");

                using CancellationTokenSource cts = new();

                // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
                ReceiverOptions receiverOptions = new()
                {
                    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
                };

                botClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandlePollingErrorAsync,
                    receiverOptions: receiverOptions,
                    cancellationToken: cts.Token
                );

                var me = await botClient.GetMeAsync();

                Console.WriteLine($"Start listening for @{me.Username}");
                Console.ReadLine();

                // Send cancellation request to stop bot
                cts.Cancel();

            }

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                    return;
                // Only process text messages
                if (message.Text is not { } messageText)
                    return;

                var chatId = message.Chat.Id;

                Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

                // Echo received message text
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "You said:\n" + messageText,
                    cancellationToken: cancellationToken);
            }

            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
    }
}