namespace FunctionApp.Tests.ChatBot;

using Azure.Core;
using Azure.Data.Tables;

using BC3Technologies.DiscordGpt.Copilot;
using BC3Technologies.DiscordGpt.Core;
using BC3Technologies.DiscordGpt.Storage.TableStorage;

using Discord;
using GitHub.Copilot.SDK.Rpc;

using global::ChatBot;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Moq;
using Moq.AutoMock;

public sealed class DiscordGptIntegrationTests
{
    [Fact]
    public async Task HandleUserMessageAsyncForwardsDirectMessagesWithExpectedEnvelope()
    {
        AutoMocker mocker = new();
        SetupTracingMocks(mocker);
        MessageHandler sut = mocker.CreateInstance<MessageHandler>();
        Mock<IDiscordEventHandler> eventHandler = mocker.GetMock<IDiscordEventHandler>();
        MessageCreatedEvent? capturedEvent = null;

        eventHandler
            .Setup(handler => handler.HandleAsync(It.IsAny<DiscordEvent>(), It.IsAny<CancellationToken>()))
            .Callback<DiscordEvent, CancellationToken>((@event, _) => capturedEvent = @event as MessageCreatedEvent)
            .Returns(Task.CompletedTask);

        IUserMessage message = CreateUserMessage(
            userId: 42UL,
            username: "bc3tech",
            globalName: "Brandon H",
            channelId: 2046UL,
            cleanContent: "hello from dm",
            messageId: 1001UL,
            isBotAuthor: false);

        await sut.HandleUserMessageAsync(message, TestContext.Current.CancellationToken);

        eventHandler.Verify(handler => handler.HandleAsync(It.IsAny<DiscordEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(capturedEvent);
        Assert.Equal("2046", capturedEvent.ChannelId);
        Assert.Null(capturedEvent.GuildId);
        Assert.Equal("42", capturedEvent.UserId);
        Assert.Equal("Brandon H", capturedEvent.UserDisplayName);
        Assert.Equal("1001", capturedEvent.MessageId);
        Assert.True(capturedEvent.IsDm);
        Assert.False(capturedEvent.BotMentioned);
        Assert.Empty(capturedEvent.MentionedRoleIds);
        Assert.Contains("hello from dm", capturedEvent.Content, StringComparison.Ordinal);
        Assert.Contains("===User Display Name: Brandon H", capturedEvent.Content, StringComparison.Ordinal);
        Assert.Contains("User Id: bc3tech===", capturedEvent.Content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task HandleUserMessageAsyncSkipsBotAuthoredMessages()
    {
        AutoMocker mocker = new();
        MessageHandler sut = mocker.CreateInstance<MessageHandler>();
        Mock<IDiscordEventHandler> eventHandler = mocker.GetMock<IDiscordEventHandler>();

        IUserMessage message = CreateUserMessage(
            userId: 42UL,
            username: "bot-user",
            globalName: "bot-user",
            channelId: 2046UL,
            cleanContent: "ignored",
            messageId: 1002UL,
            isBotAuthor: true);

        await sut.HandleUserMessageAsync(message, TestContext.Current.CancellationToken);

        eventHandler.Verify(handler => handler.HandleAsync(It.IsAny<DiscordEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void AddFrcChatBotRegistersExpectedDiscordGptOptionsAndServices()
    {
        IConfiguration configuration = BuildConfiguration(
            ("AI:Copilot:Model", "gpt-5.4-mini"),
            ("AI:Foundry:Endpoint", "https://example.services.ai.azure.com/api/projects/frc"),
            ("AI:Foundry:LocalAgentModel", "gpt-5.4-mini"),
            ("AI:Foundry:AgentId", "agent-id"),
            ("AI:Foundry:MealSignupGeniusId", "signup-board"),
            ("AI:Foundry:OpenAIApiVersion", "2025-06-01"),
            ("Discord:Token", "discord-token"),
            ("Discord:ApplicationId", "1234567890"),
            ("TbaApiKey", "tba-api-key"));

        ServiceCollection services = new();
        services.AddSingleton(configuration);
        services.AddLogging();
        services.AddSingleton(new TableServiceClient("UseDevelopmentStorage=true"));
        services.AddSingleton(Mock.Of<IDiscordClient>());

        services.TryAddChatBot(configuration, new Uri("https://storageacct.blob.core.windows.net"), out bool success, out string[] validationFailures);
        Assert.True(success);

        using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);

        DiscordGptCoreOptions coreOptions = provider.GetRequiredService<IOptions<DiscordGptCoreOptions>>().Value;
        Assert.Equal("discord-token", coreOptions.BotToken);
        Assert.Equal("1234567890", coreOptions.ApplicationId);
        Assert.Equal(50, coreOptions.MaxHistoryLength);

        TableConversationStoreOptions storeOptions = provider.GetRequiredService<IOptions<TableConversationStoreOptions>>().Value;
        Assert.Equal(ChatBotConstants.ServiceKeys.TableClient_UserChatAgentThreads, storeOptions.TableName);

        CopilotToolAuthorizationOptions authorization = provider.GetRequiredService<IOptions<CopilotToolAuthorizationOptions>>().Value;
        Assert.True(authorization.AllowAllTools);
        Assert.True(authorization.AllowToolsInDirectMessages);

        Assert.IsType<TableConversationStore>(provider.GetRequiredService<IConversationStore>());
        Assert.NotNull(provider.GetRequiredService<MessageHandler>());
    }

    [Fact]
    public void AddFrcChatBotUsesDiscordAppIdAliasWhenApplicationIdIsMissing()
    {
        IConfiguration configuration = BuildConfiguration(
            ("AI:Copilot:Model", "gpt-5.4-mini"),
            ("AI:Foundry:Endpoint", "https://example.services.ai.azure.com/api/projects/frc"),
            ("AI:Foundry:LocalAgentModel", "gpt-5.4-mini"),
            ("AI:Foundry:AgentId", "agent-id"),
            ("AI:Foundry:MealSignupGeniusId", "signup-board"),
            ("Discord:Token", "discord-token"),
            ("Discord:AppId", "app-id-alias"),
            ("TbaApiKey", "tba-api-key"));

        ServiceCollection services = new();
        services.AddSingleton(configuration);
        services.AddLogging();
        services.AddSingleton(new TableServiceClient("UseDevelopmentStorage=true"));
        services.AddSingleton(Mock.Of<IDiscordClient>());

        services.TryAddChatBot(configuration, new Uri("https://storageacct.blob.core.windows.net"), out bool success, out string[] validationFailures);
        Assert.True(success);

        using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);
        DiscordGptCoreOptions coreOptions = provider.GetRequiredService<IOptions<DiscordGptCoreOptions>>().Value;
        Assert.Equal("app-id-alias", coreOptions.ApplicationId);
    }

    [Fact]
    public void AddFrcChatBotWithInvalidConfigurationDoesNotRegisterChatbotServices()
    {
        IConfiguration configuration = BuildConfiguration(
            ("AI:Foundry:Endpoint", "https://example.services.ai.azure.com/api/projects/frc"),
            ("AI:Foundry:MealSignupGeniusId", "signup-board"),
            ("TbaApiKey", "tba-api-key"));

        ServiceCollection services = new();
        services.AddSingleton(configuration);
        services.AddLogging();
        services.AddSingleton(new TableServiceClient("UseDevelopmentStorage=true"));
        services.TryAddChatBot(
            configuration,
            new Uri("https://storageacct.blob.core.windows.net"),
            out bool isEnabled,
            out string[] validationFailures);
        Assert.False(isEnabled);
        Assert.Contains(validationFailures, failure => failure.Contains("AI:Foundry:LocalAgentModel", StringComparison.Ordinal));
        Assert.Contains(validationFailures, failure => failure.Contains("Discord:Token", StringComparison.Ordinal));

        using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);
        Assert.Null(provider.GetService<MessageHandler>());
        Assert.Null(provider.GetService<IConversationStore>());
        Assert.Null(provider.GetService<IDiscordEventHandler>());
    }

    [Fact]
    public void AddFrcChatBotWhenBlobServiceUriIsHttpThrowsInvalidOperationException()
    {
        IConfiguration configuration = BuildConfiguration(
            ("AI:Copilot:Model", "gpt-5.4-mini"),
            ("AI:Foundry:Endpoint", "https://example.services.ai.azure.com/api/projects/frc"),
            ("AI:Foundry:LocalAgentModel", "gpt-5.4-mini"),
            ("AI:Foundry:AgentId", "agent-id"),
            ("AI:Foundry:MealSignupGeniusId", "signup-board"),
            ("Discord:Token", "discord-token"),
            ("Discord:ApplicationId", "1234567890"),
            ("TbaApiKey", "tba-api-key"));

        ServiceCollection services = new();
        services.AddSingleton(configuration);
        services.AddLogging();
        services.AddSingleton(new TableServiceClient("UseDevelopmentStorage=true"));

        services.TryAddChatBot(configuration, new Uri("http://storageacct.blob.core.windows.net"), out bool success, out string[] validationFailures);
        Assert.False(success);
    }

    [Fact]
    public async Task ChatThreadResetterClearsExpectedConversationScopes()
    {
        Mock<IConversationStore> store = new(MockBehavior.Strict);
        store.Setup(s => s.ClearAsync(ConversationKey.Dm("123"), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();
        store.Setup(s => s.ClearAsync(ConversationKey.Thread("456"), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();

        ServiceCollection services = new();
        services.AddSingleton(store.Object);
        await using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);

        await ChatThreadResetter.ResetThreadForUserAsync(provider, 123UL, TestContext.Current.CancellationToken);
        await ChatThreadResetter.CleanupDeletedThreadAsync(provider, 456UL, TestContext.Current.CancellationToken);

        store.VerifyAll();
    }

    [Fact]
    public async Task ChatThreadResetterClearsCopilotSessionState()
    {
        Mock<IConversationStore> store = new(MockBehavior.Strict);
        store.Setup(s => s.ClearAsync(ConversationKey.Dm("123"), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();

        Mock<IConversationSessionMap> sessionMap = new(MockBehavior.Strict);
        sessionMap
            .Setup(m => m.GetSessionIdAsync(ConversationKey.Dm("123").ToStorageKey(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("session-123")
            .Verifiable();
        sessionMap
            .Setup(m => m.RemoveAsync(ConversationKey.Dm("123").ToStorageKey(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        SessionFsRmParams? removedPath = null;
        Mock<ISessionFsHandler> sessionFs = new(MockBehavior.Strict);
        sessionFs
            .Setup(fs => fs.RmAsync(It.IsAny<SessionFsRmParams>(), It.IsAny<CancellationToken>()))
            .Callback<SessionFsRmParams, CancellationToken>((request, _) => removedPath = request)
            .Returns(Task.CompletedTask)
            .Verifiable();

        ServiceCollection services = new();
        services.AddSingleton(store.Object);
        services.AddSingleton(sessionMap.Object);
        services.AddSingleton(sessionFs.Object);
        await using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);

        await ChatThreadResetter.ResetThreadForUserAsync(provider, 123UL, TestContext.Current.CancellationToken);

        store.VerifyAll();
        sessionMap.VerifyAll();
        sessionFs.VerifyAll();
        Assert.NotNull(removedPath);
        Assert.Equal("session-123", removedPath.SessionId);
        Assert.Equal("/", removedPath.Path);
        Assert.True(removedPath.Recursive);
    }

    private static IConfiguration BuildConfiguration(params (string Key, string Value)[] entries)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(entries.Select(static entry => new KeyValuePair<string, string?>(entry.Key, entry.Value)))
            .Build();

    private static void SetupTracingMocks(AutoMocker mocker)
    {
        mocker.GetMock<IConversationKeyResolver>()
            .Setup(r => r.ResolveAsync(It.IsAny<DiscordEvent>(), It.IsAny<CancellationToken>()))
            .Returns<DiscordEvent, CancellationToken>((evt, _) => Task.FromResult(
                evt is MessageCreatedEvent { IsDm: true } dm
                    ? ConversationKey.Dm(dm.UserId)
                    : ConversationKey.Reply(((MessageCreatedEvent)evt).MessageId)));

        Mock<global::CopilotSdk.OpenTelemetry.IConversationScope> conversationScope = new();
        conversationScope.SetupGet(s => s.Activity).Returns((System.Diagnostics.Activity?)null);
        conversationScope.SetupGet(s => s.ConversationId).Returns(string.Empty);
        conversationScope.SetupGet(s => s.IsNew).Returns(true);
        conversationScope.Setup(s => s.DisposeAsync()).Returns(ValueTask.CompletedTask);

        Mock<global::CopilotSdk.OpenTelemetry.IConversationTurnScope> turnScope = new();
        turnScope.SetupGet(s => s.Activity).Returns((System.Diagnostics.Activity?)null);
        turnScope.SetupGet(s => s.ConversationId).Returns(string.Empty);
        turnScope.Setup(s => s.DisposeAsync()).Returns(ValueTask.CompletedTask);

        mocker.GetMock<global::CopilotSdk.OpenTelemetry.IConversationTracer>()
            .Setup(t => t.CreateOrResumeConversationAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyDictionary<string, object?>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(conversationScope.Object));

        mocker.GetMock<global::CopilotSdk.OpenTelemetry.IConversationTracer>()
            .Setup(t => t.BeginTurn(It.IsAny<string>()))
            .Returns(turnScope.Object);
    }

    private static IUserMessage CreateUserMessage(
        ulong userId,
        string username,
        string globalName,
        ulong channelId,
        string cleanContent,
        ulong messageId,
        bool isBotAuthor)
    {
        Mock<IUser> user = new(MockBehavior.Strict);
        user.SetupGet(u => u.Id).Returns(userId);
        user.SetupGet(u => u.Username).Returns(username);
        user.SetupGet(u => u.GlobalName).Returns(globalName);
        user.SetupGet(u => u.IsBot).Returns(isBotAuthor);

        Mock<IDMChannel> channel = new(MockBehavior.Strict);
        channel.SetupGet(c => c.Id).Returns(channelId);

        Mock<IUserMessage> message = new(MockBehavior.Strict);
        message.SetupGet(m => m.Author).Returns(user.Object);
        message.SetupGet(m => m.Channel).Returns(channel.Object);
        message.SetupGet(m => m.CleanContent).Returns(cleanContent);
        message.SetupGet(m => m.Id).Returns(messageId);
        message.SetupGet(m => m.Timestamp).Returns(new DateTimeOffset(2026, 4, 20, 0, 0, 0, TimeSpan.Zero));

        return message.Object;
    }
}
