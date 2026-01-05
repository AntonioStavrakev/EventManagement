using EventManagement.Core.Models;
using EventManagement.Core.Repositories;
using EventManagement.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.InfraStructure.Repositories
{
    public class EventRepository : IEventManagementRepository
    {
        private readonly EventDbContext _context;

        public EventRepository(EventDbContext context)
        {
            _context = context;
        }

        

        public async Task<Event> Create(Event model)
        {
            await _context.Events.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<Event> Get(int id)
        {
            return await _context.Events
                .Include(e => e.Registrations)       
                .ThenInclude(r => r.User)            
                .FirstOrDefaultAsync(e => e.EventId == id);
        }

        public async Task<List<Event>> GetAll()
        {
            return await _context.Events
                .Include(e => e.Speaker)             
                .ToListAsync();
        }

        public async Task<Event> Update(Event model)
        {
            _context.Events.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task Delete(int id)
        {
            var entity = await _context.Events.FindAsync(id);
            if (entity != null)
            {
                _context.Events.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }




        public async Task<List<Event>> GetEventsByUser(int userId, bool upcomingOnly)
        {
                        var query = _context.Events
                .Include(e => e.Speaker)               
                .Include(e => e.Registrations)       
                .ThenInclude(r => r.User)             
                .Where(e => e.Registrations.Any(r => r.UserId == userId)); 

            if (upcomingOnly)
                query = query.Where(e => e.Date > DateTime.Now);
            else
                query = query.Where(e => e.Date <= DateTime.Now);

            return await query.ToListAsync();
        }




        public async Task<List<User>> GetUsersByEvent(int eventId)
        {
            return await _context.Registrations
                .Where(r => r.EventId == eventId)
                .Select(r => r.User)
                .ToListAsync();
        }

        
        public async Task<List<Event>> GetUpcomingEventsWithSpeakers()
        {
            return await _context.Events
                .Where(e => e.Date > DateTime.Now)
                .Include(e => e.Speaker)
                .ToListAsync();
        }
    }
}
