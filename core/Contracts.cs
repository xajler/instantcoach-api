using System.Threading.Tasks;
using Core.Models;

namespace Core.Contracts
{
    public interface IInstantCoachRepository
    {
        Task<ListResult<InstantCoachList>> GetAll(int skip, int take, bool showCompleted);
        Task<InstantCoachDb> GetById(int id);
        Task<int> GetExistingId(int id);
        Task Create(InstantCoachCreate model);
        Task Update(int id, InstantCoachUpdate model);
        Task UpdateAsCompleted(int id);
        Task Delete(int id);
    }

    public interface IInstantCoachService
    {
        Task<ListResult<InstantCoachList>> GetList(int skip, int take, bool showCompleted);
        Task<InstantCoach> GetById(int id);
        Task Create(InstantCoachCreateClient data);
        Task Update(int id, InstantCoachUpdateClient data);
        Task MarkCompleted(int id);
        Task Delete(int id);
    }
}