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
using Core.Context;
using Core.Models;
using Api;
using static Tests.Integration.TestHelpers;
using static Core.Constants;

namespace Tests.Integration
{
    public class ClientControllersV1Tests : IClassFixture<TestWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly dynamic _bearer = new ExpandoObject();
        private const string UserName = "SUTUser";
        private readonly string[] Roles = new[] { "Role1" };
        private List<InstantCoachList> _items = new List<InstantCoachList>();
        private readonly ICContext _context;

        public ClientControllersV1Tests(TestWebApplicationFactory<Startup> factory)
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
            builder.UseSqlServer(CreateConfigForTest().GetSUTConnectionString())
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
            _items = await Insert4ItemsWith1Completed(_context);
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
            _items = await Insert4ItemsWith1Completed(_context);
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
            _items = await Insert4ItemsWith1Completed(_context);
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
                        Range = new RangeClient { Start = 1, End = 2 },
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
            _items = await Insert4ItemsWith1Completed(_context);
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

            var request = $"/api/instantcoaches/{id}";
            SetFakeBearerToken();
            var response = await _client.PutAsJsonAsync(request, model);


            Action result = () => response.EnsureSuccessStatusCode();
            // WriteLine($"response: {await response.Content.ReadAsStringAsync()}");
            // WriteLine($"response status code: {response.StatusCode}");
            result.Should().NotThrow();

            response.Dispose();
        }

        [Fact]
        public async Task Should_return_success_when_patched_as_completed_by_id()
        {
            _items = await Insert4ItemsWith1Completed(_context);
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
            _items = await Insert4ItemsWith1Completed(_context);
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

        private void SetFakeBearerToken()
        {
            _bearer.name = SUTEnv;
            _client.SetFakeBearerToken(UserName, Roles, (object)_bearer);
        }

        private static string ExpectedCount(int count)
        {
            return $"\"totalCount\":{count}";
        }

    }
}