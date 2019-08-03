using System.Linq;
using FluentAssertions;
using Xunit;
using Domain;
using static Domain.Helpers;
using static Domain.Constants.Validation;
using static Tests.Unit.TestHelpers;

namespace Tests.Unit
{
    public sealed class InstantCoachTests
    {
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
            actual.Should().BeDerivedFrom(typeof(AggregateRoot));
        }

        [Fact]
        public static void Should_be_of_entity_type()
        {
            var actual = typeof(InstantCoach);
            actual.Should().BeDerivedFrom(typeof(Entity));
        }

        [Fact]
        public static void Should_be_valid_on_create_IC_via_ctor()
        {
            InstantCoach sut = NewInstantCoachWitComments();

            var actual = sut.Validate();

            actual.IsValid.Should().BeTrue();
            actual.Errors.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_have_errors_when_comments_are_null_on_create_IC_ctor()
        {
            InstantCoach sut = NewInstantCoach();

            var actual = sut.Validate();
            var expected = CreateValidationResult(
                "Comments", "Comments are required to have at least one element.");

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected.Errors.Count);
            actual.Errors.First().Key.Should().Be(expected.Errors.First().Key);
            actual.Errors.First().Value.First().Should().Be(
                expected.Errors.First().Value.First());
        }

        [Fact]
        public static void Should_be_valid_on_update()
        {
            InstantCoach sut = NewInstantCoachWitComments();
            var result = sut.Update(UpdateType.Save, GetUpdateComments());

            var actual = result.Validate();

            actual.Errors.Should().HaveCount(0);
            actual.IsValid.Should().BeTrue();
        }

        [Fact]
        public static void Should_be_of_status_waiting_on_review_update()
        {
            InstantCoach sut = NewInstantCoachWitComments();

            var actual = sut.Update(UpdateType.Review, GetUpdateComments());
            var result = actual.Validate();

            result.IsValid.Should().BeTrue();
            actual.Status.Should().Be(InstantCoachStatus.Waiting);
        }

        [Fact]
        public static void Should_be_of_status_completed_on_update_as_completed()
        {
            InstantCoach sut = NewInstantCoachWitComments();

            var actual = sut.UpdateAsCompleted();
            var result = actual.Validate();

            result.IsValid.Should().BeTrue();
            actual.Status.Should().Be(InstantCoachStatus.Completed);
        }

        [Fact]
        public static void Should_be_valid_adding_bookmark_pins_on_update()
        {
            InstantCoach sut = NewInstantCoachWitComments();
            sut.AddBookmarkPins(new System.Collections.Generic.List<BookmarkPin>
            {
                new BookmarkPin(id: 2, index: 1, new Range(12, 233),
                    mediaurl: "https://example.com/test.png")
            });
            var bookmarkPins = new System.Collections.Generic.List<BookmarkPin>
            {
                new BookmarkPin(id: 2, index: 1, new Range(12, 233),
                    mediaurl: "https://example.com/test.png"),
                new BookmarkPin(id: 1, index: 1, new Range(1, 2),
                    mediaurl: "https://example.com/test.png", comment: "No comment")
            };
            var result = sut.Update(UpdateType.Save, GetUpdateComments(), bookmarkPins);

            var actual = result.Validate();

            actual.IsValid.Should().BeTrue();
            actual.Errors.Should().HaveCount(0);
            result.BookmarkPins.Should().HaveCount(2);

        }

        [Fact]
        public static void Should_have_errors_when_comments_are_null_on_update()
        {
            InstantCoach sut = NewInstantCoachWitComments();
            var result = sut.Update(UpdateType.Save, comments: null, bookmarkPins: null);

            var actual = result.Validate();
            var expected = CreateValidationResult(
                "Comments", "Comments are required to have at least one element.");

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected.Errors.Count);
            actual.Errors.First().Key.Should().Be(expected.Errors.First().Key);
            actual.Errors.First().Value.First().Should().Be(
                expected.Errors.First().Value.First());
        }

        [Fact]
        public static void Should_have_valid_reference_on_create_IC_via_ctor()
        {
            InstantCoach actual = NewInstantCoachWitComments();

            actual.Reference.Should().StartWith("IC");
            actual.Reference.Should().HaveLength(16);
        }

        [Fact]
        public static void Should_be_of_status_New_on_create_IC_via_ctor()
        {
            InstantCoach actual = NewInstantCoachWitComments();
            actual.Status.Should().Be(InstantCoachStatus.New);
        }

        [Fact]
        public static void Should_have_comments_count_set_on_create_IC_via_ctor()
        {
            InstantCoach actual = NewInstantCoachWitComments();
            actual.CommentsCount.Should().Be(2);
        }

        [Fact]
        public static void Should_have_status_in_progress_on_update()
        {
            InstantCoach sut = NewInstantCoachWitComments();

            var actual = sut.Update(UpdateType.Save, GetUpdateComments());
            var result = actual.Validate();

            result.IsValid.Should().BeTrue();
            actual.Status.Should().Be(InstantCoachStatus.InProgress);
        }

        [Fact]
        public static void Should_have_comments_count_set_on_update()
        {
            InstantCoach sut = NewInstantCoachWitComments();

            var actual = sut.Update(UpdateType.Save, GetUpdateComments());
            var result = actual.Validate();

            result.IsValid.Should().BeTrue();
            actual.CommentsCount.Should().Be(3);
        }

        [Fact]
        public static void Should_have_some_properties_on_update_same_as_on_create()
        {
            InstantCoach sut = NewInstantCoachWitComments();

            var actual = sut.Update(UpdateType.Save, GetUpdateComments());
            var result = actual.Validate();

            result.IsValid.Should().BeTrue();
            actual.Reference.Should().Be(sut.Reference);
            actual.Description.Should().Be(sut.Description);
            actual.TicketId.Should().Be(sut.TicketId);
            actual.EvaluatorId.Should().Be(sut.EvaluatorId);
            actual.AgentId.Should().Be(sut.AgentId);
            actual.EvaluatorName.Should().Be(sut.EvaluatorName);
            actual.AgentName.Should().Be(sut.AgentName);
        }

        [Fact]
        public static void Should_have_errors_when_description_is_empty_on_create_IC_ctor()
        {
            var sut = new InstantCoach(
                 description: "",
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 AgentNameValue);
            sut.AddComments(GetComments());

            var actual = sut.Validate();
            var expected = CreateValidationResult("Description", RequiredMsg);

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected.Errors.Count);
            actual.Errors.First().Key.Should().Be(expected.Errors.First().Key);
            actual.Errors.First().Value.First().Should().Be(
                expected.Errors.First().Value.First());
        }

        [Fact]
        public static void Should_have_errors_when_description_larger_then_1000_chars_on_create_IC_ctor()
        {

            var sut = new InstantCoach(
                 description: GenerateStringOfLength(1001),
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 AgentNameValue);
            sut.AddComments(GetComments());

            var actual = sut.Validate();
            var expected = CreateValidationResult(
                "Description", "Should not exceed 1000 characters.");

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected.Errors.Count);
            actual.Errors.First().Key.Should().Be(expected.Errors.First().Key);
            actual.Errors.First().Value.First().Should().Be(
                expected.Errors.First().Value.First());
        }

        [Fact]
        public static void Should_have_errors_when_ticketId_is_empty_on_create_IC_ctor()
        {
            var sut = new InstantCoach(
                 DescriptionValue,
                 ticketId: null,
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 AgentNameValue);
            sut.AddComments(GetComments());

            var actual = sut.Validate();
            var expected = CreateValidationResult("TicketId", RequiredMsg);

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected.Errors.Count);
            actual.Errors.First().Key.Should().Be(expected.Errors.First().Key);
            actual.Errors.First().Value.First().Should().Be(
                expected.Errors.First().Value.First());
        }

        [Fact]
        public static void Should_have_errors_when_ticketId_larger_then_64_chars_on_create_IC_ctor()
        {

            var sut = new InstantCoach(
                 DescriptionValue,
                 ticketId: GenerateStringOfLength(65),
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 AgentNameValue);
            sut.AddComments(GetComments());

            var actual = sut.Validate();
            var expected = CreateValidationResult(
                "TicketId", "Should not exceed 64 characters.");

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected.Errors.Count);
            actual.Errors.First().Key.Should().Be(expected.Errors.First().Key);
            actual.Errors.First().Value.First().Should().Be(
                expected.Errors.First().Value.First());
        }

        [Fact]
        public static void Should_have_errors_when_evaluatorId_smaller_or_equal_to_zero_on_create_IC_ctor()
        {
            var sut = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 evaluatorId: -1,
                 AgentIdValue,
                 EvaluatorNameValue,
                 AgentNameValue);
            sut.AddComments(GetComments());

            var actual = sut.Validate();
            var expected = CreateValidationResult("EvaluatorId", GreaterThanZeroMsg);

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected.Errors.Count);
            actual.Errors.First().Key.Should().Be(expected.Errors.First().Key);
            actual.Errors.First().Value.First().Should().Be(
                expected.Errors.First().Value.First());
        }

        [Fact]
        public static void Should_have_errors_when_agentId_smaller_or_equal_to_zero_on_create_IC_ctor()
        {
            var sut = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 EvaluatorIdValue,
                 agentId: 0,
                 EvaluatorNameValue,
                 AgentNameValue);
            sut.AddComments(GetComments());

            var actual = sut.Validate();
            var expected = CreateValidationResult("AgentId", GreaterThanZeroMsg);

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected.Errors.Count);
            actual.Errors.First().Key.Should().Be(expected.Errors.First().Key);
            actual.Errors.First().Value.First().Should().Be(
                expected.Errors.First().Value.First());
        }

        [Fact]
        public static void Should_have_errors_when_evaluator_name_is_empty_on_create_IC_ctor()
        {
            var sut = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 evaluatorName: "",
                 AgentNameValue);
            sut.AddComments(GetComments());

            var actual = sut.Validate();
            var expected = CreateValidationResult("EvaluatorName", RequiredMsg);

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected.Errors.Count);
            actual.Errors.First().Key.Should().Be(expected.Errors.First().Key);
            actual.Errors.First().Value.First().Should().Be(
                expected.Errors.First().Value.First());
        }

        [Fact]
        public static void Should_have_errors_when_evaluator_name_larger_then_128_chars_on_create_IC_ctor()
        {
            var sut = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 evaluatorName: GenerateStringOfLength(129),
                 AgentNameValue);
            sut.AddComments(GetComments());

            var actual = sut.Validate();
            var expected = CreateValidationResult(
                "EvaluatorName", "Should not exceed 128 characters.");

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected.Errors.Count);
            actual.Errors.First().Key.Should().Be(expected.Errors.First().Key);
            actual.Errors.First().Value.First().Should().Be(
                expected.Errors.First().Value.First());
        }

        [Fact]
        public static void Should_have_errors_when_agent_name_is_empty_on_create_IC_ctor()
        {
            var sut = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 agentName: null);
            sut.AddComments(GetComments());

            var actual = sut.Validate();
            var expected = CreateValidationResult("AgentName", RequiredMsg);

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected.Errors.Count);
            actual.Errors.First().Key.Should().Be(expected.Errors.First().Key);
            actual.Errors.First().Value.First().Should().Be(
                expected.Errors.First().Value.First());
        }

        [Fact]
        public static void Should_have_errors_when_agent_name_larger_then_128_chars_on_create_IC_ctor()
        {
            var sut = new InstantCoach(
                 DescriptionValue,
                 TicketIdValue,
                 EvaluatorIdValue,
                 AgentIdValue,
                 EvaluatorNameValue,
                 agentName: GenerateStringOfLength(129));
            sut.AddComments(GetComments());

            var actual = sut.Validate();
            var expected = CreateValidationResult(
                "AgentName", "Should not exceed 128 characters.");

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected.Errors.Count);
            actual.Errors.First().Key.Should().Be(expected.Errors.First().Key);
            actual.Errors.First().Value.First().Should().Be(
                expected.Errors.First().Value.First());
        }

        [Fact]
        public static void Should_have_errors_when_one_or_more_comments_are_invalid_on_create_IC_ctor()
        {
            InstantCoach sut = NewInstantCoach();
            sut.AddComments(GetInvalidComments());

            var actual = sut.Validate();
            var expected = 2;

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected);
        }

        [Fact]
        public static void Should_have_errors_when_one_or_more_bookmark_pins_are_invalid_on_create_IC_ctor()
        {
            InstantCoach sut = NewInstantCoachWitComments();
            sut.AddBookmarkPins(GetInvalidBookmarkPins());

            var actual = sut.Validate();
            var expected = 1;

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected);
        }
    }
}
