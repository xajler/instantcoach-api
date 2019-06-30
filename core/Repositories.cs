using System.Data.SqlClient;
using System.Threading.Tasks;
using Core.Models;
using Core.Context;
using Microsoft.EntityFrameworkCore;
using Core.Contracts;

namespace Core.Repositories
{
    public class InstantCoachRepository : IInstantCoachRepository
    {
        private readonly ICContext _context;

        public InstantCoachRepository(ICContext context) => _context = context;

        public async Task<ListResult<InstantCoachList>> GetAll(
            int skip, int take, bool showCompleted)
        {
            await Task.CompletedTask; // Dummy await, to supress compiler warnings
            throw new System.NotImplementedException();
        }

        public async Task<InstantCoachDb> GetById(int id)
        {
            var existingId = await GetExistingId(id);
            if (existingId == 0) { return null; }
            var idParam = new SqlParameter("@Id", id);
            var query = @"SELECT Id, TicketId, Description, EvaluatorId, Comments, BookmarkPins
FROM InstantCoaches WHERE Id = @Id";
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(idParam);
                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return InstantCoachDb.FromReader(reader);
                    }
                    return null;
                }
            }
        }

        public async Task<int> GetExistingId(int id)
        {
            var idParam = new SqlParameter("@Id", id);
            var query = "SELECT Id FROM InstantCoaches WHERE Id = @Id";
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(idParam);
                await _context.Database.OpenConnectionAsync();
                using(var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync()) { return reader.GetInt32(0); }
                    return 0;
                }
            }
        }

        public async Task Create(InstantCoachCreate model)
        {
            await Task.CompletedTask; // Dummy await, to supress compiler warnings
            throw new System.NotImplementedException();
        }

        public async Task Update(int id, InstantCoachUpdate model)
        {
            await Task.CompletedTask; // Dummy await, to supress compiler warnings
            throw new System.NotImplementedException();
        }

        public async Task UpdateAsCompleted(int id)
        {
            await Task.CompletedTask; // Dummy await, to supress compiler warnings
            throw new System.NotImplementedException();
        }

        public async Task Delete(int id)
        {
            await Task.CompletedTask; // Dummy await, to supress compiler warnings
            throw new System.NotImplementedException();
        }

    }
}