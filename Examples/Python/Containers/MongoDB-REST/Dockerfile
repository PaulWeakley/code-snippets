# Use an official Python runtime as a parent image
FROM python:3.13-slim

# Set the working directory in the container
WORKDIR /src

# Copy the current directory contents into the container at /src
COPY . /src

# Install any dependencies specified in requirements.txt
RUN pip install --no-cache-dir -r requirements.txt

# Install the python-dotenv package to load environment variables from .env file
RUN pip install python-dotenv

# Copy the .env file into the container
COPY .env /src/.env

# Install Gunicorn
RUN pip install gunicorn

# Make port 80 available to the world outside this container
EXPOSE 80

# Run the application with Gunicorn
CMD ["gunicorn", "--bind", "0.0.0.0:80", "app:app"]
