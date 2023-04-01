import {branches} from '../db.js';

let gBranches = {};

export const getBranchesById = async (req, res) =>{
    const id = req.params.branch_id;
 
 [gBranches]= await branches.query("SELECT branch_id,country, state,username,car_id FROM branches WHERE branch_id = ?", [id]);
    res.json(gBranches[0]);
    console.table(gBranches);
}




  export {gBranches};