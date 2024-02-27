using HK_ArbitrageBot.Exceptions;
using HK_ArbitrageBot.Models;
using Quartz;
using Telegram.Bot;

namespace HK_ArbitrageBot.Jobs;

internal class CompareJob : IJob
{
   
    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Запуск итерации");
        using (var client = new AloraClient())
        {
            Program._securities = await client.GetSecurities();
            if (!Program._securities.Any())
                throw new CompareJobException("Список бумаг пуст. Не смогли скачать с api");
            
            foreach(var item in Program._securities)
            {
                var res = await client.GetQuotes(item);
            }
            

        
        }

       /* Console.WriteLine($"{DateTime.Now.ToString("dd-MM-YYYY HH:mm")} Получили Данные для  для {ticker}: \n" +
            $"Ask: {quotes?.Ask}\n" +
            $"Price: {quotes?.LastPrice}\n" +
            $"Close_Price: {quotes?.PrevClosePrice}");


        Program._chaitIds.ForEach(async x => await Program._botClient.SendTextMessageAsync(x,
                                                            $"Получены данные\n" +
                                                            $" {ticker}\n" +
                                                            $"Цена закрытия: {quotes?.PrevClosePrice}\n" +
                                                            $"Текущая цена: {quotes?.LastPrice}",
                                                            replyMarkup: null));

        if (quotes?.LastPrice < quotes?.PrevClosePrice)
        {
            Program._chaitIds.ForEach(async x => await Program._botClient.SendTextMessageAsync(x,
                                                            $"Текущая цена ниже цены закрытия для\n" +
                                                            $" {ticker}\n" +
                                                            $"Цена закрытия: {quotes.PrevClosePrice}\n" +
                                                            $"Текущая цена: {quotes.LastPrice}",
                                                            replyMarkup: null));
        }
    }*/
    }
}
