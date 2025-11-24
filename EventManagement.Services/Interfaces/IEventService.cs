using EventManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Services.Interfaces
{
    public interface IEventService
    {
        Task<Event> Create(Event model);
        Task<Event> GetEvent(int id);
        Task<List<Event>> GetAll();
        Task Delete(int id);
        Task<Event> Update(Event model);

    }
}
