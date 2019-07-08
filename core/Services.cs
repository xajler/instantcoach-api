using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Core.Repositories;
using Domain;
using Core.Models;
using static Core.Helpers;

namespace Core.Services
{
    public interface IInstantCoachService
    {
        Task<ListResult<InstantCoachList>> GetList(int skip, int take, bool showCompleted);
        Task<Result<InstantCoachForId>> GetById(int id);
        Task<Result<InstantCoach>> Create(InstantCoachCreateClient data);
        Task<Result> Update(int id, InstantCoachUpdateClient data);
        Task<Result> MarkCompleted(int id);
        Task<Result> Remove(int id);
    }

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
            // TODO: Checking existence of Agentid or EvaluatorId depending which is loggedin
            //       validation of bookmarkId in comments
            InstantCoach entity = data.ToNewInstantCoach();
            var validationResult = entity.Validate();
            _logger.LogInformation("Entity after validate:\n{EntityModel}", ToLogJson(entity));
            return await OnSave(entity, validationResult);
        }

        public async Task<Result> Update(int id, InstantCoachUpdateClient data)
        {
            // TODO: Guard those with status completed, they cannot be updated.
            var entityResult = await GetExistingIdResult(id);
            if (!entityResult.Success) { return OnNotExistingId(id); }
            var entity = entityResult.Value;
            ValidationResult validationResult = entity.UpdateAndValidate(
                data.UpdateType,
                data.Comments.ToComments(),
                data.BookmarkPins.ToBookmarkPins());
            return await OnSave(entity, validationResult);
        }

        public async Task<Result> MarkCompleted(int id)
        {
            var entityResult = await GetExistingIdResult(id);
            if (!entityResult.Success) { return OnNotExistingId(id); }
            var entity = entityResult.Value;
            ValidationResult validationResult = entity.UpdateAsCompletedAndValidate();
            return await OnSave(entity, validationResult);
        }

        public async Task<Result> Remove(int id)
        {
            var entityResult = await GetExistingIdResult(id);
            if (!entityResult.Success) { return OnNotExistingId(id); }
            var result = await _repository.Delete(entity: entityResult.Value);
            return result;
        }

        private async Task<Result<InstantCoach>> OnSave(InstantCoach entity, ValidationResult validationResult)
        {
            Result vResult;

            if (validationResult.IsValid)
                vResult = Result.AsSuccess();
            else
                vResult = Result.AsDomainError(validationResult.Errors);

            Result<InstantCoach> result;

            if (vResult.Success)
            {
                _logger.LogInformation("Entity on Save:\n{EntityModel}", ToLogJson(entity));
                result = await _repository.Save(entity);
            }
            else
                result = Result<InstantCoach>.AsDomainError(validationResult.Errors);

            _logger.LogInformation("Create Result:\n{Result}", ToLogJson(result));
            return result;
        }

        private async Task<Result<InstantCoach>> GetExistingIdResult(int id)
        {
            var result = await _repository.FindById(id);
            if (result != null) { return Result<InstantCoach>.AsSuccess(result); }
            return Result<InstantCoach>.AsError(ErrorType.UnknownId);
        }

        private Result<InstantCoach> OnNotExistingId(int id)
        {
            _logger.LogError("Service Error: Not existing Id: {Id}", id);
            return Result<InstantCoach>.AsError(ErrorType.UnknownId);
        }
    }
}