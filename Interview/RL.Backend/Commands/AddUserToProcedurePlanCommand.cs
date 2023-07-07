using MediatR;
using RL.Backend.Models;

namespace RL.Backend.Commands
{
    public class AddUserToProcedurePlanCommand : AddProcedureToPlanCommand
    {
        public int UserId { get; set; }        
    }
}