<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;

Route::get('/test', function () {
    return response()->json(['message' => 'API route is working']);
});
Route::get('/health', [\App\Http\Controllers\HealthController::class, 'index']);
Route::get('/users', [\App\Http\Controllers\UserController::class, 'index']);

