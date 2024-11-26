<?php

namespace App\Http\Controllers;

class HealthController extends Controller
{
    public function index() {
        return response()->json(['message' => 'API route is working']);
    }
}
