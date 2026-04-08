namespace Common.Discord;

public sealed class DiscordDeliveryException(string message, Exception? innerException = null) : Exception(message, innerException);
