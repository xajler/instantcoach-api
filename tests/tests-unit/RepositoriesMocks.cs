using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Xunit;
using Moq;
using Domain;
using Core.Context;
using Core.Models;
using Core.Repositories;
using static Domain.Comment;

namespace Tests.Unit
{
    public class RepositoriesMocks
    {
        // Somehow Simulate DbContext.SaveChanges return 0 for Create/Update/Delete
        //[Fact]
        public async Task Should_retun_error_save_changes_failed_when_no_changes_but_should_have_been()
        {
            var entity = new InstantCoach(
                description: "Some description",
                ticketId: "42",
                evaluatorId: 1,
                agentId: 2,
                evaluatorName: "John Evaluator",
                agentName: "Jane Agent");
            entity.AddComments(GetComments());
            entity.AddBookmarkPins(GetBookmarkPins());


            var contextMock = new Mock<ICContext>();
            var loggerMock = new Mock<ILoggerFactory>();
            contextMock.Setup(x => x.SaveChangesAsync(CancellationToken.None))
            .Returns(Task.FromResult(0));
            // var result = Result<InstantCoach>.AsError(ErrorType.SaveChangesFailed);
            // var repositoryMock = new Mock<InstantCoachRepository>(
            //     loggerMock.Object, contextMock.Object);
            // repositoryMock
            //     .Setup(x => x.Save(entity))
            //     .Returns(Task.FromResult(result));
            var repository = new InstantCoachRepository(
               loggerMock.Object, contextMock.Object);

            //Assert.NotNull(repository);
            var entityMock = new Mock<InstantCoach>();
            var actual = await repository.Save(entity);

            contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);

            // // Act
            // var user = usersService.AddUser(expectedLogin, expectedName, expectedSurname);

            // // Assert
            // Assert.Equal(expectedLogin, user.Login);
            // Assert.Equal(expectedName, user.Name);
            // Assert.Equal(expectedSurname, user.Surname);
            // Assert.False(user.AccountLocked);

            // userContextMock.Verify(x => x.SaveChanges(), Times.Once);
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
            var bookmarkPin = new BookmarkPin(id: 1, index: 1, new Domain.Range(1, 2),
                mediaurl: "https://example.com/test.png", comment: "No comment");
            result.Add(bookmarkPin);
            return result;
        }

    }
}