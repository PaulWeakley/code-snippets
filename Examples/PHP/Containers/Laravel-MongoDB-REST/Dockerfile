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
    && docker-php-ext-install pdo_mysql mbstring exif pcntl bcmath gd pdo_sqlite \
    && pecl install mongodb-1.20.0 \
    && docker-php-ext-enable mongodb \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

# Install Composer
COPY --from=composer:latest /usr/bin/composer /usr/bin/composer

# Set working directory
WORKDIR /var/www/html

# Copy application files
COPY ./src .
# Delete local log files
RUN rm -rf /var/www/html/storage/logs/*.log
# Delete local db files
RUN rm -rf /var/www/html/database/*.sqlite

# Install dependencies without development packages
RUN composer install --no-dev --optimize-autoloader

# Copy Nginx configuration file
COPY ./default.conf /etc/nginx/sites-enabled/default

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
    php artisan view:clear
# Optimize occurs in supervisord so that environment variables are available

# Start and enable SSH
# Install SSH and setup environment
# RUN apt-get update \
#     && apt-get install -y --no-install-recommends dialog openssh-server \
#     && mkdir -p /var/run/sshd \
#     && ssh-keygen -A \
#     && echo "root:Docker!" | chpasswd \
#     && chmod 600 /etc/ssh/ssh_host_* \
#     && chmod 644 /etc/ssh/sshd_config \
#     && apt-get clean && rm -rf /var/lib/apt/lists/*
# Copy SSH configuration file
# COPY sshd_config /etc/ssh/sshd_config
# Expose SSH port for Azure
# EXPOSE 2222

# Expose ports
EXPOSE 80

# Start Supervisor to manage Nginx, PHP-FPM
CMD ["supervisord", "-c", "/etc/supervisor/supervisord.conf"]