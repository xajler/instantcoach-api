using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using FluentAssertions;
using Domain;
using Core;
using Core.Repositories;
using Core.Services;
using Core.Context;
using Core.Models;
using Api.Controllers.Version1;
using static Tests.Integration.TestHelpers;

namespace Tests.Integration
{
    public sealed class ControllersV1Tests : IDisposable
    {
        private readonly ICContext _context;
        private readonly ApiV1Controller _controller;
        private readonly ILoggerFactory _loggerFactory;

        public ControllersV1Tests()
        {
            var builder = new DbContextOptionsBuilder<ICContext>();
            builder.UseSqlServer(Config.GetSutGuidConnectionString());
            _context = new ICContext(builder.Options);
            _context.Database.Migrate();
            _context.Database.EnsureCreated();

            _loggerFactory = new LoggerFactory();
            var repository = new InstantCoachRepository(_loggerFactory, _context);
            var service = new InstantCoachService(
                _loggerFactory.CreateLogger<InstantCoachService>(), repository);
            _controller = new ApiV1Controller(
                _loggerFactory.CreateLogger<ApiV1Controller>(), service);
        }

        // Repository test
        [Fact]
        public async Task Should_return_error_on_get_non_existing_IC_by_id()
        {
            // Get By Id
            var repository = new InstantCoachRepository(new LoggerFactory(), _context);
            var actual = await repository.GetById(1);

            actual.Success.Should().BeFalse();
            actual.Error.Should().Be(ErrorType.UnknownId);
        }

        // Controller V1
        [Fact]
        public async Task Should_be_ok_result_with_second_page_of_completed_list_when_data_on_GET()
        {
            List<InstantCoachList> items = await Insert4ItemsWith1Completed(_context);
            var response = await _controller.GetAsync(skip: 1, take: 3, showCompleted: true);
            var result = response as ObjectResult;
            var actual = result.Value as ListResult<InstantCoachList>;
            var expected = 1;
            response.Should().BeOfType(typeof(OkObjectResult));
            actual.Items.Should().HaveCount(expected);
            actual.TotalCount.Should().Be(items.Count);
        }

        [Fact]
        public async Task Should_be_not_found_on_non_existing_id_on_GET()
        {
            int id = 1;
            var response = await _controller.GetAsync(id: id);

            response.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task Should_be_bad_request_result_with_invalid_domain_model_on_POST()
        {
            var model = new InstantCoachCreateClient
            {
                Description = "Some Desc",
                TicketId = "76",
                EvaluatorId = 1,
                AgentId = 2,
                EvaluatorName = "Evaluator Name",
                AgentName = "Agent Name",
                Comments = new List<CommentClient>
                {
                    new CommentClient
                    {
                        CommentType = CommentType.Bookmark,
                        AuthorType = EvaluationCommentAuthor.Agent,
                        CreatedAt = DateTime.UtcNow
                    },
                    new CommentClient
                    {
                        CommentType = CommentType.Textual,
                        AuthorType = EvaluationCommentAuthor.Agent,
                        CreatedAt = DateTime.UtcNow
                    },
                    new CommentClient
                    {
                        CommentType = CommentType.Attachment,
                        Text = "Some text",
                        AuthorType = EvaluationCommentAuthor.Agent,
                        CreatedAt = DateTime.UtcNow
                    }
                },
                BookmarkPins = new List<BookmarkPinClient>
                {
                    new BookmarkPinClient
                    {
                        Id = 1,
                        Index = 1,
                        Range = new RangeClient { Start = 2, End = 1 },
                        MediaUrl = "https://example.com/test.png"
                    }
                }
            };
            var response = await _controller.PostAsync(model);

            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async Task Should_not_be_found_result_with_updated_by_model_and_not_existing_id_on_PUT()
        {
            int id = 1;
            var model = new InstantCoachUpdateClient
            {
                UpdateType = UpdateType.Save,
                Comments = new List<CommentClient>
                {
                    new CommentClient
                    {
                        CommentType = CommentType.Bookmark,
                        BookmarkPinId = 1,
                        AuthorType = EvaluationCommentAuthor.Agent,
                        CreatedAt = DateTime.UtcNow
                    }
                },
                BookmarkPins = new List<BookmarkPinClient>
                {
                    new BookmarkPinClient
                    {
                        Id = 1,
                        Index = 1,
                        Range = new RangeClient { Start = 1, End = 2 },
                        MediaUrl = "https://example.com/test.png"
                    }
                }
            };
            var response = await _controller.PutAsync(id, model);

            response.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task Should_be_not_found_result_with_not_existing_id_on_PATCH()
        {
            int id = 1;
            var response = await _controller.PatchAsync(id);
            response.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task Should_be_not_found_result_with_not_existing_id_on_DELETE()
        {
            int id = 1;
            var response = await _controller.DeleteAsync(id);
            response.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _loggerFactory.Dispose();
        }
    }
}