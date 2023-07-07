using MediatR;
using Microsoft.OData.UriParser;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;
using System.Data.Entity;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RL.Backend.Commands.Handlers.Plans;

public class AddUserToProcedurePlanCommandHandler : IRequestHandler<AddUserToProcedurePlanCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;
    private readonly IMediator _mediator;

    public AddUserToProcedurePlanCommandHandler(RLContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<ApiResponse<Unit>> Handle(AddUserToProcedurePlanCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //Validate request
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));
            if (request.UserId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid UserId"));


            var response = await _mediator.Send(new AddProcedureToPlanCommand { PlanId = request.PlanId, ProcedureId = request.ProcedureId }, cancellationToken);

            var PlanProcedures = _context.PlanProcedures
                        .Include(p => p.UserPlanProcedures)
                   .Where(p => p.PlanId == request.PlanId);


            if (!PlanProcedures.Any() )
                return ApiResponse<Unit>.Fail(new NotFoundException($"PlanProcedureId: for {request.PlanId} not found"));


            if (PlanProcedures.Any(p => p.ProcedureId == request.ProcedureId))
            {
                var planProcedure = PlanProcedures.FirstOrDefault(p => p.ProcedureId == request.ProcedureId);

                if (!planProcedure.UserPlanProcedures.Any(p => p.UserId == request.UserId))
                {
                    _context.UserPlanProcedures.Add(new UserPlanProcedure
                    {
                        UserId = request.UserId,
                        PlanProcedureId = planProcedure.PlanProcedureId
                    });

                    await _context.SaveChangesAsync();
                }
            }

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }
}