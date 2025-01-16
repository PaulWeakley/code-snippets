import { Router, Request, Response } from 'express';
import { Telemetry_Controller } from '../controllers/telemetry.controller';

const router: Router = Router();
const controller = new Telemetry_Controller();

router.post('/:start', async (req: Request, res: Response) => controller.post(req, res));

export default router;