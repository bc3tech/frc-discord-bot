namespace TestCommon;
using Discord;
using Discord.Interactions;

using FunctionApp.DiscordInterop.CommandModules;

using Moq;

using System;
using System.Threading.Tasks;

using Xunit.Abstractions;

public class TestWithDiscordInteraction<T> : TestWithLogger where T : CommandModuleBase
{
    protected TestWithDiscordInteraction(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        this.MockContext = this.Mocker.GetMock<IInteractionContext>();

        var interactionMock = this.MockInteraction = this.Mocker.GetMock<IDiscordInteraction>();
        interactionMock.Setup(i => i.DeferAsync(It.IsAny<bool>(), It.IsAny<RequestOptions>()))
                .Returns(Task.FromResult(Mock.Of<IDisposable>()));

        this.MockContext.SetupGet(c => c.Interaction)
                .Returns(interactionMock.Object);

        var channelMock = this.MockChannel = this.Mocker.GetMock<IMessageChannel>();
        channelMock.Setup(i => i.EnterTypingState(It.IsAny<RequestOptions>()))
                .Returns(Mock.Of<IDisposable>());
        this.MockContext.SetupGet(c => c.Channel).Returns(channelMock.Object);
    }

    protected Mock<IInteractionContext> MockContext { get; }
    protected Mock<IDiscordInteraction> MockInteraction { get; }
    protected Mock<IMessageChannel> MockChannel { get; }

    private T? _module;
    protected T Module
    {
        get => _module ?? throw new NullReferenceException("Module was never set!");
        set
        {
            ((IInteractionModuleBase)value).SetContext(this.MockContext.Object);
            _module = value;
        }
    }
}
