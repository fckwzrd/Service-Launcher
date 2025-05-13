# Discord Integration Project

This project provides Discord integration functionality using Discord OAuth2 and API.

## Features

- Discord OAuth2 authentication
- User data management
- Configuration service for managing application and user settings
- Новая локальная функция

## Test Section
This is a test change to demonstrate how Git works with file updates.

## Setup

1. Clone the repository
2. Create `app.dev.json` file in the root directory with the following structure:
```json
{
  "Discord": {
    "ClientId": "your_client_id",
    "ClientSecret": "your_client_secret",
    "CallbackPort": 8080
  }
}
```
3. Build and run the project

## Configuration

The application uses two types of configuration:
- Application configuration (`app.dev.json`)
- User data configuration (stored in AppData folder)

## Development

This project is built using:
- .NET
- Discord.Net library
- System.Text.Json for configuration management 