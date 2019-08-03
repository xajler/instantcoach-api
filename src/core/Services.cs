using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema;
using Core.Repositories;
using Domain;
using Core.Models;
using ErrorType = Core.Models.ErrorType;
using static Core.Helpers;
using static Core.Constants.Controller;

namespace Core.Services
{
    public interface IInstantCoachService
    {
        Task<ListResult<InstantCoachList>> GetList(int skip, int take, bool showCompleted);
        Task<Result<InstantCoachForId>> GetById(int id);
        Result<JSchema> GetJsonSchema(string schemaType);
        Task<Result<InstantCoach>> Create(InstantCoachCreateClient data);
        Task<Result> Update(int id, InstantCoachUpdateClient data);
        Task<Result> MarkCompleted(int id);
        Task<Result> Remove(int id);
    }

    public sealed class InstantCoachService : IInstantCoachService
    {
        private readonly ILogger _logger;
        private readonly InstantCoachRepository _repository;

        public InstantCoachService(
            ILogger<InstantCoachService> logger,
            InstantCoachRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<ListResult<InstantCoachList>> GetList(
            int skip, int take, bool showCompleted)
        {
            var result = await _repository.GetAll(skip, take, showCompleted);
            _logger.LogInformation("Get List [Result]: {@Result}", result);
            return result;
        }

        public async Task<Result<InstantCoachForId>> GetById(int id)
        {
            if (await _repository.GetExistingId(id) == 0)
            {
                return Result<InstantCoachForId>.AsError(ErrorType.UnknownId);
            }
            Result<InstantCoachDb> result = await _repository.GetById(id);
            _logger.LogInformation("Get By Id [Result]: {@Result}", result);
            if (!result.Success) { return Result<InstantCoachForId>.AsError(result.Error); }
            return Result<InstantCoachForId>.AsSuccess(result.Value.ToInstantCoachForId());
        }

        public Result<JSchema> GetJsonSchema(string schemaType)
        {
            if (schemaType == SchemaCreate || schemaType == SchemaUpdate)
            {
                _logger.LogInformation("GET schema by type: {@SchemaType}", schemaType);
                var generator = new JSchemaGenerator
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                generator.GenerationProviders.Add(new StringEnumGenerationProvider());

                if (schemaType == SchemaCreate)
                {
                    return Result<JSchema>.AsSuccess(
                    generator.Generate(typeof(InstantCoachCreateClient)));
                }
                else
                {
                    return Result<JSchema>.AsSuccess(
                        generator.Generate(typeof(InstantCoachUpdateClient)));
                }
            }

            return Result<JSchema>.AsError(ErrorType.InvalidData);
        }

        public async Task<Result<InstantCoach>> Create(InstantCoachCreateClient data)
        {
            InstantCoach entity = data.ToNewInstantCoach();
            _logger.LogInformation("New entity: {@EntityModel}", entity);
            return await OnSave(entity, entity.Validate());
        }

        public async Task<Result> Update(int id, InstantCoachUpdateClient data)
        {
            var entityResult = await _repository.FindById(id);
            if (!entityResult.Success) { return OnNotExistingId(id); }
            var entity = entityResult.Value.Update(
                data.UpdateType,
                data.Comments.ToComments(),
                data.BookmarkPins.ToBookmarkPins());
            return await OnSave(entity, entity.Validate());
        }

        public async Task<Result> MarkCompleted(int id)
        {
            var entityResult = await _repository.FindById(id);
            if (!entityResult.Success) { return OnNotExistingId(id); }
            var entity = entityResult.Value.UpdateAsCompleted();
            return await OnSave(entity, entity.Validate());
        }

        public async Task<Result> Remove(int id)
        {
            var entityResult = await _repository.FindById(id);
            if (!entityResult.Success) { return OnNotExistingId(id); }
            var result = await _repository.Delete(entity: entityResult.Value);
            return result;
        }

        private async Task<Result<InstantCoach>> OnSave(InstantCoach entity, ValidationResult validationResult)
        {
            Result<InstantCoach> result;

            if (validationResult.IsValid)
            {
                _logger.LogInformation("Entity on Save: {@EntityModel}", entity);
                result = await _repository.Save(entity);
            }
            else
            {
                result = Result<InstantCoach>.AsDomainError(validationResult.Errors);
            }

            _logger.LogInformation("Create Result: {@Result}", result);
            return result;
        }

        private Result<InstantCoach> OnNotExistingId(int id)
        {
            _logger.LogError("Service Error: Not existing [Id]: {Id}", id);
            return Result<InstantCoach>.AsError(ErrorType.UnknownId);
        }
    }
}