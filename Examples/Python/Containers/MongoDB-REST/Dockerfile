# Use an official Python runtime as a parent image
FROM python:3.12-slim

# Set the working directory in the container
WORKDIR /src

# Copy the current directory contents into the container at /src
COPY app.py requirements.txt ./
COPY ./src ./src

# Install any dependencies specified in requirements.txt
RUN pip install --no-cache-dir -r requirements.txt

# Install the python-dotenv package to load environment variables from .env file
RUN pip install python-dotenv

# Install Gunicorn
RUN pip install gunicorn

# Start and enable SSH
# Install SSH and setup environment
# RUN apt-get update \
#     && apt-get install -y --no-install-recommends dialog openssh-server supervisor \
#     && mkdir -p /var/run/sshd \
#     && ssh-keygen -A \
#     && echo "root:Docker!" | chpasswd \
#     && chmod 600 /etc/ssh/ssh_host_* \
#     && chmod 644 /etc/ssh/sshd_config \
#     && apt-get clean && rm -rf /var/lib/apt/lists/*
# Copy SSH configuration file
# COPY sshd_config /etc/ssh/sshd_config
# COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf
# Expose SSH port for Azure
# EXPOSE 2222

# Make port 80 available to the world outside this container
EXPOSE 80
# Run the application with Gunicorn
CMD ["gunicorn", "-b", "0.0.0.0:80", "app:app"]
# Start Supervisor to manage Gunicorn with SSH enabled
# CMD ["supervisord", "-c", "/etc/supervisor/supervisord.conf"]