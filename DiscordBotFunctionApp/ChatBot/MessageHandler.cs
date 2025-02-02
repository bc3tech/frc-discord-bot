namespace DiscordBotFunctionApp.ChatBot;

using Discord;

using Microsoft.AspNetCore.SignalR.Client;

internal sealed class MessageHandler(HubConnectionFactory hubConnections)
{
    public async Task HandleUserMessageAsync(IUserMessage msg, CancellationToken cancellationToken = default)
    {
        using var t = msg.Channel.EnterTypingState();
        var conn = await hubConnections.StartConnectionForUserAsync(msg.Author, cancellationToken).ConfigureAwait(false);
        var answer = await conn.InvokeAsync<string>(Constants.SignalR.Functions.GetAnswer, Constants.SignalR.Users.Orchestrator, msg.Content, cancellationToken).ConfigureAwait(false);

        await msg.ReplyAsync(answer);
    }
}
