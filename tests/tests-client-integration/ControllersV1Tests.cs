using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using GST.Fake.Authentication.JwtBearer;
using Domain;
using Core;
using Core.Context;
using Core.Models;
using Api;
using static System.Console;
using static Domain.Comment;
using static Tests.Integration.Client.TestHelpers;

namespace Tests.Integration.Client
{
    public class ControllersV1Tests : IClassFixture<TestWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly dynamic _bearer = new ExpandoObject();
        private const string UserName = "SUTUser";
        private readonly string[] Roles = new[] { "Role1" };
        private readonly List<InstantCoachList> _items = new List<InstantCoachList>();
        private readonly ICContext _context;

        public ControllersV1Tests(TestWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // TODO: Don't know how to get Init Services it from Fixture or Startup
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();
            var builder = new DbContextOptionsBuilder<ICContext>();
            builder.UseSqlServer(Config.GetSUTConnectionString())
                   .UseInternalServiceProvider(serviceProvider);
            _context = new ICContext(builder.Options);
            _context.Database.Migrate();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task Should_return_ok_with_empty_list_when_no_data()
        {
            //await _context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE[InstantCoaches]");
            var request = "/api/instantcoaches";
            SetFakeBearerToken();
            var response = await _client.GetAsync(request);


            Action result = () => response.EnsureSuccessStatusCode();
            var expected = new ListResult<InstantCoachList>
            {
                Items = new List<InstantCoachList>().AsReadOnly()
            };

            result.Should().NotThrow();
            var content =  await response.Content.ReadAsStringAsync();
            content.Should().Be(ToJson(expected));
            response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");

            response.Dispose();
        }

        [Fact]
        public async Task Should_return_ok_with_list_without_completed()
        {
            await Insert4ItemsWith1Completed();
            var request = "/api/instantcoaches";
            SetFakeBearerToken();
            var response = await _client.GetAsync(request);


            Action result = () => response.EnsureSuccessStatusCode();
            var expected = ExpectedCount(_items.Where(x => x.Status != InstantCoachStatus.Completed).Count());

            result.Should().NotThrow();
            var content = await response.Content.ReadAsStringAsync();
            //WriteLine($"Default content: {content}");
            content.Should().Contain(expected);

            response.Dispose();
        }

        [Fact]
        public async Task Should_return_ok_with_list_first_page_list_completed()
        {
            await Insert4ItemsWith1Completed();
            var request = $"/api/instantcoaches?skip=0&take=2&showCompleted=true";
            SetFakeBearerToken();
            var response = await _client.GetAsync(request);


            Action result = () => response.EnsureSuccessStatusCode();
            var expected = ExpectedCount(_items.Count());

            result.Should().NotThrow();
            var content = await response.Content.ReadAsStringAsync();
            //WriteLine($"Completed content: {content}");
            content.Should().Contain(expected);

            response.Dispose();
        }

        [Fact]
        public async Task Should_return_ok_with_model_by_id()
        {
            await Insert4ItemsWith1Completed();
            int id = 1;
            var request = $"/api/instantcoaches/{id}";
            SetFakeBearerToken();
            var response = await _client.GetAsync(request);


            Action result = () => response.EnsureSuccessStatusCode();
            var expected = "\"id\":1,\"ticketId\":\"41\",\"description\":\"Some description 1\"";

            result.Should().NotThrow();
            var content = await response.Content.ReadAsStringAsync();
            //WriteLine($"Completed content: {content}");
            content.Should().Contain(expected);

            response.Dispose();
        }

        [Fact]
        public async Task Should_return_success_when_created_from_model()
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
                        Range = new Range(1, 2),
                        MediaUrl = "https://example.com/test.png"
                    }
                }
            };

            var request = "/api/instantcoaches";
            SetFakeBearerToken();
            var response = await _client.PostAsJsonAsync(request, model);


            Action result = () => response.EnsureSuccessStatusCode();
            result.Should().NotThrow();

            response.Dispose();
        }

        [Fact]
        public async Task Should_return_success_when_updated_from_model_and_id()
        {
            await Insert4ItemsWith1Completed();
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
                        Range = new Range(1, 2),
                        MediaUrl = "https://example.com/test.png"
                    }
                }
            };

            var request = $"/api/instantcoaches/{id}";
            SetFakeBearerToken();
            var response = await _client.PutAsJsonAsync(request, model);


            Action result = () => response.EnsureSuccessStatusCode();
            result.Should().NotThrow();

            response.Dispose();
        }

        [Fact]
        public async Task Should_return_success_when_patched_as_completed_by_id()
        {
            await Insert4ItemsWith1Completed();
            int id = 1;
            //var content = new StringContent("{}", Encoding.UTF8, "application/json-patch+json");
            var request = $"/api/instantcoaches/{id}/completed";
            SetFakeBearerToken();
            var response = await _client.PatchAsync(request, null); //, content);


            Action result = () => response.EnsureSuccessStatusCode();
            result.Should().NotThrow();

            //content.Dispose();
            response.Dispose();
        }

        [Fact]
        public async Task Should_return_success_when_deleted_by_id()
        {
            await Insert4ItemsWith1Completed();
            int id = 1;
            var request = $"/api/instantcoaches/{id}";
            SetFakeBearerToken();
            var response = await _client.DeleteAsync(request);


            Action result = () => response.EnsureSuccessStatusCode();
            result.Should().NotThrow();

            response.Dispose();
        }

        public void Dispose()
        {
            _client.Dispose();
            _context.Database.EnsureDeleted();
            _context.Dispose();
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
            WriteLine($"Db IC count: {_context.InstantCoaches.Count()}");
            WriteLine($"Db IC count2: {_context.Set<InstantCoach>().Count()}");
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

        private string ExpectedCount(int count)
        {
            return $"\"totalCount\":{count}";
        }

        private void SetFakeBearerToken()
        {
            _bearer.name = "SUT";
            _client.SetFakeBearerToken(UserName, Roles, (object)_bearer);
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