using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
using FluentAssertions;
using Domain;
using Core.Repositories;
using Core.Services;
using Core.Context;
using Core.Models;
using static System.Console;
using static Domain.Comment;
using static Tests.Unit.TestHelpers;

namespace Tests.Unit
{
    public class ServicesTests : IDisposable
    {
        // private readonly ICContext _context;
        // private readonly IInstantCoachService _repository;

        public ServicesTests()
        {

        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}