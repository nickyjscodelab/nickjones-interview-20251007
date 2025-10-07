using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Demo.Workflow.Domain;

namespace Workflow.Wpf.Services;

public interface IApiClient
{
    Task<IReadOnlyList<ProjectRequest>> GetRequestsAsync(string? status = null, string? q = null, CancellationToken ct = default);
    Task<ProjectRequest?> GetRequestAsync(int id, CancellationToken ct = default);
    Task<ProjectRequest> CreateRequestAsync(CreateProjectRequestDto request, CancellationToken ct = default);
    Task AddSignOffAsync(int requestId, CreateSignOffDto signOff, CancellationToken ct = default);
}

public class ApiClient : IApiClient
{
    private readonly HttpClient _http;

    public static Uri DefaultBaseAddress { get; } = new("http://localhost:5100");

    public ApiClient(HttpClient? httpClient = null)
    {
        _http = httpClient ?? new HttpClient { BaseAddress = DefaultBaseAddress, Timeout = TimeSpan.FromSeconds(10) };
    }

    public async Task<IReadOnlyList<ProjectRequest>> GetRequestsAsync(string? status = null, string? q = null, CancellationToken ct = default)
    {
        var url = "/api/requests";
        List<string> qs = new();
        if (!string.IsNullOrWhiteSpace(status)) qs.Add($"status={Uri.EscapeDataString(status)}");
        if (!string.IsNullOrWhiteSpace(q)) qs.Add($"q={Uri.EscapeDataString(q)}");
        if (qs.Count > 0) url += "?" + string.Join("&", qs);
        return await _http.GetFromJsonAsync<List<ProjectRequest>>(url, ct) ?? new List<ProjectRequest>();
    }

    public Task<ProjectRequest?> GetRequestAsync(int id, CancellationToken ct = default)
        => _http.GetFromJsonAsync<ProjectRequest>($"/api/requests/{id}", ct);

    public async Task<ProjectRequest> CreateRequestAsync(CreateProjectRequestDto request, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("/api/requests", request, ct);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<ProjectRequest>(cancellationToken: ct))!;
    }

    public async Task AddSignOffAsync(int requestId, CreateSignOffDto signOff, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync($"/api/requests/{requestId}/signoffs", signOff, ct);
        resp.EnsureSuccessStatusCode();
    }
}
