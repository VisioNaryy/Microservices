using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlatformService.SyncDataServices.HTTP
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
        {
            var httpContent = new StringContent(
             JsonSerializer.Serialize(platformReadDto),
             Encoding.UTF8,
             "application/json"
             );

            var response = await _httpClient.PostAsync($"{_configuration["CommandServiceUrl"]}/TestInboundConnection", httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"--> Sync POST to CommandService was OK! Content: {response.Content}");
            }
            else
            {
                Console.WriteLine($"--> Sync POST to CommandService was NOT OK! Content: {response.Content}");
            }
        }
    }
}