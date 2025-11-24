using EventManagement.Core.Models;
using EventManagement.InfraStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Core.Repositories
{
    public interface ISpeakerRepository
    {
        Task<Speaker> Create(Speaker model);
        Task<Speaker> Get(int id);
        Task<List<Speaker>> GetAll();
        Task Delete(int id);
        Task<Speaker> Update(Speaker model);
        Task<List<Speaker>> GetSpeakersByEventId(int eventId);
    }
}
