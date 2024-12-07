# Use the official PHP image with Apache
FROM php:8.1-apache

# Enable Apache mod_rewrite
RUN a2enmod rewrite

# Install system dependencies
RUN apt-get update && apt-get install -y \
    unzip \
    libzip-dev \
    && docker-php-ext-install zip

# Install Composer
COPY --from=composer:latest /usr/bin/composer /usr/bin/composer

# Copy your app files to the /var/www/html directory
COPY src/ /var/www/html/

# Set working directory
WORKDIR /var/www/html

COPY ./000-default.conf /etc/apache2/sites-available/000-default.conf

# Install MongoDB extension
RUN pecl install mongodb && docker-php-ext-enable mongodb

# Install dependencies
RUN composer install --no-interaction --no-scripts --optimize-autoloader

# Expose port 80 for the web server
EXPOSE 80
