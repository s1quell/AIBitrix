using System.Net.Http.Json;
using OpenAI.Models;

namespace OpenAI;

public class OpenAIService
{

    #region Private Members

    private readonly string _apiKey;

    private readonly HttpClient _client;

    private const string Endpoint = "https://api.openai.com/v1/chat/completions";

    #endregion

    #region Constructor

    /// <summary>
    /// Создание сервиса, который будет работать с API от OpenAI
    /// </summary>
    /// <param name="apiKey"></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="Exception"></exception>
    public OpenAIService(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
            throw new NullReferenceException("Key is invalid: " + apiKey);
        _apiKey = apiKey;
        
        try
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }
        catch (Exception e)
        {
            throw new Exception("Error of creating OpenAIService, check internet connection");
        }
    }

    #endregion

    #region Public Methods

    public async Task<ResponseData?> GetCompletions(string content)
    {
        // формируем отправляемые данные
        var requestData = new Request() 
        {
            ModelId = "gpt-3.5-turbo",
            Messages = new(),
        };
        
        requestData.Messages.Add(new Message(){Role = "user", Content = content});
        
        // отправляем запрос
        using var response = await _client.PostAsJsonAsync(Endpoint, requestData);
 
        // если произошла ошибка, выбрасываем ошибку
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"{(int)response.StatusCode} {response.StatusCode}");
        }
        // получаем данные ответа
        var responseData = await response.Content.ReadFromJsonAsync<ResponseData>();
 
        var choices = responseData?.Choices ?? new List<Choice>();
        if (choices.Count == 0)
        {
            throw new Exception("No choices were returned by the API");
        }

        return responseData;
    }

    #endregion
}