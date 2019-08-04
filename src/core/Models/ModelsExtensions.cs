using System.Collections.Generic;
using Domain;
using static Core.Helpers;

namespace Core.Models
{
    public static class ModelsExtensions
    {
        public static InstantCoachForId ToInstantCoachForId(this InstantCoachDb db)
        {
            List<BookmarkPin> bookmarkPins = null;
            if (!string.IsNullOrWhiteSpace(db.BookmarkPins))
            {
                bookmarkPins = FromJson<List<BookmarkPin>>(db.BookmarkPins);
            }
            return new InstantCoachForId(
                id: db.Id,
                ticketId: db.TicketId,
                description: db.Description,
                evaluatorName: db.EvaluatorName,
                comments: FromJson<List<Comment>>(db.Comments),
                bookmarkPins);
        }
    }
}