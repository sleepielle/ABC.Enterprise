import express from 'express';
import employeesRoutes from './routes/employees.routes.js'
import branchesRoutes from './routes/branches.routes.js'
import carsRoutes from './routes/cars.routes.js'
const app = express();

app.use(employeesRoutes);
app.use(branchesRoutes);
app.use(carsRoutes);
app.listen(5500);