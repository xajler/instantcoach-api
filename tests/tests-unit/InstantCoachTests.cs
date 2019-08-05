using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Bogus;
using Domain;
using ValidationResult = Domain.ValidationResult;
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
            actual.Should().Implement(typeof(IAggregateRoot));
        }

        [Fact]
        public static void Should_be_of_entity_type()
        {
            var actual = typeof(InstantCoach);
            actual.Should().BeDerivedFrom(typeof(EntityBase));
        }

        [Fact]
        public static void Should_be_valid_on_create_IC_via_ctor()
        {
            InstantCoach actual = InstantCoach.Factory.Create(ClientCreateFull());

            actual.IsValid.Should().BeTrue();
            actual.Errors.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_have_errors_when_comments_are_null_on_create_IC_ctor()
        {
            InstantCoach actual = InstantCoach.Factory.Create(ClientCreate());
            var expected = CreateValidationResult(
                "Comments", "Comments are required to have at least one element.");

            RunGenericAsserts(actual, expected);
        }

        [Fact]
        public static void Should_be_valid_on_update()
        {
            InstantCoach sut = InstantCoach.Factory.Create(ClientCreateWithComments());

            var actual = InstantCoach.Factory.UpdateAsCompleted(current: sut);

            actual.Errors.Should().HaveCount(0);
            actual.IsValid.Should().BeTrue();
        }

        [Fact]
        public static void Should_be_of_status_waiting_on_review_update()
        {
            InstantCoach sut = InstantCoach.Factory.Create(ClientCreateWithComments());

            var actual = InstantCoach.Factory.Update(current: sut,
                ClientUpdate(UpdateType.Review));

            actual.IsValid.Should().BeTrue();
            actual.Status.Should().Be(InstantCoachStatus.Waiting);
        }

        [Fact]
        public static void Should_be_of_status_completed_on_update_as_completed()
        {
            InstantCoach sut = InstantCoach.Factory.Create(ClientCreateWithComments());

            var actual = InstantCoach.Factory.UpdateAsCompleted(current: sut);

            actual.IsValid.Should().BeTrue();
            actual.Status.Should().Be(InstantCoachStatus.Completed);
        }

        [Fact]
        public static void Should_be_valid_adding_bookmark_pins_on_update()
        {
            InstantCoach sut = InstantCoach.Factory.Create(ClientCreateFull());


            var actual = InstantCoach.Factory.Update(current: sut, ClientUpdateFull());

            actual.IsValid.Should().BeTrue();
            actual.Errors.Should().HaveCount(0);
            actual.BookmarkPins.Should().HaveCount(2);
        }

        [Fact]
        public static void Should_have_errors_when_comments_are_null_on_update()
        {
            InstantCoach sut = InstantCoach.Factory.Create(ClientCreateWithComments());
            var clientUpdate = new InstantCoachUpdateClient { UpdateType = UpdateType.Save };

            var actual = InstantCoach.Factory.Update(current: sut, clientUpdate);
            var expected = CreateValidationResult(
                "Comments", "Comments are required to have at least one element.");

            RunGenericAsserts(actual, expected);
        }

        [Fact]
        public static void Should_have_valid_reference_on_create_IC_via_ctor()
        {
            InstantCoach actual = InstantCoach.Factory.Create(ClientCreateWithComments());

            actual.Reference.Should().StartWith("IC");
            actual.Reference.Should().HaveLength(16);
        }

        [Fact]
        public static void Should_be_of_status_New_on_create_IC_via_ctor()
        {
            InstantCoach actual = InstantCoach.Factory.Create(ClientCreateWithComments());
            actual.Status.Should().Be(InstantCoachStatus.New);
        }

        [Fact]
        public static void Should_have_comments_count_set_on_create_IC_via_ctor()
        {
            InstantCoach actual = InstantCoach.Factory.Create(ClientCreateWithComments());
            actual.CommentsCount.Should().Be(2);
        }

        [Fact]
        public static void Should_have_status_in_progress_on_update()
        {
            InstantCoach sut = InstantCoach.Factory.Create(ClientCreateWithComments());

            var actual = InstantCoach.Factory.Update(current: sut, ClientUpdate());

            actual.IsValid.Should().BeTrue();
            actual.Status.Should().Be(InstantCoachStatus.InProgress);
        }

        [Fact]
        public static void Should_have_comments_count_set_on_update()
        {
            InstantCoach sut = InstantCoach.Factory.Create(ClientCreateWithComments());

            var actual = InstantCoach.Factory.Update(current: sut, ClientUpdate());

            actual.IsValid.Should().BeTrue();
            actual.CommentsCount.Should().Be(3);
        }

        [Fact]
        public static void Should_have_some_properties_on_update_same_as_on_create()
        {
            InstantCoach sut = InstantCoach.Factory.Create(ClientCreateWithComments());

            var actual = InstantCoach.Factory.Update(current: sut, ClientUpdate());

            actual.IsValid.Should().BeTrue();
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
            InstantCoach actual = InstantCoach.Factory.Create(
                ClientCreateWithComments(description: ""));
            var expected = CreateValidationResult("Description", RequiredMsg);

            RunGenericAsserts(actual, expected);
        }

        [Fact]
        public static void Should_have_errors_when_description_larger_then_1000_chars_on_create_IC_ctor()
        {
            InstantCoach actual = InstantCoach.Factory.Create(
                ClientCreateWithComments(description: GenerateStringOfLength(1001)));
            var expected = CreateValidationResult(
                "Description", "Should not exceed 1000 characters.");

            RunGenericAsserts(actual, expected);
        }

        [Fact]
        public static void Should_have_errors_when_ticketId_is_empty_on_create_IC_ctor()
        {
            InstantCoachCreateClient clientCreate = ClientCreateWithComments();
            clientCreate.TicketId = null;


            InstantCoach actual = InstantCoach.Factory.Create(clientCreate);
            var expected = CreateValidationResult("TicketId", RequiredMsg);

            RunGenericAsserts(actual, expected);
        }

        [Fact]
        public static void Should_have_errors_when_ticketId_larger_then_64_chars_on_create_IC_ctor()
        {
            InstantCoach actual = InstantCoach.Factory.Create(
                ClientCreateWithComments(ticketId: GenerateStringOfLength(65)));
            var expected = CreateValidationResult(
                "TicketId", "Should not exceed 64 characters.");

            RunGenericAsserts(actual, expected);
        }

        [Fact]
        public static void Should_have_errors_when_evaluatorId_smaller_or_equal_to_zero_on_create_IC_ctor()
        {
            InstantCoach actual = InstantCoach.Factory.Create(
                ClientCreateWithComments(evaluatorId: -1));
            var expected = CreateValidationResult("EvaluatorId", GreaterThanZeroMsg);

            RunGenericAsserts(actual, expected);
        }

        [Fact]
        public static void Should_have_errors_when_agentId_smaller_or_equal_to_zero_on_create_IC_ctor()
        {
            InstantCoachCreateClient clientCreate = ClientCreateWithComments();
            clientCreate.AgentId = 0;

            InstantCoach actual = InstantCoach.Factory.Create(clientCreate);
            var expected = CreateValidationResult("AgentId", GreaterThanZeroMsg);

            RunGenericAsserts(actual, expected);
        }

        [Fact]
        public static void Should_have_errors_when_evaluator_name_is_empty_on_create_IC_ctor()
        {
            InstantCoach actual = InstantCoach.Factory.Create(
                ClientCreateWithComments(evaluatorName: ""));
            var expected = CreateValidationResult("EvaluatorName", RequiredMsg);

            RunGenericAsserts(actual, expected);
        }

        [Fact]
        public static void Should_have_errors_when_evaluator_name_larger_then_128_chars_on_create_IC_ctor()
        {
            InstantCoach actual = InstantCoach.Factory.Create(
                ClientCreateWithComments(evaluatorName: GenerateStringOfLength(129)));
            var expected = CreateValidationResult(
                "EvaluatorName", "Should not exceed 128 characters.");

            RunGenericAsserts(actual, expected);
        }

        [Fact]
        public static void Should_have_errors_when_agent_name_is_empty_on_create_IC_ctor()
        {
            InstantCoachCreateClient clientCreate = ClientCreateWithComments();
            clientCreate.AgentName = null;

            InstantCoach actual = InstantCoach.Factory.Create(clientCreate);
            var expected = CreateValidationResult("AgentName", RequiredMsg);

            RunGenericAsserts(actual, expected);
        }

        [Fact]
        public static void Should_have_errors_when_agent_name_larger_then_128_chars_on_create_IC_ctor()
        {
            InstantCoach actual = InstantCoach.Factory.Create(
                ClientCreateWithComments(agentName: GenerateStringOfLength(129)));
            var expected = CreateValidationResult(
                "AgentName", "Should not exceed 128 characters.");

            RunGenericAsserts(actual, expected);
        }

        [Fact]
        public static void Should_have_errors_when_one_or_more_comments_are_invalid_on_create_IC_ctor()
        {
            InstantCoach actual = InstantCoach.Factory.Create(ClientCreateInvalidComments());

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(2);
        }

        [Fact]
        public static void Should_have_errors_when_one_or_more_bookmark_pins_are_invalid_on_create_IC_ctor()
        {
            InstantCoach actual = InstantCoach.Factory.Create(
                ClientCreateInvalidBookmarkPins());

            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(3);
        }

        private static void RunGenericAsserts(InstantCoach actual, ValidationResult expected)
        {
            actual.IsValid.Should().BeFalse();
            actual.Errors.Should().HaveCount(expected.Errors.Count);
            actual.Errors.First().Key.Should().Be(expected.Errors.First().Key);
            actual.Errors.First().Value.First().Should().Be(
                expected.Errors.First().Value.First());
        }

        private static ValidationResult CreateValidationResult(
            string memberName, string errorText)
        {
            var result = new ValidationResult();
            result.AddError($"{memberName}", new List<string> { errorText });
            return result;
        }

        private static InstantCoachCreateClient ClientCreate(
            string description = null, string ticketId = null, int evaluatorId = 0,
            int agentId = 0, string evaluatorName = null, string agentName = default)
        {
            var faker = new Faker("en");
            var lorem = new Bogus.DataSets.Lorem("en");
            return new InstantCoachCreateClient
            {
                Description = description ?? lorem.Sentence(),
                TicketId = ticketId ?? faker.Random.Number(1, 10000).ToString(),
                EvaluatorId = evaluatorId == 0 ? faker.Random.Number(1, 10000) : evaluatorId,
                AgentId = agentId == 0 ? faker.Random.Number(1, 10000) : agentId,
                EvaluatorName = evaluatorName ?? faker.Name.FullName(),
                AgentName = agentName ?? faker.Name.FullName()
            };
        }

        private static InstantCoachCreateClient ClientCreateWithComments(
            string description = null, string ticketId = null, int evaluatorId = 0,
            int agentId = 0, string evaluatorName = null, string agentName = null)
        {
            InstantCoachCreateClient result = ClientCreate(description, ticketId,
                evaluatorId, agentId, evaluatorName, agentName);
            result.Comments = GetComments();
            return result;
        }

        private static InstantCoachCreateClient ClientCreateFull(
            string description = null, string ticketId = null, int evaluatorId = 0,
            int agentId = 0, string evaluatorName = null, string agentName = null)
        {
            InstantCoachCreateClient result = ClientCreateWithComments(description, ticketId,
                evaluatorId, agentId, evaluatorName, agentName);
            result.BookmarkPins = new List<BookmarkPinClient> { GetBookmarkPin() };
            return result;
        }

        private static InstantCoachUpdateClient ClientUpdate(
            UpdateType updateType = UpdateType.Save)
        {
            return new InstantCoachUpdateClient
            {
                UpdateType = updateType,
                Comments = GetUpdateComments()
            };
        }

        private static InstantCoachUpdateClient ClientUpdateFull(
            UpdateType updateType = UpdateType.Save)
        {
            return new InstantCoachUpdateClient
            {
                UpdateType = updateType,
                Comments = GetUpdateComments(),
                BookmarkPins = new List<BookmarkPinClient>
                {
                    GetBookmarkPin(),
                    GetBookmarkPin()
                }
            };
        }

        private static List<CommentClient> GetComments()
        {
            var lorem = new Bogus.DataSets.Lorem("en");
            return new List<CommentClient>
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
                    CommentType = CommentType.Textual,
                    Text = lorem.Sentence(),
                    AuthorType = EvaluationCommentAuthor.Evaluator,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        private static List<CommentClient> GetUpdateComments()
        {
            var faker = new Faker("en");
            var result = GetComments();
            result.Add(new CommentClient
            {
                CommentType = CommentType.Attachment,
                Text = faker.Image.PicsumUrl(),
                AuthorType = EvaluationCommentAuthor.Agent,
                CreatedAt = DateTime.UtcNow
            });
            return result;
        }

        private static BookmarkPinClient GetBookmarkPin()
        {
            var faker = new Faker("en");
            var lorem = new Bogus.DataSets.Lorem("en");
            return  new BookmarkPinClient
            {
                    Id = faker.Random.Number(1, 10000),
                    Index = faker.Random.Number(1, 23),
                    Range = new RangeClient { Start = 1, End = 2 },
                    MediaUrl = faker.Internet.UrlWithPath(),
                    Comment = lorem.Sentence()
            };
        }

        private static InstantCoachCreateClient ClientCreateInvalidComments()
        {
            var result = ClientCreate();
            result.Comments = new List<CommentClient>
            {
                 new CommentClient
                {
                    CommentType = CommentType.Bookmark,
                    BookmarkPinId = null,
                    AuthorType = EvaluationCommentAuthor.Agent,
                    CreatedAt = DateTime.UtcNow
                },
                new CommentClient
                {
                    CommentType = CommentType.Textual,
                    Text = null,
                    AuthorType = EvaluationCommentAuthor.Evaluator,
                    CreatedAt = DateTime.UtcNow
                },
                new CommentClient
                {
                    CommentType = CommentType.Attachment,
                    Text = "abc",
                    AuthorType = EvaluationCommentAuthor.Evaluator,
                    CreatedAt = DateTime.UtcNow
                }
            };
            return result;
        }

        private static InstantCoachCreateClient ClientCreateInvalidBookmarkPins()
        {
            var result = ClientCreate();
            result.BookmarkPins = new List<BookmarkPinClient>
            {
                new BookmarkPinClient
                {
                    Id = 0,
                    Index = 0,
                    Range = new RangeClient { Start = 2, End = 1 },
                    MediaUrl = "https://example.com/test.png",
                    Comment = "Some comment for bookmark pin"
                }
            };
            return result;
        }
    }
}
