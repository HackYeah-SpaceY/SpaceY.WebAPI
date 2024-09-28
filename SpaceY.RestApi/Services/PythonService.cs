using Flurl;
using Flurl.Http;
using MediatR;
using SpaceY.RestApi.Entities;
using SpaceY.RestApi.Shared;
using SpaceY.RestApi.Shared.Models;

namespace SpaceY.RestApi.Services;

public interface IPythonService 
{ 
    Task<ResponseMessage> SendMessageAsync(string message, Guid chatId);
    Task<NewChatResponse> CreateChatAsync(Guid chatId, string url);
}


public class PythonService : IPythonService
{
    //private readonly string? mainUrl = System.Environment.GetEnvironmentVariable("PythonUrl");
    //private readonly string mainUrl = "https://spacey-processor-production.up.railway.app";
    private readonly string mainUrl = "https://5b72-89-171-58-3.ngrok-free.app";
    public async Task<ResponseMessage> SendMessageAsync(string message, Guid chatId)
    {
        var url = mainUrl + "/message";

        var request = new PythonRequestModel
        {
            id = chatId,
            message = message
        };

        return await url
            .PostJsonAsync(request)
            .ReceiveJson<ResponseMessage>();
    }


    public async Task<NewChatResponse> CreateChatAsync(Guid chatId, string url)
    {
        var newChatUrl = mainUrl + "/new_chat";

        var request = new NewChatRequest
        {
            ChatId = chatId,
            Url = url
        };

        return await newChatUrl
            .PostJsonAsync(request)
            .ReceiveJson<NewChatResponse>();


    }
}
