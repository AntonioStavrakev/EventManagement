using EventManagement.Core.Models;
using EventManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Core.Repositories
{
    public interface IEventManagementRepository
    {
        Task<Event> Create(Event model);
        Task<Event> Get(int id);
        Task<List<Event>> GetAll();   
        Task Delete(int id);
        Task<Event> Update(Event model);
        Task<List<Event>> GetEventsByUser(int userId, bool upcomingOnly);
        Task<List<User>> GetUsersByEvent(int eventId);
        Task<List<Event>> GetUpcomingEventsWithSpeakers();

    }
}
