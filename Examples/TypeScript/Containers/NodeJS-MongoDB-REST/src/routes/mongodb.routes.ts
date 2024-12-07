import { Router, Request, Response } from 'express';
import { MongoDB_REST_Controller } from '../controllers/mongodb-rest.controller';

const router: Router = Router();
const controller = new MongoDB_REST_Controller();

router.get('/:db_name/:collection_name/:id', async (req: Request, res: Response) => controller.get(req, res));
router.post('/:db_name/:collection_name', async (req: Request, res: Response) => controller.post(req, res));
router.put('/:db_name/:collection_name/:id', async (req: Request, res: Response) => controller.put(req, res));
router.patch('/:db_name/:collection_name/:id', async (req: Request, res: Response) => controller.put(req, res));
router.delete('/:db_name/:collection_name/:id', async (req: Request, res: Response) => controller.delete(req, res));

export default router;