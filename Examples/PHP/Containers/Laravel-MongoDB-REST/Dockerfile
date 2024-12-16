# Use the official PHP image as the base image
FROM php:8.4-fpm

# Install system dependencies and PHP extensions
RUN apt-get update && apt-get install -y \
    nginx \
    libpng-dev \
    libjpeg-dev \
    libfreetype6-dev \
    libonig-dev \
    libxml2-dev \
    libssl-dev \
    sqlite3 libsqlite3-dev \
    zip unzip git curl supervisor \
    openssh-server \
    && docker-php-ext-install pdo_mysql mbstring exif pcntl bcmath gd pdo_sqlite \
    && pecl install mongodb-1.20.0 \
    && docker-php-ext-enable mongodb \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

# Install Composer
COPY --from=composer:latest /usr/bin/composer /usr/bin/composer

# Set working directory
WORKDIR /var/www/html

# Copy Composer files
COPY composer.json composer.lock artisan ./

# Copy application files
COPY . .

# Install dependencies without development packages
RUN composer install --no-dev --optimize-autoloader

# Copy Nginx configuration file
COPY ./default.conf /etc/nginx/conf.d/default.conf

# Set permissions for Laravel
RUN chown -R www-data:www-data /var/www/html/storage /var/www/html/bootstrap/cache \
    && chmod -R 775 /var/www/html/storage /var/www/html/bootstrap/cache

# Set up Supervisor to manage processes
COPY ./supervisord.conf /etc/supervisor/conf.d/supervisord.conf

# Create the SQLite database file
RUN mkdir -p /var/www/html/database && \
    touch /var/www/html/database/database.sqlite && \
    chown -R www-data:www-data /var/www/html/database

# Run Laravel optimization commands
RUN php artisan cache:clear || true && \
    php artisan config:clear && \
    php artisan route:clear && \
    php artisan view:clear && \
    php artisan optimize

# Configure SSH
RUN mkdir /var/run/sshd && \
    echo 'root:root' | chpasswd && \
    sed -i 's/#PermitRootLogin prohibit-password/PermitRootLogin yes/' /etc/ssh/sshd_config && \
    sed -i 's/UsePAM yes/UsePAM no/' /etc/ssh/sshd_config

# Expose ports
EXPOSE 80 22

# Start Supervisor to manage Nginx, PHP-FPM, and SSH
CMD ["supervisord", "-c", "/etc/supervisor/supervisord.conf"]