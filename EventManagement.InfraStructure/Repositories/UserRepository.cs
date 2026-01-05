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
    public class UserRepository : IUserRepository
    {
        private readonly EventDbContext _context;

        public UserRepository(EventDbContext context)
        {
            _context = context;
        }

        

        public async Task<User> Create(User model)
        {
            await _context.Users.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<User> Get(int id)
        {
            return await _context.Users
                .Include(u => u.Registrations)
                .ThenInclude(r => r.Event)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users
                .Include(u => u.Registrations)
                .ThenInclude(r => r.Event)
                .ToListAsync();
        }

        public async Task<User> Update(User model)
        {
            _context.Users.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task Delete(int id)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity != null)
            {
                _context.Users.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<List<User>> GetUsersByEventId(int eventId)
        {
            return await _context.Users
                .Include(u => u.Registrations)
                .ThenInclude(r => r.Event)
                .Where(u => u.Registrations.Any(r => r.EventId == eventId))
                .ToListAsync();
        }



        public async Task<List<User>> GetUsersByRegistrationStatus(bool isRegistered)
        {
            if (isRegistered)
            {
                return await _context.Registrations
                    .Where(r => r != null) 
                    .Select(r => r.User)
                    .Distinct()
                    .Include(u => u.Registrations)
                    .ThenInclude(r => r.Event)
                    .ToListAsync();
            }
            else
            {
                
                return await _context.Users
                    .Where(u => !u.Registrations.Any())
                    .ToListAsync();
            }
        }
    }
}
