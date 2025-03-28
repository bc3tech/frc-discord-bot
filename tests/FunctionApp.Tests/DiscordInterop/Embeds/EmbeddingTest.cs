namespace FunctionApp.Tests.DiscordInterop.Embeds;
using FunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.Logging;

using TestCommon;

using Xunit.Abstractions;

public class EmbeddingTest : TestWithLogger
{
    public EmbeddingTest(Type loggerType, ITestOutputHelper outputHelper) : base(loggerType, outputHelper) => this.Mocker.Use(new EmbedBuilderFactory(new EmbeddingColorizer(this.Mocker.WithSelfMock<FRCColors.IClient>(), this.Mocker.Get<ILoggerFactory>().CreateLogger<EmbeddingColorizer>())));
}
