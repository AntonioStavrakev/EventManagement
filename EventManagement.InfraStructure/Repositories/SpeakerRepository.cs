using EventManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.InfraStructure.Repositories
{
    public class SpeakerRepository : ISpeakerRepository
    {
        private readonly EventDbContext _context;

        public SpeakerRepository(EventDbContext context)
        {
            _context = context;
        }

        

        public async Task<Speaker> Create(Speaker model)
        {
            await _context.Speakers.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<Speaker> Get(int id)
        {
            return await _context.Speakers
                .Include(s => s.Events)
                .FirstOrDefaultAsync(s => s.SpeakerID == id);
        }

        public async Task<List<Speaker>> GetAll()
        {
            return await _context.Speakers
                .Include(s => s.Events)
                .ToListAsync();
        }

        public async Task<Speaker> Update(Speaker model)
        {
            _context.Speakers.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task Delete(int id)
        {
            var entity = await _context.Speakers.FindAsync(id);
            if (entity != null)
            {
                _context.Speakers.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        
        public async Task<List<Speaker>> GetSpeakersByEventId(int eventId)
        {
            return await _context.Speakers
                .Where(s => s.Events.Any(e => e.EventId == eventId))
                .Include(s => s.Events)
                .ToListAsync();
        }
    }
}
