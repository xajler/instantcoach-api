using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using Domain;
using static Domain.Comment;


namespace Core.Context
{
    public static class DbContextExtension
    {

        public static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        public static void EnsureSeeded(this ICContext context)
        {
            if (!context.InstantCoaches.Any())
            {
                // var types = JsonConvert.DeserializeObject<List<ThreatType>>(File.ReadAllText("seed" + Path.DirectorySeparatorChar + "types.json"));
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
                context.Add(item1);
                context.Add(item2);
                context.Add(item3);
                context.Add(item4);
                context.SaveChanges();
                var item4Entity = context.Set<InstantCoach>().Find(item4.Id);
                item4Entity.UpdateAsCompletedAndValidate();
                context.Entry(item4Entity).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        private static List<Comment> GetComments()
        {
            return new List<Comment>
            {
                Bookmark(bookmarkPinId: 1, authorType: EvaluationCommentAuthor.Agent, createdAt: DateTime.UtcNow),
                Textual("some comment", authorType: EvaluationCommentAuthor.Evaluator, createdAt: DateTime.UtcNow)
            };
        }


        private static List<BookmarkPin> GetBookmarkPins()
        {
            var result = new List<BookmarkPin>();
            var bookmarkPin = new BookmarkPin(id: 1, index: 1, new Range(1, 2),
                mediaurl: "https://example.com/test.png", comment: "No comment");
            result.Add(bookmarkPin);
            return result;
        }

    }

}