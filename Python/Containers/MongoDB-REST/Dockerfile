# Use an official Python runtime as a parent image
FROM python:3.10-slim

# Set the working directory in the container
WORKDIR /app

# Copy the current directory contents into the container at /app
COPY . /app

# Copy the mongo clients
#COPY ../../MongoDB/CRUD/mongodb_crud_client.py /app/mongodb_crud_client.py
#COPY ../../MongoDB/REST/mongodb_rest_client.py /app/mongodb_rest_client.py

# Install any dependencies specified in requirements.txt
RUN pip install --no-cache-dir -r requirements.txt

# Install the python-dotenv package to load environment variables from .env file
RUN pip install python-dotenv

# Copy the .env file into the container
COPY .env /app/.env

# Install Gunicorn
RUN pip install gunicorn

# Make port 80 available to the world outside this container
EXPOSE 80

# Run the application with Gunicorn
CMD ["gunicorn", "--bind", "0.0.0.0:80", "app:app"]
