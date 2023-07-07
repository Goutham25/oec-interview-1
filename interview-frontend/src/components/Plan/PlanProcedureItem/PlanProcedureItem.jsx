import React, { useState, useEffect } from "react";
import ReactSelect from "react-select";

const PlanProcedureItem = ({ procedure,   users, handleAddUserToProcedurePlan, planProcedures }) => {
    const [selectedUsers, setSelectedUsers] = useState(null);

    useEffect(() => {
        (async () => {

          var defaultUsers = [];
          var planProcedure =  planProcedures.find(
            (p) => p.procedureId === procedure.procedureId
        )
        if(planProcedure && planProcedure.userPlanProcedures){
            planProcedure.userPlanProcedures.map((u) => defaultUsers.push({ label: u.user.name, value: u.userId }));    
            setSelectedUsers(defaultUsers);
        }
        })();
      },[]);

    const handleAssignUserToProcedure = (e) => {
        setSelectedUsers(e);
        // TODO: Remove console.log and add missing logic
        handleAddUserToProcedurePlan(procedure,e[e.length-1].value);
        console.log(e);


    };

    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={(e) => handleAssignUserToProcedure(e)}
            />
        </div>
    );
};

export default PlanProcedureItem;
