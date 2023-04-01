import {cars} from '../db.js';

let gCars = {};




export const getCarsById = async (req, res) =>{
    const id = req.params.car_id;
    const [gCars] = await cars.query("SELECT id,make,model,year,branch_id FROM cars WHERE id = ?", [id]);
    res.json(gCars[0]);
    console.table(gCars);
}

export {gCars};