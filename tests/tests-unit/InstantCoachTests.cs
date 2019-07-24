using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Domain;
using static Domain.Helpers;
using static Domain.Comment;
using static Tests.Unit.TestHelpers;

namespace Tests.Unit
{
    public static class InstantCoachTests
    {
        private const string DescriptionValue = "Some description";
        private const string TicketIdValue = "42";
        private const int AgentIdValue = 1;
        private const int EvaluatorIdValue = 2;
        private const string EvaluatorNameValue = "John Evaluator";
        private const string AgentNameValue = "Jane Agent";

        [Fact]
        public static void Should_be_able_to_create_correct_reference_via_GetTicksExcludingFirst5Digits()
        {
            var result = GetTicksExcludingFirst5Digits();

            var actual = $"IC{result}";

            result.Should().HaveLength(14);
            actual.Should().Contain(result);
            actual.Should().HaveLength(16);
        }

        [Fact]
        public static void Should_be_of_aggregate_root_type()
        {
            var actual = typeof(InstantCoach);
            var expected = typeof(AggregateRoot);

            actual.Should().BeDerivedFrom(expected);
        }

        [Fact]
        public static void Should_be_of_entity_type()
        {
            var actual = typeof(InstantCoach);
            var expected = typeof(Entity);

            actual.Should().BeDerivedFrom(expected);
        }

        [Fact]
        public static void Should_be_equal_when_same_identity()
        {
            var actual = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            actual.AddComments(GetComments());

            actual.UpdateAndValidate(
                 UpdateType.Save,
                 GetUpdateComments(),
                 bookmarkPins: null,
                 id: 1);

            var expected = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            actual.AddComments(GetComments());

            expected.UpdateAndValidate(
                 UpdateType.Save,
                 GetUpdateComments(),
                 bookmarkPins: null,
                 id: 1);
            actual.Should().BeEquivalentTo(expected);
            actual.GetHashCode().Should().Be(expected.GetHashCode());
            actual.GetHashCode(expected).Should().Be(actual.GetHashCode());
            actual.Equals(actual, expected).Should().Be(true);
        }

        [Fact]
        public static void Should_not_be_equal_when_different_identity()
        {
            var actual = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            actual.AddComments(GetComments());

            actual.UpdateAndValidate(
                 UpdateType.Save,
                 GetUpdateComments(),
                 bookmarkPins: null,
                 id: 1);

            var expected = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            actual.AddComments(GetComments());

            expected.UpdateAndValidate(
                 UpdateType.Save,
                 GetUpdateComments(),
                 bookmarkPins: null,
                 id: 2);

            var difference = actual != expected;

            difference.Should().BeTrue();
            actual.Should().NotBeEquivalentTo(expected);
            actual.GetHashCode().Should().NotBe(expected.GetHashCode());
        }

        [Fact]
        public static void Should_be_valid_on_create_IC_via_ctor()
        {
            var ic = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            ic.AddComments(GetComments());

            var actual = ic.Validate();
            int expected = 0;

            actual.IsValid.Should().BeTrue();
            actual.Errors.Should().HaveCount(expected);
        }

        [Fact]
        public static void Should_have_errors_when_comments_are_null_on_create_IC_ctor()
        {
            var ic = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            ic.AddComments(null);

            var actual = ic.Validate();
            var expected = "Comments are required to have at least one element.";

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);
            actual.Errors[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_be_valid_on_update()
        {
            var ic = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            ic.AddComments(GetComments());

            var actual = ic.UpdateAndValidate(
                 UpdateType.Save,
                 GetUpdateComments(),
                 bookmarkPins: null,
                 id: 1);
            int expected = 0;

            actual.Errors.Should().HaveCount(expected);
            actual.IsValid.Should().BeTrue();
        }

        [Fact]
        public static void Should_be_of_status_waiting_on_review_update()
        {
            var actual = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            actual.AddComments(GetComments());

            var result = actual.UpdateAndValidate(
                 UpdateType.Review,
                 GetUpdateComments(),
                 bookmarkPins: null,
                 id: 1);
            InstantCoachStatus expected = InstantCoachStatus.Waiting;

            result.IsValid.Should().BeTrue();
            actual.Status.Should().Be(expected);
        }

        [Fact]
        public static void Should_be_of_status_completed_on_update_as_completed()
        {
            var actual = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            actual.AddComments(GetComments());

            var result = actual.UpdateAsCompletedAndValidate(id: 1);
            InstantCoachStatus expected = InstantCoachStatus.Completed;

            result.IsValid.Should().BeTrue();
            actual.Status.Should().Be(expected);
        }

        [Fact]
        public static void Should_be_valid_adding_bookmark_pins_on_update()
        {
            var ic = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            ic.AddComments(GetComments());

            var actual = ic.UpdateAndValidate(
                 UpdateType.Save,
                 GetUpdateComments(),
                 GetBookmarkPins(),
                 id: 1);
            int expected = 0;

            actual.IsValid.Should().BeTrue();
            actual.Errors.Should().HaveCount(expected);

        }

        [Fact]
        public static void Should_have_errors_when_comments_are_null_on_update()
        {
            var ic = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            ic.AddComments(GetComments());

            var actual = ic.UpdateAndValidate(
                 UpdateType.Save,
                 comments: null,
                 bookmarkPins: null,
                 id: 1);
            var expected = "Comments are required to have at least one element.";

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);
            actual.Errors[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_have_valid_reference_on_create_IC_via_ctor()
        {
            var actual = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            actual.AddComments(GetComments());
            int expected = 16;

            actual.Reference.Should().StartWith("IC");
            actual.Reference.Should().HaveLength(expected);
        }

        [Fact]
        public static void Should_be_of_status_New_on_create_IC_via_ctor()
        {
            var actual = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            actual.AddComments(GetComments());
            InstantCoachStatus expected = InstantCoachStatus.New;


            actual.Status.Should().Be(expected);
        }

        [Fact]
        public static void Should_have_comments_count_set_on_create_IC_via_ctor()
        {
            var actual = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            actual.AddComments(GetComments());
            int expected = 2;

            actual.CommentsCount.Should().Be(expected);
        }

        [Fact]
        public static void Should_have_status_in_progress_on_update()
        {
            var actual = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            actual.AddComments(GetComments());

            var update = actual.UpdateAndValidate(
                 UpdateType.Save,
                 GetUpdateComments(),
                 bookmarkPins: null,
                 id: 1);
            InstantCoachStatus expected = InstantCoachStatus.InProgress;

            update.IsValid.Should().BeTrue();
            actual.Status.Should().Be(expected);
        }

        [Fact]
        public static void Should_have_comments_count_set_on_update()
        {
            var actual = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            actual.AddComments(GetComments());

            var update = actual.UpdateAndValidate(
                 UpdateType.Save,
                 GetUpdateComments(),
                 bookmarkPins: null,
                 id: 1);
            int expected = 3;

            update.IsValid.Should().BeTrue();
            actual.CommentsCount.Should().Be(expected);
        }

        [Fact]
        public static void Should_have_some_properties_on_update_same_as_on_create()
        {
            var actual = new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
            actual.AddComments(GetComments());

            var reference = actual.Reference;
            var description = actual.Description;
            var ticketId = actual.TicketId;
            var evaluatorId = actual.EvaluatorId;
            var agentId = actual.AgentId;
            var evaluatorName = actual.EvaluatorName;
            var agentName = actual.AgentName;

            var update = actual.UpdateAndValidate(
                 UpdateType.Save,
                 GetUpdateComments(),
                 bookmarkPins: null,
                 id: 1);

            update.IsValid.Should().BeTrue();
            actual.Reference.Should().Be(reference);
            actual.Description.Should().Be(description);
            actual.TicketId.Should().Be(ticketId);
            actual.EvaluatorId.Should().Be(evaluatorId);
            actual.AgentId.Should().Be(agentId);
            actual.EvaluatorName.Should().Be(evaluatorName);
            actual.AgentName.Should().Be(agentName);
        }

        [Fact]
        public static void Should_have_errors_when_description_is_empty_on_create_IC_ctor()
        {
            var ic = new InstantCoach(
                 description: "",
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 AgentNameValue);
            ic.AddComments(GetComments());

            var actual = ic.Validate();
            var expected = "Description is required.";

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);
            actual.Errors[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_have_errors_when_description_larger_then_1000_chars_on_create_IC_ctor()
        {

            var ic = new InstantCoach(
                 description: GenerateStringOfLength(1001),
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 AgentNameValue);
            ic.AddComments(GetComments());

            var actual = ic.Validate();
            var expected = "Description should not exceed 1000 characters.";

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);
            actual.Errors[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_have_errors_when_ticketId_is_empty_on_create_IC_ctor()
        {
            var ic = new InstantCoach(
                 DescriptionValue,
                 ticketId: null,
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 AgentNameValue);
            ic.AddComments(GetComments());

            var actual = ic.Validate();
            var expected = "TicketId is required.";

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);
            actual.Errors[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_have_errors_when_ticketId_larger_then_64_chars_on_create_IC_ctor()
        {

            var ic = new InstantCoach(
                 DescriptionValue,
                 ticketId: GenerateStringOfLength(65),
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 AgentNameValue);
            ic.AddComments(GetComments());

            var actual = ic.Validate();
            var expected = "TicketId should not exceed 64 characters.";

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);
            actual.Errors[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_have_errors_when_evaluatorId_smaller_or_equal_to_zero_on_create_IC_ctor()
        {
            var ic = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 -1,
                 AgentIdValue,
                 EvaluatorNameValue,
                 AgentNameValue);
            ic.AddComments(GetComments());

            var actual = ic.Validate();
            var expected = "EvaluatorId should be greater than 0.";

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);
            actual.Errors[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_have_errors_when_agentId_smaller_or_equal_to_zero_on_create_IC_ctor()
        {
            var ic = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 EvaluatorIdValue,
                 0,
                 EvaluatorNameValue,
                 AgentNameValue);
            ic.AddComments(GetComments());

            var actual = ic.Validate();
            var expected = "AgentId should be greater than 0.";

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);
            actual.Errors[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_have_errors_when_evaluator_name_is_empty_on_create_IC_ctor()
        {
            var ic = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 evaluatorName: "",
                 AgentNameValue);
            ic.AddComments(GetComments());

            var actual = ic.Validate();
            var expected = "EvaluatorName is required.";

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);
            actual.Errors[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_have_errors_when_evaluator_name_larger_then_128_chars_on_create_IC_ctor()
        {
            var ic = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 evaluatorName: GenerateStringOfLength(129),
                 AgentNameValue);
            ic.AddComments(GetComments());

            var actual = ic.Validate();
            var expected = "EvaluatorName should not exceed 128 characters.";

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);
            actual.Errors[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_have_errors_when_agent_name_is_empty_on_create_IC_ctor()
        {
            var ic = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 agentName: null);
            ic.AddComments(GetComments());

            var actual = ic.Validate();
            var expected = "AgentName is required.";

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);
            actual.Errors[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_have_errors_when_agent_name_larger_then_128_chars_on_create_IC_ctor()
        {
            var ic = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 agentName: GenerateStringOfLength(129));
            ic.AddComments(GetComments());

            var actual = ic.Validate();
            var expected = "AgentName should not exceed 128 characters.";

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);
            actual.Errors[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_have_errors_when_one_or_more_comments_are_invalid_on_create_IC_ctor()
        {
            var ic = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 AgentNameValue);
            ic.AddComments(GetCommentsWithError());

            var actual = ic.Validate();
            var expected = 2;

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected);
        }

        [Fact]
        public static void Should_have_errors_when_one_or_more_bookmark_pins_are_invalid_on_create_IC_ctor()
        {
            var ic = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 AgentNameValue);
            ic.AddComments(GetComments());
            ic.AddBookmarkPins(GetBookmarkPinsWithError());

            var actual = ic.Validate();
            var expected = 1;

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected);
        }

        private static List<Comment> GetComments()
        {
            return new List<Comment>
            {
                Bookmark(bookmarkPinId: 1, authorType: EvaluationCommentAuthor.Agent, createdAt: DateTime.UtcNow),
                Textual("some comment", authorType: EvaluationCommentAuthor.Evaluator, createdAt: DateTime.UtcNow)
            };
        }

        private static List<Comment> GetUpdateComments()
        {
            var result = GetComments();
            result.Add(
                Attachment(text: "http://somecomment", authorType: EvaluationCommentAuthor.Agent, createdAt: DateTime.UtcNow));
            return result;
        }

        private static List<BookmarkPin> GetBookmarkPins()
        {
            var result = new List<BookmarkPin>();
            var bookmarkPin = new BookmarkPin(id: 1, index: 1, new Range(1, 2),
                mediaurl: "https://example.com/test.png", comment: "No comment");
            result.Add(bookmarkPin);
            return result;
        }

        private static List<BookmarkPin> GetBookmarkPinsWithError()
        {
            var result = new List<BookmarkPin>();
            var bookmarkPin = new BookmarkPin(id: 0, index: 1, new Range(1, 2),
                mediaurl: "https://example.com/test.png", comment: "No comment");
            result.Add(bookmarkPin);
            return result;
        }

        private static List<Comment> GetCommentsWithError()
        {
            return new List<Comment>
            {
                Bookmark(bookmarkPinId: 0, authorType: EvaluationCommentAuthor.Agent, createdAt: DateTime.UtcNow),
                Textual(null, authorType: EvaluationCommentAuthor.Evaluator, createdAt: DateTime.UtcNow),
            };
        }
    }
}
