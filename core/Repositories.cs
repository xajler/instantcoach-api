using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Domain;
using Core.Context;
using Core.Models;
using static Core.Constants.Db;

namespace Core.Repositories
{
    public class Repository<T> where T : AggregateRoot
    {
        private readonly ILogger<Repository<T>> _logger;
        private readonly DbContext _context;

        protected Repository(ILoggerFactory loggerFactory, DbContext context)
        {
            _logger = loggerFactory.CreateLogger<Repository<T>>();
            _context = context;
        }

        public async Task<Result<T>> FindById(int id)
        {
            T result = await _context.Set<T>().FindAsync(id);
            if (result == null) { return Result<T>.AsError(ErrorType.UnknownId); }
            return Result<T>.AsSuccess(result);
        }

        public async Task<Result<T>> Save(T entity)
        {
            if (entity == null) { return Result<T>.AsError(ErrorType.InvalidData); }
            if (entity.Id == default)
            {
                _logger.LogInformation("Add Entity Model: {@EntityModel}", entity);
               await _context.Set<T>().AddAsync(entity);
            }
            else
            {
                _logger.LogInformation("Update Entity Model: {@EntityModel}", entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            var result = await _context.SaveChangesAsync();
            return GetCreateResult(entity, rows: result);
        }

        public async Task<Result> Delete(T entity)
        {
            if (entity == null) { return Result<T>.AsError(ErrorType.InvalidData); }
            _context.Set<T>().Remove(entity);
            var result = await _context.SaveChangesAsync();
            return GetResult(result);
        }

        private Result<T> GetCreateResult(T entity, int rows)
        {
            _logger.LogInformation("Saved Changes rows count: {RowCount}", rows);
            return Result<T>.AsSuccess(entity);
        }

        private Result GetResult(int rows)
        {
            _logger.LogInformation("Saved Changes rows count: {RowCount}", rows);
            return Result.AsSuccess();
        }
    }

    public class InstantCoachRepository : Repository<InstantCoach>
    {
        private readonly ILogger<InstantCoachRepository> _logger;
        private readonly ICContext _context;

        public InstantCoachRepository(ILoggerFactory loggerFactory, ICContext context)
            : base(loggerFactory, context)
        {
            _logger = loggerFactory.CreateLogger<InstantCoachRepository>();
            _context = context;
        }

        public async Task<ListResult<InstantCoachList>> GetAll(
            int skip, int take, bool showCompleted)
        {
            SqlParameter[] dbParams = new []
            {
                new SqlParameter(SkipParam, skip),
                new SqlParameter(TakeParam, take),
                new SqlParameter(ShowCompletedParam, showCompleted),
            };
            _logger.LogInformation("Repository Get All DB Parameters: {@DbParams}", dbParams);

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                int totalCount = 0;
                var items = new List<InstantCoachList>();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = GetAllStoreProcedure;
                command.Parameters.AddRange(GetListParams(skip, take, showCompleted));
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
                        return Result<InstantCoachDb>.AsSuccess(InstantCoachDb.FromReader(reader));
                    }
                    _logger.LogError("Repository Get By Id Error. Not existing Id: {Id}", id);
                    return Result<InstantCoachDb>.AsError(ErrorType.UnknownId);
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
                    _logger.LogError("Repository Get Existing Id Error. Not existing [Id]: {Id}", id);
                    return 0;
                }
            }
        }

        private SqlParameter GetAndLogIdParam(int id)
        {
            var result = new SqlParameter(IdParam, id);
            var dbParams = SqlParamValue.ToSelf(IdParam, id.ToString());
            _logger.LogInformation("Get By Id DB Parameters: {@DbParams}", dbParams);
            return result;
        }

        private SqlParameter[] GetListParams(int skip, int take, bool showCompleted)
        {
            SqlParameter[] result = new[]
{
                new SqlParameter(SkipParam, skip),
                new SqlParameter(TakeParam, take),
                new SqlParameter(ShowCompletedParam, showCompleted),
            };

            SqlParamValue[] dbParams = new[]
{
                SqlParamValue.ToSelf(SkipParam, skip.ToString()),
                SqlParamValue.ToSelf(TakeParam, take.ToString()),
                SqlParamValue.ToSelf(ShowCompletedParam, showCompleted.ToString()),
            };
            _logger.LogInformation("Repository Get All DB Parameters: {@DbParams}", dbParams);

            return result;
        }
    }
}