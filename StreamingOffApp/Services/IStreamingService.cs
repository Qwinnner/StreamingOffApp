using StreamingOffApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StreamingOffApp.Services
{
    public interface IStreamingService
    {
        Task AddOffer(StreamingOffer offer);
        Task UpdateOffer(StreamingOffer offer);
        Task DeleteOffer(int id);
        Task<IEnumerable<StreamingOffer>> GetAllOffers();
    }
}