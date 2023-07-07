using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Plans;
using RL.Backend.Exceptions;
using RL.Data;

namespace RL.Backend.UnitTests;

[TestClass]
public class AddUserToProcedurePlanTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task AddUserToProcedurePlanTests_InvalidPlanId_ReturnsBadRequest(int planId)
    {
        //Given
        var context = new Mock<RLContext>();
        var _mediator = new Mock<IMediator>();

        var sut = new AddUserToProcedurePlanCommandHandler(context.Object, _mediator.Object);
        var request = new AddUserToProcedurePlanCommand()
        {
            PlanId = planId,
            ProcedureId = 1,
            UserId = 1
            
        };
        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task AddUserToProcedurePlanTests_InvalidProcedureId_ReturnsBadRequest(int procedureId)
    {
        //Given
        var context = new Mock<RLContext>();
        var _mediator = new Mock<IMediator>();

        var sut = new AddUserToProcedurePlanCommandHandler(context.Object, _mediator.Object);
        var request = new AddUserToProcedurePlanCommand()
        {
            PlanId = 1,
            ProcedureId = procedureId
        };
        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task AddUserToProcedurePlanTests_PlanIdNotFound_ReturnsNotFound(int planId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var _mediator = new Mock<IMediator>();

        var sut = new AddUserToProcedurePlanCommandHandler(context, _mediator.Object);
        var request = new AddUserToProcedurePlanCommand()
        {
            PlanId = planId,
            ProcedureId = 1,
            UserId = 1
        };

        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure
        {
            PlanId = planId + 1,
            PlanProcedureId = 1
        });

        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task AddUserToProcedurePlanTests_ProcedureIdNotFound_ReturnsNotFound(int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var _mediator = new Mock<IMediator>();

        var sut = new AddUserToProcedurePlanCommandHandler(context, _mediator.Object);
        var request = new AddUserToProcedurePlanCommand()
        {
            PlanId = 1,
            ProcedureId = procedureId,
            UserId = 1
        };

        context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = procedureId + 1
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId + 1,
            ProcedureTitle = "Test Procedure"
        });
        context.UserPlanProcedures.Add(new Data.DataModels.UserPlanProcedure
        {
           UserPlanProcedureId = 1,
           PlanProcedureId=1,
           UserId = 1
           
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(19, 1010)]
    [DataRow(35, 69)]
    public async Task AddUserToProcedurePlanTests_AlreadyContainsProcedure_ReturnsSuccess(int planId, int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var _mediator = new Mock<IMediator>();

        var sut = new AddUserToProcedurePlanCommandHandler(context, _mediator.Object);
        var request = new AddUserToProcedurePlanCommand()
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = 1
        };

        context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = planId
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure
        {
            ProcedureId = procedureId,
            PlanId = planId
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(19, 1010)]
    [DataRow(35, 69)]
    public async Task AddUserToProcedurePlanTests_DoesntContainsProcedure_ReturnsSuccess(int planId, int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var _mediator = new Mock<IMediator>();

        var sut = new AddUserToProcedurePlanCommandHandler(context, _mediator.Object);
        var request = new AddUserToProcedurePlanCommand()
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = 1

        };

        context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = planId
        });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });

        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure
        {
            PlanId = planId,
            PlanProcedureId=1,
            ProcedureId= procedureId
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        var dbPlanProcedure = await context.PlanProcedures.FirstOrDefaultAsync(pp => pp.PlanId == planId && pp.ProcedureId == procedureId);

        dbPlanProcedure.Should().NotBeNull();

        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }
}