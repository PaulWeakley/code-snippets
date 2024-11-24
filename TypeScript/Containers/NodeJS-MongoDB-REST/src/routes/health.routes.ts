import { Router, Request, Response } from 'express';
import { Health_Controller } from '../controllers/health.controller';

const router: Router = Router();
const controller = new Health_Controller();

router.get('/', async (req: Request, res: Response) => controller.get(req, res));
router.post('/', (req: Request, res: Response) => controller.get(req, res));

export default router;