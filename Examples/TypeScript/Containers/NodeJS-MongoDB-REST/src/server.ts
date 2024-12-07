
import initTracing from "./tracing";
import initLogging from "./logging";

// Initialize tracing and logging
//initTracing("server", "server");
//initLogging("server", "server");

import app from "./app";

const PORT = process.env.PORT || 80;

app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});