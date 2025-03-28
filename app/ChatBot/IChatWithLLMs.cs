namespace FunctionApp.ChatBot;

using System.Collections.Generic;
using System.Threading;

internal interface IChatWithLLMs
{
    IAsyncEnumerable<string> GetCompletionsAsync(string prompt, CancellationToken cancellationToken = default);
}