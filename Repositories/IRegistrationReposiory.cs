using EventManagement.Core.Models;
using EventManagement.InfraStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Core.Repositories
{
    public interface IRegistrationReposiory
    {
        Task<Registration> Create(Registration model);
        Task<Registration> Get(int id);
        Task<List<Registration>> GetAll();
        Task Delete(int id);
        Task<Registration> Update(Registration model);
        Task<List<Registration>> GetRegistrationsByEventId(int eventId);
        Task<List<Registration>> GetRegistrationsByUserId(int userId);
        Task<List<Registration>> GetRegistrationsByStatus(bool isRegistered);
    }
}
