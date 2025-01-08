<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Support\Facades\Log;

class LogRequestMiddleware
{
    /**
     * Handle an incoming request.
     *
     * @param  \Illuminate\Http\Request  $request
     * @param  \Closure  $next
     * @return mixed
     */
    public function handle($request, Closure $next)
    {
        // Log the request details
        Log::info('Incoming Request', [
            'method' => $request->getMethod(),
            'url' => $request->fullUrl(),
            'headers' => $request->headers->all(),
            'body' => $request->all(),
        ]);

        // Proceed with the request
        $response = $next($request);

        // Log the response details
        Log::info('Response', [
            'status' => $response->getStatusCode(),
            'content' => $response->getContent(),
        ]);

        return $response;
    }
}
