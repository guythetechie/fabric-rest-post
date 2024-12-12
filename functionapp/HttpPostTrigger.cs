using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace functionapp;

public static class HttpPostTrigger
{
    [Function("HttpPostTrigger")]
    public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest request)
    {
        var responseJson = new JsonObject
        {
            ["message"] = "Welcome to Azure Functions!"
        };

        foreach (var header in request.Headers)
        {
            responseJson[header.Key] = header.Value.ToString();
        }

        if (TryGetContent(request, out var content))
        {
            responseJson["content"] = content;
        }

        return new OkObjectResult(responseJson);
    }

    private static bool TryGetContent(HttpRequest request, out JsonNode? content)
    {
        try
        {
            content = JsonNode.Parse(request.Body);

            return content is not null;
        }
        catch (JsonException)
        {
            content = null;
            return false;
        }
    }
}
