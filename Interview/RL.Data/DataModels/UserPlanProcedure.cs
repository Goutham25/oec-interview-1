using RL.Data.DataModels.Common;
using System.ComponentModel.DataAnnotations;

namespace RL.Data.DataModels
{
    public class UserPlanProcedure : IChangeTrackable
    {
        [Key]
        public int UserPlanProcedureId { get; set; }
        public int PlanProcedureId { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual PlanProcedure PlanProcedure { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
