namespace HK_ArbitrageBot.Exceptions;

internal class TelegramException : Exception
{
    public TelegramException(string message)
        : base(message)
    { }
}
