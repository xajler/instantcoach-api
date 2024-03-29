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
using static Core.Constants;
using static Tests.Integration.TestHelpers;

namespace Tests.Integration
{
    public sealed class ClientControllersV1Tests : IClassFixture<TestWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly dynamic _bearer = new ExpandoObject();
        private const string UserName = "SUTUser";
        private readonly string[] Roles = new[] { "Role1" };
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
            builder.UseSqlServer(CreateConfigForTest().GetSutConnectionString())
                   .UseInternalServiceProvider(serviceProvider);
            _context = new ICContext(builder.Options);
            _context.Database.Migrate();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task Should_return_ok_with_empty_list_when_no_data()
        {
            var request = GetRequestUrlAndSetToken("");

            using(var response = await _client.GetAsync(request))
            {
                Action result = () => response.EnsureSuccessStatusCode();
                var expected = new ListResult<InstantCoachList>
                {
                    Items = new List<InstantCoachList>().AsReadOnly()
                };

                result.Should().NotThrow();
                var content = await response.Content.ReadAsStringAsync();
                content.Should().Be(ToJson(expected));
                response.Content.Headers.ContentType.ToString()
                    .Should().Be("application/json; charset=utf-8");
            }
        }

        [Fact]
        public async Task Should_return_ok_with_list_without_completed()
        {
            List<InstantCoachList> items = await Insert4ItemsWith1Completed(_context);
            var request = GetRequestUrlAndSetToken("");

            using(var response = await _client.GetAsync(request))
            {
                Action result = () => response.EnsureSuccessStatusCode();
                var expected = ExpectedCount(items.Where(
                    x => x.Status != InstantCoachStatus.Completed).Count());

                result.Should().NotThrow();
                var content = await response.Content.ReadAsStringAsync();
                content.Should().Contain(expected);
            }
        }

        [Fact]
        public async Task Should_return_ok_with_list_first_page_list_completed()
        {
            List<InstantCoachList> items = await Insert4ItemsWith1Completed(_context);
            var request = GetRequestUrlAndSetToken($"?skip=0&take=2&showCompleted=true");

            using(var response = await _client.GetAsync(request))
            {
                Action result = () => response.EnsureSuccessStatusCode();
                var expected = ExpectedCount(items.Count());

                result.Should().NotThrow();
                var content = await response.Content.ReadAsStringAsync();
                content.Should().Contain(expected);
            }
        }

        [Fact]
        public async Task Should_return_ok_with_model_by_id()
        {
            await Insert4ItemsWith1Completed(_context);
            int id = 1;
            var request = GetRequestUrlAndSetToken($"/{id}");

            using(var response = await _client.GetAsync(request))
            {
                Action result = () => response.EnsureSuccessStatusCode();
                var expected = "\"id\":1,\"ticketId\":\"41\",\"description\":\"Some description 1\"";

                result.Should().NotThrow();
                var content = await response.Content.ReadAsStringAsync();
                content.Should().Contain(expected);
            }
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
                Comments = GetClientComments(),
                BookmarkPins = GetClientBookmarkPins()
            };
            var request = GetRequestUrlAndSetToken("");

            using(var response = await _client.PostAsJsonAsync(request, model))
            {
                Action result = () => response.EnsureSuccessStatusCode();
                result.Should().NotThrow();
            }
        }


        [Fact]
        public async Task Should_return_bad_request_on_create_when_model_is_invalid()
        {
            var model = new InstantCoachCreateClient
            {
                Description = "New description",
                TicketId = "50",
                EvaluatorId = 0,
                AgentId = 0,
                EvaluatorName = "John Evaluator",
                AgentName = "Jane Agent",
                Comments = GetInvalidClientComments()
            };
            var request = GetRequestUrlAndSetToken("");

            using(var response = await _client.PostAsJsonAsync(request, model))
            {
                int actual = (int)response.StatusCode;
                actual.Should().Be(400);
            }
        }

        [Fact]
        public async Task Should_return_bad_request_on_create_when_no_comments_null_or_empty()
        {
            var model = new InstantCoachCreateClient
            {
                Description = "New description",
                TicketId = "50",
                EvaluatorId = 1,
                AgentId = 2,
                EvaluatorName = "John Evaluator",
                AgentName = "Jane Agent",
                Comments = new List<CommentClient>()
            };

            var request = GetRequestUrlAndSetToken("");

            using(var response = await _client.PostAsJsonAsync(request, model))
            {
                int actual = (int)response.StatusCode;
                actual.Should().Be(400);
            }
        }


        [Fact]
        public async Task Should_return_success_when_updated_from_model_and_id()
        {
            await Insert4ItemsWith1Completed(_context);
            int id = 1;
            var model = new InstantCoachUpdateClient
            {
                UpdateType = UpdateType.Save,
                Comments = GetClientComments(),
                BookmarkPins = GetClientBookmarkPins()
            };
            var request = GetRequestUrlAndSetToken($"/{id}");

            using(var response = await _client.PutAsJsonAsync(request, model))
            {
                Action result = () => response.EnsureSuccessStatusCode();
                result.Should().NotThrow();
            }
        }

        [Fact]
        public async Task Should_return_bad_request_when_updated_with_invalid_model()
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
                        Range = new RangeClient { Start = 0, End = 0 }
                    }
                }
            };
            var request = GetRequestUrlAndSetToken($"/{id}");

            using(var response = await _client.PutAsJsonAsync(request, model))
            {
                int actual = (int)response.StatusCode;
                actual.Should().Be(400);
            }
        }

        [Fact]
        public async Task Should_return_success_when_patched_as_completed_by_id()
        {
            await Insert4ItemsWith1Completed(_context);
            int id = 1;
            var request = GetRequestUrlAndSetToken($"/{id}/completed");

            using(var response = await _client.PatchAsync(request, null))
            {
                Action result = () => response.EnsureSuccessStatusCode();
                result.Should().NotThrow();
            }
        }

        [Fact]
        public async Task Should_return_success_when_deleted_by_id()
        {
            await Insert4ItemsWith1Completed(_context);
            int id = 1;
            var request = GetRequestUrlAndSetToken($"/{id}");

            using(var response = await _client.DeleteAsync(request))
            {
                Action result = () => response.EnsureSuccessStatusCode();
                result.Should().NotThrow();
            }
        }

        public void Dispose()
        {
            _client.Dispose();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private string GetRequestUrlAndSetToken(string requestUrl)
        {
            SetFakeBearerToken();
            return $"/api/instantcoaches{requestUrl}";
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