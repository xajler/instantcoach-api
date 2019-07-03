using System.Threading.Tasks;
using Core.Context;
using Core.Contracts;
using Core.Models;
using static Core.Helpers;

namespace Core.Services
{
    public class InstantCoachServices : IInstantCoachService
    {
        private readonly IInstantCoachRepository _repository;

        public InstantCoachServices(IInstantCoachRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<ListResult<InstantCoachList>>> GetList(
            int skip, int take, bool showCompleted)
        {
            var result = await _repository.GetAll(skip, take, showCompleted);
            return SuccessResult(result);
        }

        public async Task<Result<InstantCoach>> GetById(int id)
        {
            if (await _repository.GetExistingId(id) == 0)
            {
                return ErrorResult(ErrorType.UnknownId) as Result<InstantCoach>;
            }
            InstantCoachDb result = await _repository.GetById(id);
            return SuccessResult(result.ToInstantCoach());
        }

        public async Task<Result<int>> Create(InstantCoachCreateClient data)
        {
            // TODO: validation/business logic for create
            //       Checking existence of Agentid or EvaluatorId depending which is loggedin
            //       validation of
            string reference = Helpers.CreateReference();
            InstantCoachStatus status = InstantCoachStatus.New;
            InstantCoachCreate model = data.ToInstantCoachCreate(reference, status);
            // TODO
            var result = await _repository.Add(model);
            return result;
        }

        public async Task<Result> Update(int id, InstantCoachUpdateClient data)
        {
            var existingResult = await GetExistingIdResult(id);
            var status = SetStatus(data.UpdateType);
            var model = data.ToInstantCoachUpate(status);
            if (!existingResult.Success) { return existingResult; }
            var result = await _repository.Update(
                currentEntity: existingResult.Value, model);
            return result;
        }

        public async Task<Result> MarkCompleted(int id)
        {
            var existingResult = await GetExistingIdResult(id);
            if (!existingResult.Success) { return existingResult; }
            var result = await _repository.UpdateAsCompleted(
                currentEntity: existingResult.Value);
            return result;
        }

        public async Task<Result> Remove(int id)
        {
            var existingResult = await GetExistingIdResult(id);
            if (!existingResult.Success) { return existingResult; }
            var result = await _repository.Remove(currentEntity: existingResult.Value);
            return result;
        }

        private async Task<Result<InstantCoachDbEntity>> GetExistingIdResult(int id)
        {
            var result = await _repository.FetchById(id);
            if (result != null) { return SuccessResult(result); }
            return ErrorResult<InstantCoachDbEntity>(ErrorType.UnknownId);
        }
    }
}