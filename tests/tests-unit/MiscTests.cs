using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Domain;
using Core;
using Core.Models;
using Helpers = Domain.Helpers;
using static Domain.Constants.Validation;
using static Tests.Unit.TestHelpers;

namespace Tests.Unit
{
    public sealed class MiscTests
    {
        // Config

        [Fact]
        public static void Should_get_connection_string_from_Config()
        {
            var config = new Config
            {
                DbHost = "DB_HOST",
                DbName = "DB_NAME",
                DbUser = "DB_USER",
                DbPassword = "DB_PASSWORD"

            };
            Environment.SetEnvironmentVariable(config.DbHost, "localhost");
            Environment.SetEnvironmentVariable(config.DbName, "test-db");
            Environment.SetEnvironmentVariable(config.DbUser, "icUser");
            Environment.SetEnvironmentVariable(config.DbPassword, "password");

            var actual = config.GetConnectionString();
            var expected = "Server=localhost;Initial Catalog=test-db;User Id=icUser;Password=password;Integrated Security=false;MultipleActiveResultSets=True;";

            actual.Should().Be(expected);
        }

        [Fact]
        public static void Should_get_SUT_connection_string_from_Config()
        {
            var config = new Config { DbHost = "DB_HOST" };
            Environment.SetEnvironmentVariable(config.DbHost, "localhost");

            var actual = config.GetSutConnectionString();
            var expected = "Server=localhost;Initial Catalog=test_db_sut;User Id=sa;Password=Abc$12345;Integrated Security=false;MultipleActiveResultSets=True;";

            actual.Should().Be(expected);
        }

        [Fact]
        public static void Should_get_SUT_connection_string_with_guid_from_Config()
        {
            var actual = Config.GetSutGuidConnectionString();
            var expected = "Server=localhost;Initial Catalog=test_db_sut_";

            actual.Should().Contain(expected);
        }

        [Fact]
        public static void Should_throw_when_unable_to_get_env_var_in_Config()
        {
            var config = new Config();
            Action actual = () => config.GetConnectionString();
            actual.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public static void Should_throw_when_env_var_is_empty_in_Config()
        {
            var config = new Config { DbHost = "DB_HOST" };
            Environment.SetEnvironmentVariable(config.DbHost, "");

            Action actual = () => config.GetSutConnectionString();
            actual.Should().Throw<ArgumentNullException>();
        }

        // Validation Result

        [Fact]
        public static void Should_not_add_error_to_validation_result_when_range_null_or_empty()
        {
            var actual = new ValidationResult();
            actual.AddError(null, null);
            actual.AddError("", null);

            var expected = 0;

            actual.Errors.Should().HaveCount(expected);
        }

        // Result

        [Fact]
        public static void Should_be_able_to_create_result_as_error()
        {
            var actual = Result.AsError(ErrorType.InvalidData);
            var expected = false;

            actual.Success.Should().Be(expected);
            actual.Error.Should().Be(ErrorType.InvalidData);
        }

        [Fact]
        public static void Should_be_able_to_create_result_as_domain_error()
        {
            var errors = new Dictionary<string, IReadOnlyCollection<string>>
            {
                { "TicketId", new List<string> { RequiredMsg } }
            };
            var actual = Result.AsDomainError(errors);
            var expected = false;

            actual.Success.Should().Be(expected);
            actual.Error.Should().Be(ErrorType.InvalidData);
        }

        // Entity

        [Fact]
        public static void Should_be_equal_when_same_identity()
        {
            TestEntity actual = new TestEntity(id: 1);
            TestEntity expected = new TestEntity(id: 1);

            actual.GetHashCode().Should().Be(expected.GetHashCode());
            actual.Equals(expected).Should().BeTrue();
        }

        [Fact]
        public static void Should_not_be_equal_when_different_identity()
        {
            TestEntity actual = new TestEntity(id: 1);
            TestEntity expected = new TestEntity(id: 2);

            actual.Should().NotBeEquivalentTo(expected);
            actual.GetHashCode().Should().NotBe(expected.GetHashCode());
        }

        [Fact]
        public static void Should_not_be_equal_when_other_entity_is_null()
        {
            var ic = NewInstantCoach();
            InstantCoach nullIc = null;

            var actual = ic.Equals(nullIc);

            actual.Should().BeFalse();
        }

        [Fact]
        public static void Should_be_equal_when_other_entity_is_same_reference()
        {
            var ic = NewInstantCoach();
            InstantCoach ic2 = ic;

            var actual = ic.Equals(ic2);

            actual.Should().BeTrue();
        }

        [Fact]
        public static void Should_not_be_equal_when_both_entities_having_id_of_zero()
        {
            var ic = NewInstantCoach();
            InstantCoach ic2 = NewInstantCoach();

            var actual = ic.Equals(ic2);

            actual.Should().BeFalse();
        }

        // Helpers

        [Fact]
        public static void Should_be_able_to_deserialize_T_from_json()
        {
            var expected = NewInstantCoach();
            var json = Helpers.ToJson(expected);

            var actual = Helpers.FromJson<InstantCoach>(json);

            actual.TicketId.Should().Be(expected.TicketId);
            actual.Status.Should().Be(expected.Status);
        }

        // ICContextDesignTimeFactory

        [Fact]
        public static void Test_to_cover_context_design_time_factory_in_core()
        {
            var factory = new ICContextDesignTimeFactory();
            var actual = factory.CreateDbContext(new string[] { });

            actual.Should().NotBeNull();
        }

        private static InstantCoach NewInstantCoach()
        {
            var clientCreate = new InstantCoachCreateClient
            {
                Description = "Some description",
                TicketId = "42",
                EvaluatorId = 1,
                AgentId = 2,
                EvaluatorName = "John Evaluator",
                AgentName = "Jane Agent"
            };
            return InstantCoach.Factory.Create(clientCreate);
        }
    }
}