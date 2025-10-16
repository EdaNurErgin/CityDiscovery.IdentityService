using SocialService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialService.Infrastructure.HttpClients
{
    public class VenueServiceClient : IVenueServiceClient
    {
        private readonly HttpClient _httpClient;
        public VenueServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> VenueExistsAsync(Guid venueId)
        {
            try
            {
                // Gerçek senaryoda, VenueService'in ilgili endpoint'ini çağırırız.
                // Örneğin: var response = await _httpClient.GetAsync($"/api/venues/{venueId}");
                // return response.IsSuccessStatusCode;

                // ŞİMDİLİK, VenueService hazır olmadığı için, test amacıyla her zaman doğru varsayalım.
                return await Task.FromResult(true);
            }
            catch
            {
                // Hata durumunda mekan yokmuş gibi davranabiliriz.
                return false;
            }
        }
    }
}
