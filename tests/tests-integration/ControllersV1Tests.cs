using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public async Task Should_be_ok_result_with_empty_list_when_no_data_on_GET()
        {
            var response = await _controller.GetAsync();
            var result = response as ObjectResult;
            var actual = result.Value as ListResult<InstantCoachList>;
            var expected = new ListResult<InstantCoachList>
            {
                Items = new List<InstantCoachList>().AsReadOnly()
            };
            response.Should().BeOfType(typeof(OkObjectResult));
            actual.Items.Should().HaveCount(expected.Items.Count);
            actual.TotalCount.Should().Be(expected.TotalCount);
        }

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
        public async Task Should_be_ok_result_with_model_by_id_on_GET()
        {
            List<InstantCoachList> items = await Insert4ItemsWith1Completed(_context);
            int id = items.Select(x => x.Id).First();
            var response = await _controller.GetAsync(id: id);
            var result = response as ObjectResult;
            var actual = result.Value as InstantCoachForId;
            var expected = items.Where(x => x.Id == id).FirstOrDefault();
            response.Should().BeOfType(typeof(OkObjectResult));
            actual.EvaluatorName.Should().Be(expected.EvaluatorName);
            actual.Comments.Should().HaveCount(expected.CommentsCount);
        }

        [Fact]
        public async Task Should_be_not_found_on_non_existing_id_on_GET()
        {
            int id = 1;
            var response = await _controller.GetAsync(id: id);

            response.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task Should_be_created_result_with_created_by_model_on_POST()
        {
            var model = new InstantCoachCreateClient
            {
                Description = "New description",
                TicketId = "50",
                EvaluatorId = 1,
                AgentId = 2,
                EvaluatorName = "John Evaluator",
                AgentName = "Jane Agent",
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
            var response = await _controller.PostAsync(model);
            var result = response as ObjectResult;
            var actual = result.Value as CreatedId;
            var expected = 1;

            response.Should().BeOfType(typeof(CreatedResult));
            actual.Id.Should().Be(expected);
        }

        [Fact]
        public async Task Should_be_bad_request_result_when_model_only_when_enums_not_set_not_valid_on_POST()
        {
            var model = new InstantCoachCreateClient
            {
                Description = "New description",
                TicketId = "50",
                EvaluatorId = 1,
                AgentId = 2,
                EvaluatorName = "John Evaluator",
                AgentName = "Jane Agent",
                Comments = new List<CommentClient>
                {
                    new CommentClient
                    {
                        BookmarkPinId = 1,
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
            var response = await _controller.PostAsync(model);

            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async Task Should_be_bad_request_result_with_domain_model_not_valid_on_POST()
        {
            var model = new InstantCoachCreateClient
            {
                Description = "New description",
                TicketId = "5088888888888888888888888888888888888888888888888888888888888888",
                EvaluatorId = 0,
                AgentId = 0,
                EvaluatorName = "John Evaluator",
                AgentName = "Jane Agent",
                Comments = new List<CommentClient>
                {
                    new CommentClient
                    {
                        CommentType = CommentType.Bookmark,
                        BookmarkPinId = 0,
                        AuthorType = EvaluationCommentAuthor.Agent,
                        CreatedAt = DateTime.UtcNow
                    }
                },
                BookmarkPins = new List<BookmarkPinClient>
                {
                    new BookmarkPinClient
                    {
                        Id = 0,
                        Index = 0,
                        Range = new RangeClient { Start = 1, End = 2 },
                        MediaUrl = "https://example.com/test.png"
                    }
                }
            };
            var response = await _controller.PostAsync(model);

            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async Task Should_be_no_content_result_when_updated_by_model_and_id_on_PUT()
        {
            List<InstantCoachList> items = await Insert4ItemsWith1Completed(_context);
            int id = items.Select(x => x.Id).First();
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

            response.Should().BeOfType(typeof(NoContentResult));
        }

        [Fact]
        public async Task Should_be_not_found_result_with_updated_by_model_and_not_existing_id_on_PUT()
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
        public async Task Should_be_bad_request_result_when_model_not_valid_only_when_enums_not_set_for_update_by_id_on_PUT()
        {
            List<InstantCoachList> items = await Insert4ItemsWith1Completed(_context);
            int id = items.Select(x => x.Id).First();
            var model = new InstantCoachUpdateClient
            {
                UpdateType = UpdateType.Save,
                Comments = new List<CommentClient>
                {
                    new CommentClient
                    {
                        BookmarkPinId = 1,
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

            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async Task Should_be_bad_request_result_when_domain_model_not_valid_for_update_by_id_on_PUT()
        {
            List<InstantCoachList> items = await Insert4ItemsWith1Completed(_context);
            int id = items.Select(x => x.Id).First();
            var model = new InstantCoachUpdateClient
            {
                UpdateType = UpdateType.Save,
                Comments = new List<CommentClient>
                {
                    new CommentClient
                    {
                        CommentType = CommentType.Bookmark,
                        BookmarkPinId = 0,
                        AuthorType = EvaluationCommentAuthor.Agent,
                        CreatedAt = DateTime.UtcNow
                    }
                },
                BookmarkPins = new List<BookmarkPinClient>
                {
                    new BookmarkPinClient
                    {
                        Id = 0,
                        Index = 0,
                        Range = new RangeClient { Start = 1, End = 2 },
                        MediaUrl = "https://example.com/test.png"
                    }
                }
            };
            var response = await _controller.PutAsync(id, model);

            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async Task Should_be_no_content_result_with_id_on_PATCH()
        {
            List<InstantCoachList> items = await Insert4ItemsWith1Completed(_context);
            int id = items.Select(x => x.Id).First();
            var response = await _controller.PatchAsync(id);

            response.Should().BeOfType(typeof(NoContentResult));
        }

        [Fact]
        public async Task Should_be_not_found_result_with_not_existing_id_on_PATCH()
        {
            int id = 1;
            var response = await _controller.PatchAsync(id);
            response.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task Should_be_no_content_result_with_id_on_DELETE()
        {
            List<InstantCoachList> items = await Insert4ItemsWith1Completed(_context);
            int id = items.Select(x => x.Id).First();
            var response = await _controller.DeleteAsync(id);

            response.Should().BeOfType(typeof(NoContentResult));
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