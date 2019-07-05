using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Core.Context;
using Core.Contracts;
using Core.Models;
using static Core.Helpers;

namespace Core.Services
{
    public class InstantCoachService : IInstantCoachService
    {
        private readonly ILogger _logger;
        private readonly Config _config;
        private readonly IInstantCoachRepository _repository;

        public InstantCoachService(
            ILogger<InstantCoachService> logger,
            IOptions<Config> configOptions,
            IInstantCoachRepository repository)
        {
            _logger = logger;
            _config = configOptions.Value;
            _repository = repository;
        }

        public async Task<ListResult<InstantCoachList>> GetList(
            int skip, int take, bool showCompleted)
        {
            var result = await _repository.GetAll(skip, take, showCompleted);
            _logger.LogInformation($"Get List Result:\n{ToLogJson(result)}");
            return result;
        }

        public async Task<Result<InstantCoach>> GetById(int id)
        {
            if (await _repository.GetExistingId(id) == 0)
            {
                return ErrorResult<InstantCoach>(ErrorType.UnknownId);
            }
            Result<InstantCoachDb> result = await _repository.GetById(id);
            _logger.LogInformation($"Get List Result:\n{ToLogJson(result)}");
            if (!result.Success) { return ErrorResult<InstantCoach>(result.Error); }
            return SuccessResult(result.Value.ToInstantCoach());
        }

        public async Task<Result<int>> Create(InstantCoachCreateClient data)
        {
            // TODO: validation/business logic for create
            //       Checking existence of Agentid or EvaluatorId depending which is loggedin
            //       validation of
            string reference = CreateReference();
            InstantCoachStatus status = _config.InstantCoachStatusDefault;
            InstantCoachCreate model = data.ToInstantCoachCreate(reference, status);
            _logger.LogInformation($"Create Domain Model:\n{ToLogJson(model)}");
            var result = await _repository.Add(model);
            _logger.LogInformation($"Create Result:\n{ToLogJson(result)}");
            return result;
        }

        public async Task<Result> Update(int id, InstantCoachUpdateClient data)
        {
            var existingResult = await GetExistingIdResult(id);
            if (!existingResult.Success) { return OnNotExistingId(id, existingResult); }
            var status = SetStatus(data.UpdateType);
            var model = data.ToInstantCoachUpate(status);
            _logger.LogInformation($"Update Domain Model:\n{ToLogJson(model)}");
            var result = await _repository.Update(
                currentEntity: existingResult.Value, model);
            return result;
        }

        public async Task<Result> MarkCompleted(int id)
        {
            var existingResult = await GetExistingIdResult(id);
            if (!existingResult.Success) { return OnNotExistingId(id, existingResult); }
            var result = await _repository.UpdateAsCompleted(
                currentEntity: existingResult.Value);
            return result;
        }

        public async Task<Result> Remove(int id)
        {
            var existingResult = await GetExistingIdResult(id);
            if (!existingResult.Success) { return OnNotExistingId(id, existingResult); }
            var result = await _repository.Remove(currentEntity: existingResult.Value);
            return result;
        }

        private async Task<Result<InstantCoachDbEntity>> GetExistingIdResult(int id)
        {
            var result = await _repository.FetchById(id);
            if (result != null) { return SuccessResult(result); }
            return ErrorResult<InstantCoachDbEntity>(ErrorType.UnknownId);
        }

        private Result OnNotExistingId(int id, Result result)
        {
            _logger.LogError($"Service Error: Not existing Id: {id}");
            return result;
        }
    }
}