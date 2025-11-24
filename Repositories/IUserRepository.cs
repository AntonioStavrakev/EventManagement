using EventManagement.Core.Models;
using EventManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Core.Repositories
{
    public interface IUserRepository
    {
        Task<User> Create(User model);
        Task<User> Get(int id);
        Task<List<User>> GetAll();
        Task Delete(int id);
        Task<User> Update(User model);
        Task<List<User>> GetUsersByEventId(int eventId);
        Task<List<User>> GetUsersByRegistrationStatus(bool isRegistered);
    }
}
