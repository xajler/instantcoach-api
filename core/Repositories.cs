using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Core.Context;
using Core.Contracts;
using Core.Models;
using static Core.Helpers;

namespace Core.Repositories
{
    public class Repository<T> where T : DbEntity
    {
        private readonly DbContext _context;

        protected Repository(DbContext context)
        {
            _context = context;
        }

        public virtual async Task<T> FetchById(int id)
        {
            T result = await _context.Set<T>().FindAsync(id);
            return result;
        }

        protected async Task<int> AddOrUpdate(T entity)
        {
            if (entity.Id == default)
            {
                _context.Set<T>().Add(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            return await _context.SaveChangesAsync();
        }

        protected async Task<int> Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync();
        }
    }

    public class InstantCoachRepository : Repository<InstantCoachDbEntity>,
        IInstantCoachRepository
    {
        private const string GetAllStoreProcedure = "InstantCoach_List";
        private const string GetByIdQuery = @"SELECT Id, TicketId, Description, EvaluatorName, Comments, BookmarkPins
FROM InstantCoaches WHERE Id = @Id";
        private const string GetExistingIdQuery = "SELECT Id FROM InstantCoaches WHERE Id = @Id";

        private readonly ILogger<InstantCoachRepository> _logger;
        private readonly ICContext _context;

        public InstantCoachRepository(ILogger<InstantCoachRepository> logger, ICContext context)
            : base(context)
        {
            _logger = logger;
            _context = context;
        }

        public override async Task<InstantCoachDbEntity> FetchById(int id)
        {
            return await base.FetchById(id);
        }

        public async Task<ListResult<InstantCoachList>> GetAll(
            int skip, int take, bool showCompleted)
        {
            SqlParameter[] dbParams = new []
            {
                new SqlParameter("@Skip", skip),
                new SqlParameter("@Take", take),
                new SqlParameter("@ShowCompleted", showCompleted),
            };
            _logger.LogInformation($"Repository Get All DB Parameters:\n{ToLogJson(dbParams)}");

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                int totalCount = 0;
                var items = new List<InstantCoachList>();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = GetAllStoreProcedure;
                command.Parameters.AddRange(dbParams);
                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (totalCount == 0) { totalCount = reader.GetInt32(0); }
                        items.Add(InstantCoachList.FromReader(reader));
                    }
                    return new ListResult<InstantCoachList>
                    {
                        Items = items.AsReadOnly(),
                        TotalCount = totalCount
                    };
                }
            }
        }

        public async Task<Result<InstantCoachDb>> GetById(int id)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = GetByIdQuery;
                command.Parameters.Add(GetAndLogIdParam(id));
                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return SuccessResult(InstantCoachDb.FromReader(reader));
                    }
                    _logger.LogError($"Repository Get By Id Error.\nNot existing Id: {id}");
                    return ErrorResult<InstantCoachDb>(ErrorType.UnknownId);
                }
            }
        }

        public async Task<int> GetExistingId(int id)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = GetExistingIdQuery;
                command.Parameters.Add(GetAndLogIdParam(id));
                await _context.Database.OpenConnectionAsync();
                using(var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync()) { return reader.GetInt32(0); }
                    _logger.LogError($"Repository Get Existing Id Error.\nNot existing Id: {id}");
                    return 0;
                }
            }
        }

        public async Task<Result<int>> Add(InstantCoachCreate model)
        {
            var commentsWithCount = GetCommentsWithCount(model.Comments);
            var entity = model.ToInstantCoachDbEntity(commentsWithCount);
            _logger.LogInformation($"Add Entity Model:\n{ToLogJson(entity)}");
            var result = await AddOrUpdate(entity);
            if (result == 0)
            {
                _logger.LogError($"Failed to Save DB Changes");
                return ErrorResult<int>(ErrorType.SaveChangesFailed);
            }
            _logger.LogInformation($"Saved Changes rows count: {result}");
            return SuccessResult(entity.Id);
        }

        public async Task<Result> Update(InstantCoachDbEntity currentEntity,
            InstantCoachUpdate model)
        {
            var commentsWithCount = GetCommentsWithCount(model.Comments);
            var updatedEntity = model.ToInstantCoachDbEntity(
                currentState: currentEntity, commentsWithCount);
            _logger.LogInformation($"Update Entity Model:\n{ToLogJson(updatedEntity)}");
            var result = await AddOrUpdate(updatedEntity);
            return GetResult(rows: result);
        }

        public async Task<Result> UpdateAsCompleted(InstantCoachDbEntity currentEntity)
        {
            currentEntity.Status = InstantCoachStatus.Completed;
            _logger.LogInformation($"Update Completed Entity Model:\n{ToLogJson(currentEntity)}");
            var result = await AddOrUpdate(currentEntity);
            return GetResult(rows: result);
        }

        public async Task<Result> Remove(InstantCoachDbEntity currentEntity)
        {
            var result = await Delete(currentEntity);
            return GetResult(rows: result);
        }

        private Result GetResult(int rows)
        {
            if (rows == 0)
            {
                _logger.LogError($"Failed to Save DB Changes");
                return ErrorResult(ErrorType.SaveChangesFailed);
            }

            _logger.LogInformation($"Saved Changes rows count: {rows}");
            return SuccessResult();
        }

        private (int, string) GetCommentsWithCount(List<Comment> comments)
        {
            if (comments == null)
            {
                return (0, null);
            }
            return (comments.Count, ToJson(comments));
        }

        private SqlParameter GetAndLogIdParam(int id)
        {
            var result = new SqlParameter("@Id", id);
            _logger.LogInformation($"Get By Id DB Parameters:\n{ToLogJson(result)}");
            return result;
        }
    }
}