namespace DiscordBotFunctionApp.Functions;

using Azure;
using Azure.AI.Projects;
using Azure.Data.Tables;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

internal sealed class MatchSummariesUpdater(AgentsClient client,
                                            [FromKeyedServices(Constants.ServiceKeys.TableClient_VectorStoreFiles)] TableClient filesTable,
                                            IHttpClientFactory httpClientFactory, IConfiguration appConfig,
                                            ILogger<MatchSummariesUpdater> logger)
{
    private const string httpClientName = "GoogleDocs";
    private const string matchSummariesPdfName = "Match Summaries 2025.pdf";

    [Function("MatchSummariesUpdater")]
    public async Task RunAsync([TimerTrigger("*/15 * * * *"
#if DEBUG
        , RunOnStartup = true
#endif
        )] TimerInfo myTimer, CancellationToken cancellationToken)
    {
#if !DEBUG
        var currentTime = TimeProvider.System.GetLocalNow();
        if (currentTime.DayOfWeek is not DayOfWeek.Friday and not DayOfWeek.Saturday and not DayOfWeek.Sunday)
        {
            logger.NotRunningOnFridaySaturdayOrSundaySkippingMatchSummariesUpdate();
            return;
        }

        if (currentTime.Hour is < 7 or >= 21)
        {
            logger.OutsideNormalMatchHoursSkippingUpdate();
            return;
        }
#endif

        logger.RunningMatchSummaryDocUpdate();

        var matchSummariesDocUrlConfigVal = appConfig[Constants.Configuration.MatchSummariesDocumentUrl];
        var existingAgentId = (await client.GetAgentsAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).Value.FirstOrDefault(i => i.Name is "BearMetalBot-SK")?.Id;
        while (existingAgentId is null && !cancellationToken.IsCancellationRequested)
        {
            logger.DidnTFindTheTargetAgentToUpdateWaitingForItToBeAvailable();
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
        }

        if (existingAgentId is not null)
        {
            if (!string.IsNullOrWhiteSpace(matchSummariesDocUrlConfigVal))
            {
                var agent = (await client.GetAgentAsync(existingAgentId, cancellationToken: cancellationToken).ConfigureAwait(false)).Value;
                if (agent.ToolResources.FileSearch.VectorStoreIds.Count is 1)
                {
                    var matchSummariesDocUrl = new Uri(matchSummariesDocUrlConfigVal);
                    var vectorStoreId = agent.ToolResources.FileSearch.VectorStoreIds[0];

                    var summariesUploadTrackingRecord = await filesTable.GetEntityIfExistsAsync<TableEntity>(vectorStoreId, matchSummariesPdfName, cancellationToken: cancellationToken).ConfigureAwait(false);
                    if (!summariesUploadTrackingRecord.HasValue)
                    {
                        logger.NoTrackingRecordFoundInTableUploadingNewFile();
                        var newFile = (await UploadMatchSummariesAsync(agent, matchSummariesDocUrl, cancellationToken: cancellationToken).ConfigureAwait(false))!;

                        await filesTable.AddEntityAsync(new TableEntity(vectorStoreId, matchSummariesPdfName)
                        {
                            ["FileId"] = newFile.Id,
                            ["FilePurpose"] = newFile.Purpose.ToString(),
                            ["FileSize"] = newFile.Size,
                        }, cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        logger.FoundTrackingRecordInTableCheckingIfReuploadIsNeeded();
                        var fileId = summariesUploadTrackingRecord.Value!["FileId"].ToString();
                        bool reuploadNeeded = false;
                        try
                        {
                            var file = (await client.GetVectorStoreFileAsync(vectorStoreId, fileId, cancellationToken).ConfigureAwait(false)).Value;
                            reuploadNeeded = file is null;
                        }
                        catch
                        {
                            reuploadNeeded = true;
                        }

                        if (reuploadNeeded)
                        {
                            int existingFilesize = (int)summariesUploadTrackingRecord.Value!["FileSize"];
                            var newFile = await UploadMatchSummariesAsync(agent, matchSummariesDocUrl, existingFilesize, cancellationToken).ConfigureAwait(false);
                            if (newFile is not null)
                            {
                                Debug.Assert(newFile.Size != existingFilesize);
                                await filesTable.UpdateEntityAsync(new TableEntity(vectorStoreId, matchSummariesPdfName)
                                {
                                    ["FileId"] = newFile.Id,
                                    ["FilePurpose"] = newFile.Purpose.ToString(),
                                    ["FileSize"] = newFile.Size,
                                }, ETag.All, mode: TableUpdateMode.Replace, cancellationToken).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
        }
    }

    private async Task<AgentFile?> UploadMatchSummariesAsync(Agent agent, Uri matchSummariesDocUrl, int? existingFilesize = null, CancellationToken cancellationToken = default)
    {
        logger.LoadingTeamMatchSummariesPDFFromGoogleDocs();
        var response = await httpClientFactory.CreateClient(httpClientName).GetAsync(matchSummariesDocUrl, cancellationToken).ConfigureAwait(false);

        if (existingFilesize.HasValue && response.Content.Headers.ContentLength == existingFilesize)
        {
            logger.MatchSummariesPDFAlreadyUploadedAndIsTheSameSizeAsTheExistingFileNoNeedToReUpload();
            return null;
        }

        logger.UploadingTeamMatchSummariesPDFToAzureAI();
        var newFile = await client.UploadFileAsync(await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false), AgentFilePurpose.Agents, matchSummariesPdfName, cancellationToken).ConfigureAwait(false);
        await client.CreateVectorStoreFileAsync(agent.ToolResources.FileSearch.VectorStoreIds[0], newFile.Value.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
        logger.UploadedTeamMatchSummariesPDFToAzureAI();
        return newFile.Value;
    }
}
