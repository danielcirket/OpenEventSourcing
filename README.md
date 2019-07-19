# OpenEventSourcing

OpenEventSourcing is a set of .NET Standard libraries that provides a minimal set of abstractions and implementations which help you to build event-driven and event-sourced applications.

## What's in the box?

- Commands, Dispatching and Handling
- Queries, Dispatching and Handling
- Events, Dispatching, Handling, Storage and Event Bus
- Aggregates, Entities and Value Objects
- Projections

## Installation

### Base package

via package manager

```
Install-Package OpenEventSourcing
```

or via .NET CLI

```
dotnet add package OpenEventSourcing
```

### Serializers

By default no assumption is made about which serialization format you are using, so one of the serialization packages needs to be installed.

The current supported formats are:

- OpenEventSourcing.Serialization.Json

via package manager

```
Install-Package OpenEventSourcing.Serialization.Json
```

or via .NET CLI

```
dotnet add package OpenEventSourcing.Serialization.Json
```

### Database provider

In order to use some of the features provided by OpenEventSourcing you will also need to install a accompanying database provider package.

The current supported providers are:

- OpenEventSourcing.EntityFrameworkCore.SqlServer
- OpenEventSourcing.EntityFrameworkCore.Postgres
- OpenEventSourcing.EntityFrameworkCore.Sqlite
- OpenEventSourcing.EntityFrameworkCore.InMemory (For testing only)

via package manager

```
Install-Package OpenEventSourcing.EntityFrameworkCore.SqlServer
```

or via .NET CLI

```
dotnet add package OpenEventSourcing.EntityFrameworkCore.SqlServer
```

### Message bus provider

In order to use the message/event bus functionality you will need to install a message bus provider package.

The current supported providers are:

- OpenEventSourcing.Azure.ServiceBus

via package manager

```
Install-Package OpenEventSourcing.Azure.ServiceBus
```

or via .NET CLI

```
dotnet add package OpenEventSourcing.Azure.ServiceBus
```

## Configuration

Within the `ConfigureServices` method of `Startup.cs` ([WebHost](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host) e.g. ASP.NET Core) or `ConfigureServices` on the `HostBuilder` when using the [generic host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host):

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddOpenEventSourcing()
            .AddEntityFrameworkCoreSqlServer()
            .AddCommands()
            .AddEvents()
            .AddQueries()
            .AddJsonSerializers();
}
```

There are several configurations and combinations, for more information see the [wiki](wiki)

## Docs

The documentation can be found on the [wiki](wiki).



