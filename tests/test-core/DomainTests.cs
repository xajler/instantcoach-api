using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Core.Domain;
using Core.Models;
using static Core.Helpers;
using static System.Console;

namespace Tests.Core.Domain
{
    public class InstantCoachTests
    {
        private const string DescriptionValue = "Some description";
        private const string TicketIdValue = "42";
        private const int AgentIdValue = 1;
        private const int EvaluatorIdValue = 2;
        private const string EvaluatorNameValue = "John Evaluator";
        private const string AgentNameValue = "Jane Agent";
        private readonly string ReferenceValue = $"IC{GetTicksExcludingFirst5Digits()}";

        [Fact]
        public void Should_be_able_create_IC_via_constructor()
        {
            var actual = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue,
                GetComments(),
                null);
            var validationResult = actual.Validate();
            WriteLine($"No of comments: {actual.CommentsCount}");
            // actual.Reference.Should().Equals(ReferenceValue);
            actual.Status.Should().Equals(InstantCoachStatus.InProgress);
            actual.CommentsCount.Should().Be(2);
            validationResult.IsValid.Should().BeTrue();
        }

        private List<Comment> GetComments()
        {
            return new List<Comment>
            {
                Comment.Bookmark(1, EvaluationCommentAuthor.Agent, DateTime.UtcNow),
                Comment.Textual("some comment", EvaluationCommentAuthor.Evaluator, DateTime.UtcNow)
            };
        }
    }
}
