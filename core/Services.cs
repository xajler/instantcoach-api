using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Core.Repositories;
using Core.Contracts;
using Core.Models;
using static Core.Helpers;

namespace Core.Services
{
    public class InstantCoachService : IInstantCoachService
    {
        private readonly ILogger _logger;
        private readonly Config _config;
        private readonly InstantCoachRepository _repository;

        public InstantCoachService(
            ILogger<InstantCoachService> logger,
            IOptions<Config> configOptions,
            InstantCoachRepository repository)
        {
            _logger = logger;
            _config = configOptions.Value;
            _repository = repository;

        }

        public async Task<ListResult<InstantCoachList>> GetList(
            int skip, int take, bool showCompleted)
        {
            var result = await _repository.GetAll(skip, take, showCompleted);
            _logger.LogInformation("Get List Result:\n{Result}", ToLogJson(result));
            return result;
        }

        public async Task<Result<InstantCoachForId>> GetById(int id)
        {
            if (await _repository.GetExistingId(id) == 0)
            {
                return Result<InstantCoachForId>.AsError(ErrorType.UnknownId);
            }
            Result<InstantCoachDb> result = await _repository.GetById(id);
            _logger.LogInformation("Get List Result:\n{Result}", ToLogJson(result));
            if (!result.Success) { return Result<InstantCoachForId>.AsError(result.Error); }
            return Result<InstantCoachForId>.AsSuccess(result.Value.ToInstantCoachForId());
        }

        public async Task<Result<InstantCoach>> Create(InstantCoachCreateClient data)
        {
            // TODO: validation/business logic for create
            //       Checking existence of Agentid or EvaluatorId depending which is loggedin
            //       validation of
            InstantCoach entity = data.ToInstantCoach();
            var result = await _repository.Save(entity);
            _logger.LogInformation("Create Result:\n{Result}", ToLogJson(result));
            return result;
        }

        // public async Task<Result> Update(int id, InstantCoachUpdateClient data)
        // {
        //     var commentsWithCount = GetCommentsWithCount(model.Comments);
        //     var updatedEntity = model.ToInstantCoachDbEntity(
        //         currentState: currentEntity, commentsWithCount);
        //     _logger.LogInformation("Update Entity Model:\n{EntityModel}", ToLogJson(updatedEntity));
        //     var existingResult = await GetExistingIdResult(id);
        //     if (!existingResult.Success) { return OnNotExistingId(id, existingResult); }
        //     var status = SetStatus(data.UpdateType);
        //     var model = data.ToInstantCoachUpate(status);
        //     _logger.LogInformation("Update Domain Model:\n{DomainModel}", ToLogJson(model));
        //     var result = await _repository.Update(
        //         currentEntity: existingResult.Value, model);
        //     return result;
        // }

        // public async Task<Result> MarkCompleted(int id)
        // {
        //     currentEntity.Status = InstantCoachStatus.Completed;
        //     _logger.LogInformation("Update Completed Entity Model:\n{EntityModel}", ToLogJson(currentEntity));
        //     var existingResult = await GetExistingIdResult(id);
        //     if (!existingResult.Success) { return OnNotExistingId(id, existingResult); }
        //     var result = await _repository.UpdateAsCompleted(
        //         currentEntity: existingResult.Value);
        //     return result;
        // }

        // public async Task<Result> Remove(int id)
        // {
        //     var existingResult = await GetExistingIdResult(id);
        //     if (!existingResult.Success) { return OnNotExistingId(id, existingResult); }
        //     var result = await _repository.Remove(currentEntity: existingResult.Value);
        //     return result;
        // }

        // private async Task<Result<InstantCoachDbEntity>> GetExistingIdResult(int id)
        // {
        //     var result = await _repository.FetchById(id);
        //     if (result != null) { return SuccessResult(result); }
        //     return ErrorResult<InstantCoachDbEntity>(ErrorType.UnknownId);
        // }

        // private Result OnNotExistingId(int id, Result result)
        // {
        //     _logger.LogError("Service Error: Not existing Id: {Id}", id);
        //     return result;
        // }
    }
}