using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Collections.Frozen;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace functionapp;

public class HttpPostTrigger(ILogger<HttpPostTrigger> logger)
{
    [Function("HttpPostTrigger")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await HandleRequest(request, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, exception.Message);
            throw;
        }
    }

    private static async ValueTask<IActionResult> HandleRequest(HttpRequest request, CancellationToken cancellationToken)
    {
        var responseJson = new JsonObject
        {
            ["message"] = "Welcome to Azure Functions!"
        };

        foreach (var header in request.GetHeaders())
        {
            responseJson[header.Key] = header.Value.ToString();
        }

        if (await request.TryGetContent(cancellationToken) is JsonNode content)
        {
            responseJson["content"] = content;
        }

        return new OkObjectResult(responseJson);
    }
}

file static class HttpRequestModule
{
    public static FrozenDictionary<string, string> GetHeaders(this HttpRequest? request)
    {
        var dictionary = request?.Headers ?? new HeaderDictionary();

        return dictionary.ToFrozenDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString(), StringComparer.OrdinalIgnoreCase);
    }

    public static async ValueTask<JsonNode?> TryGetContent(this HttpRequest? request, CancellationToken cancellationToken)
    {
        try
        {
            return request?.Body switch
            {
                null => null,
                var body => await JsonNode.ParseAsync(body, cancellationToken: cancellationToken)
            };
        }
        catch (JsonException)
        {
            return null;
        }
    }
}