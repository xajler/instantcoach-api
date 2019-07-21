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
using static Domain.Comment;


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

        public static async Task<List<InstantCoachList>> Insert4ItemsWith1Completed(ICContext context)
        {
            var items = new List<InstantCoachList>();
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

            await context.Set<InstantCoach>().AddAsync(item1);
            await context.Set<InstantCoach>().AddAsync(item2);
            await context.Set<InstantCoach>().AddAsync(item3);
            await context.Set<InstantCoach>().AddAsync(item4);
            await context.SaveChangesAsync();
            //WriteLine($"Inserted items: {item1.Id},{item2.Id},{item3.Id},{item4.Id}");
            var item4Entity = await context.Set<InstantCoach>().FindAsync(item4.Id);
            item4Entity.UpdateAsCompletedAndValidate();
            context.Entry(item4Entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            items.Add(EntityToModelList(item1));
            items.Add(EntityToModelList(item2));
            items.Add(EntityToModelList(item3));
            items.Add(EntityToModelList(item4));
            // WriteLine($"Db IC count: {context.InstantCoaches.Count()}");
            // WriteLine($"Db IC count2: {context.Set<InstantCoach>().Count()}");
            //WriteLine($"Inserted Ids: {item1.Id}, {item2.Id}, {item3.Id}, {item4.Id}");
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

        public static List<Comment> GetComments()
        {
            return new List<Comment>
            {
                Bookmark(
                    bookmarkPinId: 1,
                    authorType: EvaluationCommentAuthor.Agent,
                    createdAt: DateTime.UtcNow),
                Textual(
                    "some comment",
                    authorType: EvaluationCommentAuthor.Evaluator,
                     createdAt: DateTime.UtcNow)
            };
        }

        public static List<BookmarkPin> GetBookmarkPins()
        {
            var result = new List<BookmarkPin>();
            var bookmarkPin = new BookmarkPin(id: 1, index: 1, new Range(1, 2),
                mediaurl: "https://example.com/test.png", comment: "No comment");
            result.Add(bookmarkPin);
            return result;
        }

        private static InstantCoachList EntityToModelList(InstantCoach entity)
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