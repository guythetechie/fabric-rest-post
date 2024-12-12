using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Collections.Frozen;
using System;

namespace functionapp;

public static class HttpPostTrigger
{
    [Function("HttpPostTrigger")]
    public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request)
    {
        var responseJson = new JsonObject
        {
            ["message"] = "Welcome to Azure Functions!"
        };

        foreach (var header in request.GetHeaders())
        {
            responseJson[header.Key] = header.Value.ToString();
        }

        if (request.TryGetContent(out var content))
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

    public static bool TryGetContent(this HttpRequest request, out JsonNode? content)
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