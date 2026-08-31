using System.Net.Http.Json;
using MiEcommerce.Application.Contracts.Infrastructure;
using MiEcommerce.Domain.Exceptions;

namespace MiEcommerce.Infrastructure.Services;

/// <summary>
/// Cliente HTTP hacia el microservicio externo PaymentService (proyecto .NET independiente,
/// con su propio puerto — ver PaymentService.WebApi). Se registra como Typed Client vía
/// AddHttpClient en InfrastructureServiceExtensions.
/// </summary>
public class PaymentServiceClient : IPaymentServiceClient
{
    private readonly HttpClient _httpClient;

    public PaymentServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsJsonAsync(
                "api/payments/process",
                new { orderId, amount },
                cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            throw new PaymentServiceUnavailableException("No se pudo contactar al servicio de pagos.", ex);
        }

        if (!response.IsSuccessStatusCode)
            throw new PaymentServiceUnavailableException(
                $"El servicio de pagos respondió con código {(int)response.StatusCode}.");

        var payload = await response.Content.ReadFromJsonAsync<PaymentServiceResponseDto>(cancellationToken: cancellationToken)
            ?? throw new PaymentServiceUnavailableException("Respuesta vacía del servicio de pagos.");

        return new PaymentResult(payload.Status, payload.TransactionId);
    }

    private record PaymentServiceResponseDto(string Status, string TransactionId);
}
