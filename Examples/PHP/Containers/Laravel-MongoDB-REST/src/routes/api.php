<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\MongoDB_REST_Controller;


Route::get('/health', [\App\Http\Controllers\HealthController::class, 'index']);

Route::get('/mongodb/{db_name}/{collection_name}/{id}', [MongoDB_REST_Controller::class, 'show']);
Route::post('/mongodb/{db_name}/{collection_name}', [MongoDB_REST_Controller::class, 'store']);
Route::put('/mongodb/{db_name}/{collection_name}/{id}', [MongoDB_REST_Controller::class, 'update']);
Route::patch('/mongodb/{db_name}/{collection_name}/{id}', [MongoDB_REST_Controller::class, 'update']);
Route::delete('/mongodb/{db_name}/{collection_name}/{id}', [MongoDB_REST_Controller::class, 'destroy']);

Route::post('/telemetry/{raw_start}', [\App\Http\Controllers\TelemetryController::class, 'index']);


