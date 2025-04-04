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
                                            TimeProvider time,
                                            ILogger<MatchSummariesUpdater> logger)
{
    private const string HttpClientName = "GoogleDocs";
    private const string MatchSummariesPdfName = "2025 Season Bear Metal Match Summaries.pdf";

    [Function("MatchSummariesUpdater")]
    public async Task RunAsync([TimerTrigger("*/15 * * * *"
#if DEBUG
        , RunOnStartup = true
#endif
        )] TimerInfo myTimer, CancellationToken cancellationToken)
    {
#if !DEBUG
        var currentTime = time.GetLocalNow();
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
            await Task.Delay(TimeSpan.FromSeconds(1), time, cancellationToken).ConfigureAwait(false);
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

                    var summariesUploadTrackingRecord = await filesTable.GetEntityIfExistsAsync<TableEntity>(vectorStoreId, MatchSummariesPdfName, cancellationToken: cancellationToken).ConfigureAwait(false);
                    if (!summariesUploadTrackingRecord.HasValue)
                    {
                        logger.NoTrackingRecordFoundInTableUploadingNewFile();
                        var newFile = (await UploadMatchSummariesAsync(agent, matchSummariesDocUrl, cancellationToken: cancellationToken).ConfigureAwait(false))!;

                        await filesTable.AddEntityAsync(new TableEntity(vectorStoreId, MatchSummariesPdfName)
                        {
                            ["FileId"] = newFile.Id,
                            ["FilePurpose"] = newFile.Purpose.ToString(),
                            ["FileSize"] = newFile.Size,
                        }, cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        int existingFilesize = (int)summariesUploadTrackingRecord.Value!["FileSize"];
                        var newFile = await UploadMatchSummariesAsync(agent, matchSummariesDocUrl, existingFilesize, cancellationToken).ConfigureAwait(false);
                        if (newFile is not null)
                        {
                            Debug.Assert(newFile.Size != existingFilesize);
                            await filesTable.UpdateEntityAsync(new TableEntity(vectorStoreId, MatchSummariesPdfName)
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

    private async Task<AgentFile?> UploadMatchSummariesAsync(Agent agent, Uri matchSummariesDocUrl, int? existingFilesize = null, CancellationToken cancellationToken = default)
    {
        logger.LoadingTeamMatchSummariesPDFFromGoogleDocs();
        var response = await httpClientFactory.CreateClient(HttpClientName).GetAsync(matchSummariesDocUrl, cancellationToken).ConfigureAwait(false);

        if (existingFilesize.HasValue && response.Content.Headers.ContentLength == existingFilesize)
        {
            logger.MatchSummariesPDFAlreadyUploadedAndIsTheSameSizeAsTheExistingFileNoNeedToReUpload();
            return null;
        }

        logger.UploadingTeamMatchSummariesPDFToAzureAI();
        var newFile = await client.UploadFileAsync(await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false), AgentFilePurpose.Agents, MatchSummariesPdfName, cancellationToken).ConfigureAwait(false);
        var newVectorStoreFile = await client.CreateVectorStoreFileAsync(agent.ToolResources.FileSearch.VectorStoreIds[0], newFile.Value.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
        var ingestStartTime = time.GetUtcNow();
        logger.UploadedTeamMatchSummariesPDFToAzureAI();

        bool fiveMinWarningGiven = false;
        while (!cancellationToken.IsCancellationRequested)
        {
            var fileStatus = (await client.GetVectorStoreFileAsync(newVectorStoreFile.Value.VectorStoreId, newVectorStoreFile.Value.Id, cancellationToken).ConfigureAwait(false)).Value.Status;
            if (fileStatus == VectorStoreFileStatus.InProgress)
            {
                logger.StillWaitingForFileToBeProcessed();

                if (!fiveMinWarningGiven && (time.GetUtcNow() - ingestStartTime).TotalMinutes > 5)
                {
                    logger.VectorStoreProcessingIsTakingAVERYLongTimeThisMayBeASignOfAProblemFileFileId(newVectorStoreFile.Value.Id);
                    fiveMinWarningGiven = true;
                }
                else if (fiveMinWarningGiven && (time.GetUtcNow() - ingestStartTime).TotalMinutes > 10)
                {
                    logger.VectorStoreProcessingHasTakenOver10MinutesBailingFileFileId(newVectorStoreFile.Value.Id);
                    return null;
                }

                await Task.Delay(TimeSpan.FromSeconds(1), time, cancellationToken).ConfigureAwait(false);
            }
            else if (fileStatus == VectorStoreFileStatus.Completed)
            {
                logger.FileFileIdHasBeenProcessedSuccessfully(newVectorStoreFile.Value.Id);
                return newFile.Value;
            }
            else if (fileStatus == VectorStoreFileStatus.Failed || fileStatus == VectorStoreFileStatus.Cancelled)
            {
                logger.FailedToUplooadMatchSummariesFileFileIdWithStatusStatus(newVectorStoreFile.Value.Id, fileStatus);
                break;
            }
            else
            {
                logger.UnknownFileStatusStatus(fileStatus);
                break;
            }
        }

        return null;
    }
}
