using EventManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.InfraStructure.Repositories
{
    public class RegistrationRepository : IRegistrationReposiory
    {
        private readonly EventDbContext _context;

        public RegistrationRepository(EventDbContext context)
        {
            _context = context;
        }

        

        public async Task<Registration> Create(Registration model)
        {
            await _context.Registrations.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<Registration> Get(int id)
        {
            return await _context.Registrations
                .Include(r => r.User)
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.RegistrationId == id);
        }

        public async Task<List<Registration>> GetAll()
        {
            return await _context.Registrations
                .Include(r => r.User)
                .Include(r => r.Event)
                .ToListAsync();
        }

        public async Task<Registration> Update(Registration model)
        {
            _context.Registrations.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task Delete(int id)
        {
            var entity = await _context.Registrations.FindAsync(id);
            if (entity != null)
            {
                _context.Registrations.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Registration>> GetRegistrationsByEventId(int eventId)
        {
            return await _context.Registrations
                .Where(r => r.EventId == eventId)
                .Include(r => r.User)
                .Include(r => r.Event)
                .ToListAsync();
        }


        public async Task<List<Registration>> GetRegistrationsByUserId(int userId)
        {
            return await _context.Registrations
                .Where(r => r.UserId == userId)
                .Include(r => r.User)
                .Include(r => r.Event)
                .ToListAsync();
        }

        
        public async Task<List<Registration>> GetRegistrationsByStatus(bool isRegistered)
        {
            if (isRegistered)
                return await _context.Registrations
                    .Include(r => r.User)
                    .Include(r => r.Event)
                    .ToListAsync();
            else
                return new List<Registration>(); 
        }
    }
}
