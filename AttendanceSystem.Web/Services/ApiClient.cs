using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AttendanceSystem.Web.Services
{
    public interface IApiClient
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
        Task<TResponse?> PostMultipartAsync<TResponse>(string endpoint, MultipartFormDataContent content);
        Task<bool> PutAsync<TRequest>(string endpoint, TRequest data);
        Task<bool> DeleteAsync(string endpoint);
        Task<HttpResponseMessage> DeleteRawAsync(string endpoint);
        Task<HttpResponseMessage> PostRawAsync<TRequest>(string endpoint, TRequest data);
        Task<HttpResponseMessage> PutRawAsync<TRequest>(string endpoint, TRequest data);
    }

    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            return default;
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            return default;
        }

        public async Task<HttpResponseMessage> PostRawAsync<TRequest>(string endpoint, TRequest data)
        {
            return await _httpClient.PostAsJsonAsync(endpoint, data);
        }

        public async Task<TResponse?> PostMultipartAsync<TResponse>(string endpoint, MultipartFormDataContent content)
        {
            var response = await _httpClient.PostAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            return default;
        }

        public async Task<bool> PutAsync<TRequest>(string endpoint, TRequest data)
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            return response.IsSuccessStatusCode;
        }

        public async Task<HttpResponseMessage> PutRawAsync<TRequest>(string endpoint, TRequest data)
        {
            return await _httpClient.PutAsJsonAsync(endpoint, data);
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }

        public async Task<HttpResponseMessage> DeleteRawAsync(string endpoint)
        {
            return await _httpClient.DeleteAsync(endpoint);
        }
    }
}
