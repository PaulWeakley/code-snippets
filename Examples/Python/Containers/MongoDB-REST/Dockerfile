# Use an official Python runtime as a parent image
FROM python:3.12-slim

# Set the working directory in the container
WORKDIR /src

# Copy the current directory contents into the container at /src
COPY . /src

# Install any dependencies specified in requirements.txt
RUN pip install --no-cache-dir -r requirements.txt

# Install the python-dotenv package to load environment variables from .env file
RUN pip install python-dotenv

# Install Gunicorn
RUN pip install gunicorn
# Install Datadog's Python tracer
RUN pip install ddtrace

# Make port 80 available to the world outside this container
EXPOSE 80

# Run the application with Gunicorn
#CMD ["gunicorn", "--bind", "0.0.0.0:80", "app:app"]
CMD ["ddtrace-run", "gunicorn", "-b", "0.0.0.0:80", "app:app"]

