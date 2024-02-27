using HK_ArbitrageBot.Exceptions;
using HK_ArbitrageBot.Models;
using Quartz.Impl;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using HK_ArbitrageBot.Jobs;

namespace HK_ArbitrageBot;

class Program
{
    public static TelegramBotClient _botClient = new TelegramBotClient("вот сюда токен бота");
    public static List<long> _chaitIds = new List<long>();
    public static List<Security> _securities = new List<Security>();

    static async Task Main(string[] args)
    {
        await StartListeningBot();

        await StartJob();
      
        Console.ReadLine();
    }

    private static async Task StartListeningBot()
    {
        try
        {
            await ListenForMessagesAsync();
        }
        catch (Exception ex)
        {
            throw new TelegramException(ex.Message);
        }
    }

    private static async Task StartJob()
    {
        IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();


        IJobDetail job = JobBuilder.Create<CompareJob>().Build();

        ITrigger trigger = TriggerBuilder
            .Create()
            .WithDailyTimeIntervalSchedule(s =>
                s.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(23, 00))
                .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(23, 59))
                .WithIntervalInMinutes(1))
        .Build();

        await scheduler.ScheduleJob(job, trigger);
        await scheduler.Start();
    }


    private static async Task ListenForMessagesAsync()
    {
        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }
        };

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token);

        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"Start listening for @{me.Username}");
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken token)
    {
        Console.WriteLine($"Получено сообщение от {update.Message.From.Id} {update.Message.From.Username}");

        if (update.Type == UpdateType.Message)
        {

            await botClient.SendChatActionAsync(update.Message.Chat.Id, Telegram.Bot.Types.Enums.ChatAction.Typing);
           //MessageHandler messageHandler = new MessageHandler(update, botClient, _applicationDbContext, _appOptions, _logger, _emailService);

            //Если прислали контакт, обновляем телефон
           

            //Далее обрабатываем текст
            if (update.Message?.Text is null)
                return;

            if (update.Message?.Text == "/start")
            {
                if (!_chaitIds.Any(x => x == update.Message.Chat.Id))
                    _chaitIds.Add(update.Message.Chat.Id);
      
                await botClient.SendTextMessageAsync(update.Message.Chat.Id,
                                                            $"Бот приветствует Женю Шу. Вы приняты в Яндекс-хуяндекс, а также в Пиньков, в общем-то, атлична",
                                                            replyMarkup: null);

            }

        }
       
    }

    private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString(),
        };

        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(errorMessage));
        return Task.CompletedTask;
    }
}
