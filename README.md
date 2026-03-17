# TravelPricingGateway

A small ASP.NET Core Web API that simulates a hotel pricing gateway.

The project receives a hotel ID and stay dates, asks multiple providers for prices in parallel, and returns the cheapest successful result.

## What this project shows

- clean separation between **Core**, **Infrastructure**, and **API** layers
- simple use of **async/await** with multiple providers
- support for both **modern JSON** and **legacy XML** provider responses
- basic validation for hotel IDs and stay dates
- dependency injection with a small, easy-to-follow design

## How it works

1. The client sends a request to the API.
2. The controller checks that the hotel exists in the catalog.
3. A search request is created and passed to the aggregator.
4. The aggregator calls all registered providers in parallel.
5. Each provider parses its own response format and returns a common result object.
6. The API returns the cheapest successful price.

## Project structure

- **Core** – domain models, interfaces, and the pricing aggregator
- **Infrastructure** – provider implementations and in-memory hotel repository
- **Controllers** – API endpoint for price lookup
- **Program.cs** – dependency injection and application startup

## Endpoint

`GET /api/hotels/{hotelId}/cheapest-price?checkIn=YYYY-MM-DD&checkOut=YYYY-MM-DD`

Example:

```http
GET /api/hotels/HOTEL_123/cheapest-price?checkIn=2026-03-20&checkOut=2026-03-23
```

## Running the project

```bash
dotnet run
```

The API runs on:

```text
http://localhost:5005
```

## Notes

This is a demo project, so the providers use hardcoded sample responses instead of real external APIs. The main goal is to show the design, the async flow, and the idea of supporting different provider formats behind one clean interface.
