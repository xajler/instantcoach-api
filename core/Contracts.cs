using System.Threading.Tasks;
using Core.Domain;
using Core.Models;

namespace Core.Contracts
{
    public interface IInstantCoachService
    {
        Task<ListResult<InstantCoachList>> GetList(int skip, int take, bool showCompleted);
        Task<Result<InstantCoachForId>> GetById(int id);
        Task<Result<InstantCoach>> Create(InstantCoachCreateClient data);
        // Task<Result> Update(int id, InstantCoachUpdateClient data);
        // Task<Result> MarkCompleted(int id);
        // Task<Result> Remove(int id);
    }
}