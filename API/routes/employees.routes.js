import {Router} from 'express';
import { getEmployeesByUsername } from '../controllers/employees.controller.js';

const router = Router();

router.get('/employees/:username', getEmployeesByUsername);
 
export default router;