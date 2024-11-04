// src/app.ts
import express, { Application, Request, Response } from 'express';
import routes from './routes';

const app: Application = express();
const PORT = process.env.PORT || 3000;

app.use(express.json()); // for parsing application/json
app.use('/api', routes); // add your routes

app.get('/', (req: Request, res: Response) => {
  res.send('Hello World!');
});

app.listen(PORT, () => {
  console.log(`Server is running on http://localhost:${PORT}`);
});
