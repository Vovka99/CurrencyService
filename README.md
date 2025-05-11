# Currency Service

A lightweight service that fetches daily exchange rates from the National Bank of Ukraine (NBU) and stores them in a PostgreSQL database. The data can be accessed via HTTP endpoints.

## Features

- Fetches currency rates daily (after 15:30 Kyiv time) using Hangfire.
- Provides endpoints to retrieve rate and calculate average rates.
- Uses PostgreSQL for storage and Dapper for database access.

## Getting Started

To run the service using Docker Compose:

```bash
docker-compose up -d --build
```

This will:

- Build the .NET application
- Start a PostgreSQL database container
- Start the CurrencyService

## API Endpoints

Base URL: `http://localhost:8000`

### Get the rate for a date
```http
GET /v1/rates?date=2025-05-11
```

### Get average rate in a range
```http
GET /v1/rates/average?start=2025-04-10&end=2025-05-10
```

