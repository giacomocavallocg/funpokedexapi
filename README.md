# Fun Pokedex Api

The **Fun Pokedex API**, built with **.NET 8**, exposes two endpoints: one for retrieving Pokémon species information and another for translating their descriptions in a humorous way based on specific characteristics.

The application uses two external REST services:

- [PokeApi](https://pokeapi.co/): to get Pokémon species information
- [FunTranslations](https://funtranslations.com/api/): to get hilarious descriptions

Additionally, the application uses an in-memory cache to store responses from the external APIs, reducing the number of calls and improving performance.

## Installation

After cloning the API, you can run it either using .NET or via Docker.

### Dotnet

To run the application with .NET, you need to download [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0). Optionally, you can open the project with [Visual Studio 2022](https://visualstudio.microsoft.com/en-us/vs/community/).

With Visual Studio, you can install all dependencies using the Visual Studio Installer by selecting the _ASP.NET Web Development_ package and ensuring that .NET 8 is included in the installation.

From Visual Studio, you can:

- Open the solution (file `FunPokedex.sln`)
- Start the application using `Debug > Run`
- Run tests using `Test > Run All Tests`

If you choose the `http` startup configuration, the application will be listening on `http://localhost:5000`.

Alternatively, you can run the application from the command line. Navigate to the `/FunPokedex` directory and run the following commands:

```
dotnet restore

dotnet build

dotnet run --project FunPokedex.Api\FunPokedex.Api.csproj --urls "http://localhost:5000"
```

This will make the application listen on `http://localhost:5000`.

Optionally, you can run the tests with the command:

```
dotnet test
```

In both cases, whether running via Visual Studio or dotnet, the application will be launched in Debug mode, and the Swagger interface will be available at the path `swagger/index.html`.

### Docker

It is possible to run the application via Docker. The prerequisites are:

- Docker (tested with version 24.0.2)

To create an image, navigate to the `/FunPokedex` folder and run the following command:

```
doker build -t funpokeapi .
```

This will create a Docker image called `funpokeapi`.

To start a container based on the image just created, run:

```
docker run -it --rm -p 5000:8080 funpokeapi
```

This will start the service listening on `localhost:5000`. In this case, the application will run in "Release" mode, so the Swagger interface will not be available. You can test the API using tools like Postman or Bruno.

## Solution Architecture

The solution consists of three projects:

- **FunPokedex.Application**: contains the application logic
- **FunPokedex.Api**: a Web API that contains the API layer
- **FunPokedex.Test**: contains the unit tests

### FunPokedex.Application

It contains the application logic and the connection to external services. The project is structured into the following folders:

- **Interfaces**: contains the interfaces that define access to the services
- **Service**: contains the concrete implementation of the services that handle the application logic
- **Infrastructure**: contains the concrete implementation of services that interact with external resources, such as third-party API integrations and caching
- **Models**: contains the models used by the services and integrations
- **Exceptions**: contains the definition of exceptions

Among the services, we have `IPokeApi`, which defines access to Pokémon data; its implementation uses the PokeAPI. The `ITranslationApi` defines access to the translation API, while `IApplicationCache` describes a generic cache for storing temporary data, implemented by an in-memory cache. Additionally, there are two internal services: `PokedexService` and `FunPokedexService`, which implement the Pokémon retrieval and description transformation services. These services utilize the APIs mentioned above and the cache to limit access to them.

### FunPokedex.Api

It define the api layer and has the following structure:

- **Controllers**: defines the API endpoints
- **Exceptions**: defines the exception handling logic
- **Models**: defines the DTOs returned by the APIs
- **Program.cs**: configures the application and services

The `PokedexController` defines the application's API endpoints.

Within `Program.cs`, the concrete services used by the application are defined through dependency injection. Additionally, resilient logic is applied for using the APIs, including the implementation of a `circuit breaker` pattern.

### FunPokedex.Tests

It contains the unit tests for the application:

- **FunPokedex.Api**: contains tests related to the API project
- **FunPokedex.Application**: contains tests related to the Application project

The unit tests focus on individual controllers and services. For each test, the respective dependent services are mocked, and the test outcome is evaluated based on the behavior of these dependencies.

There are no end-to-end integration tests, which have been manually executed using Swagger.

## Endpoints

The application provides two endpoints:

- **GET /pokemon/{name}**

  Provides information about the Pokémon species specified by the `name` parameter. The API returns an object of type `PokemonDto`. If the Pokémon does not exist, the application returns a 404 status code.

- **GET /pokemon/translated/{name}**

  Provides information about the requested Pokémon species but humorously translates its description if one of the following conditions is true:

  - If the Pokémon is legendary or lives in a cave, the description is translated in a "Yoda" style.
  - For all other Pokémon, the description is translated in "Shakespeare" style.
  - If there are errors with the translation service, the original description will be returned.

Boths APIs returns an object of type `PokemonDto`.

```json
{
  "name": "pikachu",
  "description": "When several of these POKéMON gather, their electricity could build and cause lightning storms.",
  "habitat": "forest",
  "isLegendary": false
}
```

In case of an error, the API returns an object of type `ErrorDto`.

```json
{
  "code": "100",
  "message": "Resource not found",
  "details": {
    "message": "species pikachus not found"
  }
}
```

## Error Handling

In the application, errors are handled with the following logic:

- If an error does not compromise the execution of a specific operation, an exception is not thrown; instead, a negative result is returned. For example, if the PokeApi does not find a Pokémon, it does not throw an exception but instead returns a failure message. The controller then returns a 404 status code.
- If the error is unexpected and compromises execution, an exception of type `PokedexApplicationException` is thrown. The API layer catches these exceptions via a filter and returns a 500 error.

All errors, both expected and unexpected, are tracked with a unique error code.

## Notes and Future Developments

The goal was to create an application that is reliable and maintainable, with the aim of having a product close to being ready for production. However, due to time constraints, some aspects were left out, including:

- **Logging Management**: Although the application's logging level is configured, It should be implemented within the code to enable quick and efficient issue resolution.
- **Environment Configuration**: The application does not use configuration parameters. It would be possible to customize the production and development environments, configure logging, and more.
- **In-Memory Cache**: The memory usage of the cache should be considered, ensuring it does not exceed the limit and cause errors. Additionally, cache-related errors are not currently handled, and this should be addressed.
- **Exception Handling**: While an effort was made to standardize error responses, their definition is not centralized and is not well documented.
- **Circuit Breaker Policies**: A more suitable logic should be implemented to address the potential issues that could arise with the APIs.
- **Integration Tests**: End-to-end tests should be implemented using the `Microsoft.AspNetCore.Mvc.Testing` library, with only the API responses being mocked.
- **Authentication or Rate Limiter**: An authentication mechanism or rate limiter should be implemented to prevent overloads and fraudulent access to the APIs.
- **Swagger Documentation**: The Swagger documentation should be improved by adding all possible responses and providing a comprehensive description of the controllers.
