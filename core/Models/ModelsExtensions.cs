using System.Collections.Generic;
using Core.Context;
using static Core.Helpers;

namespace Core.Models
{
    public static class ModelsExtensions
    {
        public static InstantCoachDbEntity ToInstantCoachDbEntity(
            this InstantCoachCreate create,
            (int, string) commentsWithCount)
        {
            (int count, string comments) = commentsWithCount;
            return new InstantCoachDbEntity
            {
                Description = create.Description,
                Status = create.Status,
                TicketId = create.TicketId,
                Reference = create.Reference,
                EvaluatorId = create.EvaluatorId,
                AgentId = create.AgentId,
                EvaluatorName = create.EvaluatorName,
                AgentName = create.AgentName,
                Comments = comments,
                CommentsCount = count,
                BookmarkPins = ToJson(create.BookmarkPins)
            };
        }

        public static InstantCoachDbEntity ToInstantCoachDbEntity(
            this InstantCoachUpdate update,
            InstantCoachDbEntity currentState,
            (int, string) commentsWithCount)
        {
            (int count, string comments) = commentsWithCount;
            var result = currentState;
            result.Status = update.Status;
            result.Comments = comments;
            result.CommentsCount = count;
            result.BookmarkPins = ToJson(update.BookmarkPins);
            return result;
        }

        public static InstantCoachForId ToInstantCoach(this InstantCoachDb db)
        {
            return new InstantCoachForId(
                id: db.Id,
                ticketId: db.TicketId,
                description: db.Description,
                evaluatorName: db.EvaluatorName,
                comments: FromJson<List<Comment>>(db.Comments),
                bookmarkPins: FromJson<List<BookmarkPin>>(db.BookmarkPins));
        }

        public static InstantCoachCreate ToInstantCoachCreate(
            this InstantCoachCreateClient clientData,
            string reference,
            InstantCoachStatus status)
        {
            return new InstantCoachCreate(
                description: clientData.Description,
                status: status,
                ticketId: clientData.TicketId,
                reference: reference,
                evaluatorId: clientData.EvaluatorId,
                agentId: clientData.AgentId,
                evaluatorName: clientData.EvaluatorName,
                agentName: clientData.AgentName,
                comments: clientData.Comments,
                bookmarkPins: clientData.BookmarkPins
            );
        }

        public static InstantCoachUpdate ToInstantCoachUpate(
            this InstantCoachUpdateClient clientData,
            InstantCoachStatus status)
        {
            return new InstantCoachUpdate(
                status: status,
                comments: clientData.Comments,
                bookmarkPins: clientData.BookmarkPins
            );
        }
    }
}