using Microsoft.EntityFrameworkCore;
using StreamingOffApp.Data;
using StreamingOffApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamingOffApp.Services
{
    public class StreamingService : IStreamingService
    {
        private readonly StreamingContext _context;

        public StreamingService(StreamingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<StreamingOffer>> GetAllOffers()
        {
            try
            {
                return await _context.StreamingOffers.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas pobierania ofert", ex);
            }
        }

        public async Task AddOffer(StreamingOffer offer)
        {
            try
            {
                _context.StreamingOffers.Add(offer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas dodawania oferty: " + ex.Message);
            }
        }

        public async Task UpdateOffer(StreamingOffer offer)
        {
            try
            {
                _context.StreamingOffers.Update(offer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas aktualizacji oferty", ex);
            }
        }

        public async Task DeleteOffer(int id)
        {
            try
            {
                var offer = await _context.StreamingOffers.FindAsync(id);
                if (offer != null)
                {
                    _context.StreamingOffers.Remove(offer);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas usuwania oferty", ex);
            }
        }
    }
}