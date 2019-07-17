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
using static System.Console;
using static Domain.Comment;

namespace Tests.Integration
{
    public class ControllersV1Tests : IDisposable
    {
        private readonly ICContext _context;
        private List<InstantCoachList> _items = new List<InstantCoachList>();
        private readonly ApiV1Controller _controller;
        private readonly ILoggerFactory _loggerFactory;

        public ControllersV1Tests()
        {
            var builder = new DbContextOptionsBuilder<ICContext>();
            builder.UseSqlServer(Config.GetSUTGuidConnectionString());
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

        // [Fact]
        // public async Task Should_be_ok_result_with_empty_list_when_no_data_on_GET()
        // {
        //     var response = await _controller.GetAsync();
        //     var result = response as ObjectResult;
        //     var actual = result.Value as ListResult<InstantCoachList>;
        //     var expected = new ListResult<InstantCoachList>
        //     {
        //         Items = new List<InstantCoachList>().AsReadOnly()
        //     };
        //     response.Should().BeOfType(typeof(OkObjectResult));
        //     actual.Items.Should().HaveCount(expected.Items.Count);
        //     //WriteLine($"Total count: {actual.TotalCount}");
        //     actual.TotalCount.Should().Be(expected.TotalCount);
        // }

        // [Fact]
        // public async Task Should_be_ok_result_with_second_page_of_completed_list_when_data_on_GET()
        // {
        //     await Insert4ItemsWith1Completed();
        //     var response = await _controller.GetAsync(skip: 1, take: 3, showCompleted: true);
        //     var result = response as ObjectResult;
        //     var actual = result.Value as ListResult<InstantCoachList>;
        //     var expected = 1;
        //     response.Should().BeOfType(typeof(OkObjectResult));
        //     actual.Items.Should().HaveCount(expected);
        //     actual.TotalCount.Should().Be(_items.Count);
        // }

        // [Fact]
        // public async Task Should_be_ok_result_with_model_by_id_on_GET()
        // {
        //     await Insert4ItemsWith1Completed();
        //     int id = _items.Select(x => x.Id).First();
        //     var response = await _controller.GetAsync(id: id);
        //     var result = response as ObjectResult;
        //     var actual = result.Value as InstantCoachForId;
        //     var expected = _items.Where(x => x.Id == id).FirstOrDefault();
        //     response.Should().BeOfType(typeof(OkObjectResult));
        //     actual.EvaluatorName.Should().Be(expected.EvaluatorName);
        //     actual.Comments.Should().HaveCount(expected.CommentsCount);
        // }

        // [Fact]
        // public async Task Should_be_not_found_on_non_existing_id_on_GET()
        // {
        //     int id = 1;
        //     var response = await _controller.GetAsync(id: id);

        //     response.Should().BeOfType(typeof(NotFoundObjectResult));
        // }

        // [Fact]
        // public async Task Should_be_created_result_with_created_by_model_on_POST()
        // {
        //     var model = new InstantCoachCreateClient
        //     {
        //         Description = "New description",
        //         TicketId = "50",
        //         EvaluatorId = 1,
        //         AgentId = 2,
        //         EvaluatorName = "John Evaluator",
        //         AgentName = "Jane Agent",
        //         Comments = new List<CommentClient>
        //         {
        //             new CommentClient
        //             {
        //                 CommentType = CommentType.Bookmark,
        //                 BookmarkPinId = 1,
        //                 AuthorType = EvaluationCommentAuthor.Agent,
        //                 CreatedAt = DateTime.UtcNow
        //             }
        //         },
        //         BookmarkPins = new List<BookmarkPinClient>
        //         {
        //             new BookmarkPinClient
        //             {
        //                 Id = 1,
        //                 Index = 1,
        //                 Range = new Range(1, 2),
        //                 MediaUrl = "https://example.com/test.png"
        //             }
        //         }
        //     };
        //     var response = await _controller.PostAsync(model);
        //     var result = response as ObjectResult;
        //     var actual = result.Value as CreatedId;
        //     var expected = 1;

        //     response.Should().BeOfType(typeof(CreatedResult));
        //     actual.Id.Should().Be(expected);
        // }

        // [Fact]
        // public async Task Should_be_bad_request_result_when_model_only_when_enums_not_set_not_valid_on_POST()
        // {
        //     var model = new InstantCoachCreateClient
        //     {
        //         Description = "New description",
        //         TicketId = "50",
        //         EvaluatorId = 1,
        //         AgentId = 2,
        //         EvaluatorName = "John Evaluator",
        //         AgentName = "Jane Agent",
        //         Comments = new List<CommentClient>
        //         {
        //             new CommentClient
        //             {
        //                 BookmarkPinId = 1,
        //                 CreatedAt = DateTime.UtcNow
        //             }
        //         },
        //         BookmarkPins = new List<BookmarkPinClient>
        //         {
        //             new BookmarkPinClient
        //             {
        //                 Id = 1,
        //                 Index = 1,
        //                 Range = new Range(1, 2),
        //                 MediaUrl = "https://example.com/test.png"
        //             }
        //         }
        //     };
        //     var response = await _controller.PostAsync(model);

        //     response.Should().BeOfType(typeof(BadRequestObjectResult));
        // }

        // [Fact]
        // public async Task Should_be_bad_request_result_with_domain_model_not_valid_on_POST()
        // {
        //     var model = new InstantCoachCreateClient
        //     {
        //         Description = "New description",
        //         TicketId = "5088888888888888888888888888888888888888888888888888888888888888",
        //         EvaluatorId = 0,
        //         AgentId = 0,
        //         EvaluatorName = "John Evaluator",
        //         AgentName = "Jane Agent",
        //         Comments = new List<CommentClient>
        //         {
        //             new CommentClient
        //             {
        //                 CommentType = CommentType.Bookmark,
        //                 BookmarkPinId = 0,
        //                 AuthorType = EvaluationCommentAuthor.Agent,
        //                 CreatedAt = DateTime.UtcNow
        //             }
        //         },
        //         BookmarkPins = new List<BookmarkPinClient>
        //         {
        //             new BookmarkPinClient
        //             {
        //                 Id = 0,
        //                 Index = 0,
        //                 Range = new Range(1, 2),
        //                 MediaUrl = "https://example.com/test.png"
        //             }
        //         }
        //     };
        //     var response = await _controller.PostAsync(model);

        //     response.Should().BeOfType(typeof(BadRequestObjectResult));
        // }

        // [Fact]
        // public async Task Should_be_no_content_result_when_updated_by_model_and_id_on_PUT()
        // {
        //     await Insert4ItemsWith1Completed();
        //     int id = _items.Select(x => x.Id).First();
        //     var model = new InstantCoachUpdateClient
        //     {
        //         UpdateType = UpdateType.Save,
        //         Comments = new List<CommentClient>
        //         {
        //             new CommentClient
        //             {
        //                 CommentType = CommentType.Bookmark,
        //                 BookmarkPinId = 1,
        //                 AuthorType = EvaluationCommentAuthor.Agent,
        //                 CreatedAt = DateTime.UtcNow
        //             }
        //         },
        //         BookmarkPins = new List<BookmarkPinClient>
        //         {
        //             new BookmarkPinClient
        //             {
        //                 Id = 1,
        //                 Index = 1,
        //                 Range = new Range(1, 2),
        //                 MediaUrl = "https://example.com/test.png"
        //             }
        //         }
        //     };
        //     var response = await _controller.PutAsync(id, model);

        //     response.Should().BeOfType(typeof(NoContentResult));
        // }

        // [Fact]
        // public async Task Should_be_not_found_result_with_updated_by_model_and_not_existing_id_on_PUT()
        // {
        //     int id = 1;
        //     var model = new InstantCoachUpdateClient
        //     {
        //         UpdateType = UpdateType.Save,
        //         Comments = new List<CommentClient>
        //         {
        //             new CommentClient
        //             {
        //                 CommentType = CommentType.Bookmark,
        //                 BookmarkPinId = 1,
        //                 AuthorType = EvaluationCommentAuthor.Agent,
        //                 CreatedAt = DateTime.UtcNow
        //             }
        //         },
        //         BookmarkPins = new List<BookmarkPinClient>
        //         {
        //             new BookmarkPinClient
        //             {
        //                 Id = 1,
        //                 Index = 1,
        //                 Range = new Range(1, 2),
        //                 MediaUrl = "https://example.com/test.png"
        //             }
        //         }
        //     };
        //     var response = await _controller.PutAsync(id, model);

        //     response.Should().BeOfType(typeof(NotFoundObjectResult));
        // }

        // [Fact]
        // public async Task Should_be_bad_request_result_when_model_not_valid_only_when_enums_not_set_for_update_by_id_on_PUT()
        // {
        //     await Insert4ItemsWith1Completed();
        //     int id = _items.Select(x => x.Id).First();
        //     var model = new InstantCoachUpdateClient
        //     {
        //         UpdateType = UpdateType.Save,
        //         Comments = new List<CommentClient>
        //         {
        //             new CommentClient
        //             {
        //                 BookmarkPinId = 1,
        //                 CreatedAt = DateTime.UtcNow
        //             }
        //         },
        //         BookmarkPins = new List<BookmarkPinClient>
        //         {
        //             new BookmarkPinClient
        //             {
        //                 Id = 1,
        //                 Index = 1,
        //                 Range = new Range(1, 2),
        //                 MediaUrl = "https://example.com/test.png"
        //             }
        //         }
        //     };
        //     var response = await _controller.PutAsync(id, model);

        //     response.Should().BeOfType(typeof(BadRequestObjectResult));
        // }

        // [Fact]
        // public async Task Should_be_bad_request_result_when_domain_model_not_valid_for_update_by_id_on_PUT()
        // {
        //     await Insert4ItemsWith1Completed();
        //     int id = _items.Select(x => x.Id).First();
        //     var model = new InstantCoachUpdateClient
        //     {
        //         UpdateType = UpdateType.Save,
        //         Comments = new List<CommentClient>
        //         {
        //             new CommentClient
        //             {
        //                 CommentType = CommentType.Bookmark,
        //                 BookmarkPinId = 0,
        //                 AuthorType = EvaluationCommentAuthor.Agent,
        //                 CreatedAt = DateTime.UtcNow
        //             }
        //         },
        //         BookmarkPins = new List<BookmarkPinClient>
        //         {
        //             new BookmarkPinClient
        //             {
        //                 Id = 0,
        //                 Index = 0,
        //                 Range = new Range(2, 1),
        //                 MediaUrl = "https://example.com/test.png"
        //             }
        //         }
        //     };
        //     var response = await _controller.PutAsync(id, model);

        //     response.Should().BeOfType(typeof(BadRequestObjectResult));
        // }

        // [Fact]
        // public async Task Should_be_no_content_result_with_id_on_PATCH()
        // {
        //     await Insert4ItemsWith1Completed();
        //     int id = _items.Select(x => x.Id).First();
        //     var response = await _controller.PatchAsync(id);

        //     response.Should().BeOfType(typeof(NoContentResult));
        // }

        // [Fact]
        // public async Task Should_be_not_found_result_with_not_existing_id_on_PATCH()
        // {
        //     int id = 1;
        //     var response = await _controller.PatchAsync(id);
        //     response.Should().BeOfType(typeof(NotFoundObjectResult));
        // }

        // [Fact]
        // public async Task Should_be_no_content_result_with_id_on_DELETE()
        // {
        //     await Insert4ItemsWith1Completed();
        //     int id = _items.Select(x => x.Id).First();
        //     var response = await _controller.DeleteAsync(id);

        //     response.Should().BeOfType(typeof(NoContentResult));
        // }

        // [Fact]
        // public async Task Should_be_not_found_result_with_not_existing_id_on_DELETE()
        // {
        //     int id = 1;
        //     var response = await _controller.DeleteAsync(id);
        //     response.Should().BeOfType(typeof(NotFoundObjectResult));
        // }


        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _loggerFactory.Dispose();
        }

        private async Task Insert4ItemsWith1Completed()
        {
            var item1 = new InstantCoach(
                description: "Some description 1",
                ticketId: "41",
                evaluatorId: 1,
                agentId: 2,
                evaluatorName: "John Evaluator",
                agentName: "Jane Agent");
            item1.AddComments(GetComments());
            item1.AddBookmarkPins(GetBookmarkPins());
            var item2 = new InstantCoach(
                description: "Some description 2",
                ticketId: "42",
                evaluatorId: 1,
                agentId: 2,
                evaluatorName: "John Evaluator",
                agentName: "Jane Agent");
            item2.AddComments(GetComments());
            item2.AddBookmarkPins(GetBookmarkPins());
            var item3 = new InstantCoach(
                description: "Some description 3",
                ticketId: "43",
                evaluatorId: 1,
                agentId: 2,
                evaluatorName: "John Evaluator",
                agentName: "Jane Agent");
            item3.AddComments(GetComments());
            item3.AddBookmarkPins(GetBookmarkPins());
            var item4 = new InstantCoach(
                description: "Some description 3",
                ticketId: "43",
                evaluatorId: 1,
                agentId: 2,
                evaluatorName: "John Evaluator",
                agentName: "Jane Agent");
            item4.AddComments(GetComments());
            item4.AddBookmarkPins(GetBookmarkPins());

            await _context.Set<InstantCoach>().AddAsync(item1);
            await _context.Set<InstantCoach>().AddAsync(item2);
            await _context.Set<InstantCoach>().AddAsync(item3);
            await _context.Set<InstantCoach>().AddAsync(item4);
            await _context.SaveChangesAsync();
            //WriteLine($"Inserted items: {item1.Id},{item2.Id},{item3.Id},{item4.Id}");
            var item4Entity = await _context.Set<InstantCoach>().FindAsync(item4.Id);
            item4Entity.UpdateAsCompletedAndValidate();
            _context.Entry(item4Entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _items.Add(EntityToModelList(item1));
            _items.Add(EntityToModelList(item2));
            _items.Add(EntityToModelList(item3));
            _items.Add(EntityToModelList(item4));
            _items = _items.OrderByDescending(x => x.CreatedAt)
                           .OrderBy(x => x.Status)
                           .ToList();
            //WriteLine($"Db IC count: {_context.InstantCoaches.Count()}");
            //WriteLine($"Inserted Ids: {item1.Id}, {item2.Id}, {item3.Id}, {item4.Id}");
        }

        private List<Comment> GetComments()
        {
            return new List<Comment>
            {
                Bookmark(bookmarkPinId: 1, authorType: EvaluationCommentAuthor.Agent, createdAt: DateTime.UtcNow),
                Textual("some comment", authorType: EvaluationCommentAuthor.Evaluator, createdAt: DateTime.UtcNow)
            };
        }

        private List<BookmarkPin> GetBookmarkPins()
        {
            var result = new List<BookmarkPin>();
            var bookmarkPin = new BookmarkPin(id: 1, index: 1, new Range(1, 2),
                mediaurl: "https://example.com/test.png", comment: "No comment");
            result.Add(bookmarkPin);
            return result;
        }

        private InstantCoachList EntityToModelList(InstantCoach entity)
        {
            return new InstantCoachList(
                id: entity.Id,
                status: entity.Status,
                reference: entity.Reference,
                description: entity.Description,
                createdAt: entity.CreatedAt,
                updatedAt: entity.UpdatedAt,
                commentsCount: entity.CommentsCount,
                evaluatorName: entity.EvaluatorName);
        }
    }
}