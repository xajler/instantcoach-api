using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Domain;
using Core;
using Core.Repositories;
using Core.Context;
using Core.Models;
using static System.Console;
using static Domain.Comment;
using static Tests.Integration.TestHelpers;

namespace Tests.Integration
{
    public class RepositoriesTests : IDisposable
    {
        private readonly ICContext _context;
        private readonly InstantCoachRepository _repository;

        public RepositoriesTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();
            var builder = new DbContextOptionsBuilder<ICContext>();
            builder.UseSqlServer(Config.GetSUTGuidConnectionString()).UseInternalServiceProvider(serviceProvider);
            _context = new ICContext(builder.Options);
            _context.Database.Migrate();
            _repository = new InstantCoachRepository(new LoggerFactory(), _context);
        }

        // Repository<T>
        [Fact]
        public async Task Should_be_able_to_insert_IC_and_get_created_id()
        {
            var entity = new InstantCoach(
                description: "Some description",
                ticketId: "42",
                evaluatorId: 1,
                agentId: 2,
                evaluatorName: "John Evaluator",
                agentName: "Jane Agent");
            entity.AddComments(GetComments());
            entity.AddBookmarkPins(GetBookmarkPins());

            entity.Id.Should().Be(0);

            // Insert
            var result = await _repository.Save(entity);

            entity.Id.Should().Be(1);
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Should_result_an_error_on_empty_entity_insert_IC_and_get_created_id()
        {
            InstantCoach entity = null;

            // Insert
            var actual = await _repository.Save(entity);

            actual.Success.Should().BeFalse();
            actual.Error.Should().Be(ErrorType.InvalidData);
        }

        [Fact]
        public async Task Should_be_able_to_find_inserted_IC_by_id()
        {

            var entity = new InstantCoach(
                description: "Some description",
                ticketId: "42",
                evaluatorId: 1,
                agentId: 2,
                evaluatorName: "John Evaluator",
                agentName: "Jane Agent");
            entity.AddComments(GetComments());
            entity.AddBookmarkPins(GetBookmarkPins());

            // Insert
            var insertResult = await _repository.Save(entity);
            insertResult.Success.Should().BeTrue();

            // Get By Id
            var findResult = await _repository.FindById(entity.Id);
            var actual = findResult.Value;

            actual.Id.Should().Be(1);
            actual.Reference.Should().Be(entity.Reference);
            actual.Comments.Should().HaveCount(2);
            actual.BookmarkPins.Should().HaveCount(1);
        }

        [Fact]
        public async Task Should_return_error_on_find_non_existing_IC_by_id_Entity()
        {
            // Get By Id
            var actual = await _repository.FindById(1);

            actual.Success.Should().BeFalse();
            actual.Error.Should().Be(ErrorType.UnknownId);
        }

        [Fact]
        public async Task Should_be_able_to_update_inserted_IC_entity()
        {
            var entity = new InstantCoach(
                description: "Some description",
                ticketId: "42",
                evaluatorId: 1,
                agentId: 2,
                evaluatorName: "John Evaluator",
                agentName: "Jane Agent");
            entity.AddComments(GetComments());
            entity.AddBookmarkPins(GetBookmarkPins());

            // Insert
            var insertResult = await _repository.Save(entity);
            insertResult.Success.Should().BeTrue();

            // Find by Id
            var findResult = await _repository.FindById(entity.Id);
            var entityForUpdate = findResult.Value;
            entityForUpdate.UpdateAndValidate(
                updateType: UpdateType.Save,
                comments: GetUpdateComments(),
                bookmarkPins: entity.BookmarkPins);

            // Update
            var updateResult = await _repository.Save(entityForUpdate);
            updateResult.Success.Should().BeTrue();
            var actual = updateResult.Value;

            actual.Id.Should().Be(1);
            actual.Comments.Should().HaveCount(3);
            actual.CommentsCount.Should().Be(3);
            actual.BookmarkPins.Should().HaveCount(1);
        }

        [Fact]
        public async Task Should_be_able_to_update_as_status_completed_inserted_IC_entity()
        {

            var entity = new InstantCoach(
                description: "Some description",
                ticketId: "42",
                evaluatorId: 1,
                agentId: 2,
                evaluatorName: "John Evaluator",
                agentName: "Jane Agent");
            entity.AddComments(GetComments());
            entity.AddBookmarkPins(GetBookmarkPins());

            // Insert
            var insertResult = await _repository.Save(entity);
            insertResult.Success.Should().BeTrue();

            // Find by Id
            var findResult = await _repository.FindById(entity.Id);
            var entityForUpdate = findResult.Value;
            entityForUpdate.UpdateAsCompletedAndValidate();

            // Update
            var updateResult = await _repository.Save(entityForUpdate);
            updateResult.Success.Should().BeTrue();
            var actual = updateResult.Value;

            actual.Id.Should().Be(1);
            actual.Comments.Should().HaveCount(2);
            actual.Status.Should().Be(InstantCoachStatus.Completed);
        }

        [Fact]
        public async Task Should_be_able_to_delete_inserted_IC_entity()
        {

            var entity = new InstantCoach(
                description: "Some description",
                ticketId: "42",
                evaluatorId: 1,
                agentId: 2,
                evaluatorName: "John Evaluator",
                agentName: "Jane Agent");
            entity.AddComments(GetComments());
            entity.AddBookmarkPins(GetBookmarkPins());

            // Insert
            var insertResult = await _repository.Save(entity);
            insertResult.Success.Should().BeTrue();

            // Get By Id
            var findResult = await _repository.FindById(entity.Id);
            var entityForDelete = findResult.Value;

            // Delete
            var actual = await _repository.Delete(entityForDelete);

            actual.Success.Should().BeTrue();

            // Get By Id after Delete
            var unknowResult = await _repository.FindById(entity.Id);
            unknowResult.Error.Should().Be(ErrorType.UnknownId);
        }

        [Fact]
        public async Task Should_result_an_error_on_empty_IC_entity_delete()
        {
            InstantCoach entity = null;

            // Insert
            var actual = await _repository.Delete(entity);

            actual.Success.Should().BeFalse();
            actual.Error.Should().Be(ErrorType.InvalidData);
        }

        // InstantCoachRepository
        [Fact]
        public async Task Should_be_able_to_get_paginated_list_of_ICs_without_completed()
        {
            await Insert4ItemsWith1Completed(_context);

            var actual = await _repository.GetAll(
                skip: 0,
                take: 10,
                showCompleted: false);

            actual.GetType().Should().Be(typeof(ListResult<InstantCoachList>));
            actual.Items.Should().HaveCount(3);
            actual.TotalCount.Should().Be(3);
        }

        [Fact]
        public async Task Should_be_able_to_get_empty_list_of_ICs_when_none_in_db()
        {
            var actual = await _repository.GetAll(
                skip: 0,
                take: 10,
                showCompleted: false);

            actual.Items.Should().HaveCount(0);
            actual.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task Should_be_able_to_get_first_page_of_ICs_with_completed()
        {
            await Insert4ItemsWith1Completed(_context);

            var actual = await _repository.GetAll(
                skip: 0,
                take: 3,
                showCompleted: true);

            actual.Items.Should().HaveCount(3);
            actual.TotalCount.Should().Be(4);
        }

        [Fact]
        public async Task Should_be_able_to_get_second_page_of_ICs_with_completed()
        {
            await Insert4ItemsWith1Completed(_context);

            var actual = await _repository.GetAll(
                skip: 1,
                take: 3,
                showCompleted: true);

            actual.Items.Should().HaveCount(1);
            actual.TotalCount.Should().Be(4);
        }

        [Fact]
        public async Task Should_be_able_to_get_db_model_of_inserted_IC_by_id()
        {
            var entity = new InstantCoach(
                description: "Some description",
                ticketId: "42",
                evaluatorId: 1,
                agentId: 2,
                evaluatorName: "John Evaluator",
                agentName: "Jane Agent");
            entity.AddComments(GetComments());
            entity.AddBookmarkPins(GetBookmarkPins());

            var insertResult = await _repository.Save(entity);
            insertResult.Success.Should().BeTrue();
            var getByIdResult = await _repository.GetById(entity.Id);
            var actual = getByIdResult.Value;

            getByIdResult.Success.Should().BeTrue();
            actual.GetType().Should().Be(typeof(InstantCoachDb));
            actual.Id.Should().Be(1);
            actual.TicketId.Should().Be("42");
            actual.Description.Should().Be("Some description");
            actual.EvaluatorName.Should().Be("John Evaluator");
            actual.Comments.Should().Be(ToJson(entity.Comments));
            actual.BookmarkPins.Should().Be(ToJson(entity.BookmarkPins));
        }

        [Fact]
        public async Task Should_return_error_on_get_non_existing_IC_by_id()
        {
            // Get By Id
            var actual = await _repository.GetById(1);

            actual.Success.Should().BeFalse();
            actual.Error.Should().Be(ErrorType.UnknownId);
        }

        [Fact]
        public async Task Should_be_able_to_get_existing_id_of_inserted_IC_by_id()
        {
            var entity = new InstantCoach(
                description: "Some description",
                ticketId: "42",
                evaluatorId: 1,
                agentId: 2,
                evaluatorName: "John Evaluator",
                agentName: "Jane Agent");
            entity.AddComments(GetComments());
            entity.AddBookmarkPins(GetBookmarkPins());

            var insertResult = await _repository.Save(entity);
            insertResult.Success.Should().BeTrue();
            var actual = await _repository.GetExistingId(entity.Id);

            actual.Should().Be(1);
        }

        [Fact]
        public async Task Should_return_zero_when_getting_not_existing_IC_id()
        {
            var actual = await _repository.GetExistingId(1);

            actual.Should().Be(0);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private static List<Comment> GetUpdateComments()
        {
            var result = GetComments();
            result.Add(Attachment(
                text: "http://somecomment",
                authorType: EvaluationCommentAuthor.Agent,
                createdAt: DateTime.UtcNow));
            return result;
        }
    }
}
