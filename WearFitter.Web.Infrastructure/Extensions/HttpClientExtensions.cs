using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using WearFitter.Web.Infrastructure.Exceptions;
using WearFitter.Web.Infrastructure.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace WearFitter.Web.Infrastructure.Extensions;

public static class HttpClientExtensions
{
    private static JsonSerializerSettings _settings;

#pragma warning disable S3963 // "static" fields should be initialized inline
    static HttpClientExtensions()
    {
        _settings = new JsonSerializerSettings();
        JsonSettingsConfigurator.Configure(_settings);
    }
#pragma warning restore S3963 // "static" fields should be initialized inline

    public static async Task<TValue> GetFromNJsonAsync<TValue>(this HttpClient client, string requestUri, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(Application.Json));

        HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return default;

        await ThrowIfApiError(response);

        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentType.MediaType != Application.Json)
        {
            throw new HttpRequestException($"Response content type is not json: {response.Content.Headers.ContentType.MediaType}", null, System.Net.HttpStatusCode.UnsupportedMediaType);
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ApiException("Response body is empty.", response.StatusCode);
        }

        TValue result = JsonConvert.DeserializeObject<TValue>(json, _settings);

        if (result == null)
        {
            throw new ApiException($"Response is not of type {typeof(TValue).FullName}", response.StatusCode);
        }

        return result;
    }

    public static async Task<byte[]> GetFromMessagePackUnpackedAsync(this HttpClient client, string requestUri, CancellationToken cancellationToken = default, bool useCompression = false)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/x-msgpack"));

        if (useCompression)
        {
            request.Headers.Add("x-compression", "msgpack");
        }

        HttpResponseMessage response = await client.SendAsync(request);

        await ThrowIfApiError(response);

        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentType.MediaType != "application/x-msgpack")
        {
            throw new HttpRequestException($"Response content type is not msgpack: {response.Content.Headers.ContentType.MediaType}", null, System.Net.HttpStatusCode.UnsupportedMediaType);
        }

        var result = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        return result;
    }

    public static async Task<HttpResponseMessage> PostAsNJsonAsync<TValue>(this HttpClient client, string requestUri, TValue value, CancellationToken cancellationToken = default)
    {
        var json = JsonConvert.SerializeObject(value, typeof(TValue), _settings);
        var content = new StringContent(json, Encoding.UTF8, Application.Json);

        var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(Application.Json));
        request.Content = content;

        HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

        await ThrowIfApiError(response);

        return response;
    }

    public static async Task<HttpResponseMessage> DeleteAsNJsonAsync<TValue>(this HttpClient client, string requestUri, TValue value, CancellationToken cancellationToken = default)
    {
        var json = JsonConvert.SerializeObject(value, typeof(TValue), _settings);
        var content = new StringContent(json, Encoding.UTF8, Application.Json);

        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(Application.Json));
        request.Content = content;

        HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

        await ThrowIfApiError(response);

        return response;
    }

    public static async Task<HttpResponseMessage> PutAsNJsonAsync<TValue>(this HttpClient client, string requestUri, TValue value, CancellationToken cancellationToken = default)
    {
        var json = JsonConvert.SerializeObject(value, typeof(TValue), _settings);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Put, requestUri);
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        request.Content = content;

        HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

        await ThrowIfApiError(response);

        return response;
    }

    public static async Task<HttpResponseMessage> PatchAsNJsonAsync<TValue>(this HttpClient client, string requestUri, TValue value, CancellationToken cancellationToken = default)
    {
        var json = JsonConvert.SerializeObject(value, typeof(TValue), _settings);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Patch, requestUri);
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        request.Content = content;

        HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

        await ThrowIfApiError(response);

        return response;
    }

    private static async ValueTask ThrowIfApiError(HttpResponseMessage response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError &&
            response.Content.Headers.ContentType.MediaType == Application.Json)
        {
            string errorJson = await response.Content.ReadAsStringAsync();

            var headers = GetResponseHeaders(response);

            throw new ApiException("Internal server error", (int)System.Net.HttpStatusCode.InternalServerError, errorJson, headers, null);
        }
    }

    public static IReadOnlyDictionary<string, IEnumerable<string>> GetResponseHeaders(HttpResponseMessage response)
    {
        var headers = response.Headers.ToDictionary(h => h.Key, h => h.Value);

        if (response.Content is not null && response.Content.Headers is not null)
        {
            foreach (var item in response.Content.Headers)
                headers[item.Key] = item.Value;
        }

        return headers;
    }
}
