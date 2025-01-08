<?php

namespace App\Providers;

use Illuminate\Support\ServiceProvider;
use App\Services\MongoDB\CRUD\MongoDB_Config;

class AppServiceProvider extends ServiceProvider
{
    /**
     * Register any application services.
     */
    public function register(): void
    {
        $this->app->singleton(\MongoDB\Client::class, function ($app) {
            return new \MongoDB\Client(MongoDB_Config::getInstance()->toUri());
        });
    }

    /**
     * Bootstrap any application services.
     */
    public function boot(): void
    {
        //
    }
}
