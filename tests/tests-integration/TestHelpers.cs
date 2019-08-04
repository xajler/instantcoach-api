using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Domain;
using Core;
using Core.Context;
using Core.Models;


namespace Tests.Integration
{
    public static class TestHelpers
    {
        public static T FromJson<T>(string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json, GetJsonSettings());
        }

        public static string ToJson<T>(T value) where T : class
        {
            return JsonConvert.SerializeObject(value, GetJsonSettings());
        }

        private static JsonSerializerSettings GetJsonSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        }

        public static List<CommentClient> GetClientComments()
        {
            return new List<CommentClient>
            {
                new CommentClient
                {
                    CommentType = CommentType.Textual,
                    Text = "Some Comment",
                    AuthorType = EvaluationCommentAuthor.Agent,
                    CreatedAt = DateTime.UtcNow
                },
                new CommentClient
                {
                    CommentType = CommentType.Attachment,
                    Text = "http://example.com/example.pdf",
                    AuthorType = EvaluationCommentAuthor.Agent,
                    CreatedAt = DateTime.UtcNow
                },
                new CommentClient
                {
                    CommentType = CommentType.Bookmark,
                    BookmarkPinId = 1,
                    AuthorType = EvaluationCommentAuthor.Agent,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public static List<BookmarkPinClient> GetClientBookmarkPins()
        {
            return new List<BookmarkPinClient>
            {
                new BookmarkPinClient
                {
                    Id = 1,
                    Index = 1,
                    Range = new RangeClient { Start = 1, End = 2 },
                    MediaUrl = "https://example.com/test.png",
                    Comment = "Some comment for bookmark pin"
                }
            };
        }

        public static List<CommentClient> GetInvalidClientComments()
        {
            return new List<CommentClient>
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
            };
        }


        public static async Task<List<InstantCoachList>> Insert4ItemsWith1Completed(ICContext context)
        {
            var items = new List<InstantCoachList>();
            var item1 = InstantCoach.Factory.Create(InsertClientData(1));
            var item2 = InstantCoach.Factory.Create(InsertClientData(2));
            var item3 = InstantCoach.Factory.Create(InsertClientData(3));
            var item4 = InstantCoach.Factory.Create(InsertClientData(4));
            await context.Set<InstantCoach>().AddAsync(item1);
            await context.Set<InstantCoach>().AddAsync(item2);
            await context.Set<InstantCoach>().AddAsync(item3);
            await context.Set<InstantCoach>().AddAsync(item4);
            await context.SaveChangesAsync();
            var item4Entity = await context.Set<InstantCoach>().FindAsync(item4.Id);
            item4Entity = InstantCoach.Factory.UpdateAsCompleted(item4Entity);
            context.Entry(item4Entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            items.Add(EntityToModelList(item1));
            items.Add(EntityToModelList(item2));
            items.Add(EntityToModelList(item3));
            items.Add(EntityToModelList(item4));
            return items;
        }

        public static Config CreateConfigForTest()
        {
            return new Config
            {
                DbHost = "DB_HOST",
                DbName = "DB_NAME",
                DbUser = "DB_USER",
                DbPassword = "DB_PASSWORD",
                InstantCoachStatusDefault = InstantCoachStatus.New,
                JwtAuthority = "JWT_AUTHORITY",
                JwtAudience = "JWT_AUDIENCE"
            };
        }

        private static InstantCoachCreateClient InsertClientData(int index)
        {
            return new InstantCoachCreateClient
            {
                Description = $"Some description {index}",
                TicketId = $"4{index}",
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
                            Range = new RangeClient { Start = 1, End = 2 },
                            MediaUrl = "https://example.com/test.png",
                            Comment = "No comment"
                        }
                    }
            };
        }

        private static InstantCoachList EntityToModelList(InstantCoach entity)
        {
            return new InstantCoachList
            {
                Id = entity.Id,
                Status = entity.Status,
                Reference = entity.Reference,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CommentsCount = entity.CommentsCount,
                EvaluatorName = entity.EvaluatorName
            };
        }
    }
}