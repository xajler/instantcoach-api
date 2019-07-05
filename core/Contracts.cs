using System.Threading.Tasks;
using Core.Models;
using Core.Context;

namespace Core.Contracts
{
    public interface IInstantCoachRepository
    {
        Task<InstantCoachDbEntity> FetchById(int id);
        Task<ListResult<InstantCoachList>> GetAll(int skip, int take, bool showCompleted);
        Task<Result<InstantCoachDb>> GetById(int id);
        Task<int> GetExistingId(int id);
        Task<Result<int>> Add(InstantCoachCreate model);
        Task<Result> Update(InstantCoachDbEntity currentEntity,
            InstantCoachUpdate model);
        Task<Result> UpdateAsCompleted(InstantCoachDbEntity currentEntity);
        Task<Result> Remove(InstantCoachDbEntity currentEntity);
    }

    public interface IInstantCoachService
    {
        Task<ListResult<InstantCoachList>> GetList(int skip, int take, bool showCompleted);
        Task<Result<InstantCoach>> GetById(int id);
        Task<Result<int>> Create(InstantCoachCreateClient data);
        Task<Result> Update(int id, InstantCoachUpdateClient data);
        Task<Result> MarkCompleted(int id);
        Task<Result> Remove(int id);
    }
}