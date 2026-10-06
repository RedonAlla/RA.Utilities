---
title: Installation
sidebar_position: 2
---

# Installation

**Prerequisites**
Before beginning, ensure these tools are ready:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or newer
- [Docker Desktop](https://www.docker.com/products/docker-desktop) or [Podman](https://podman.io/) — needed only if you run SQL Server, PostgreSQL, or Oracle in containers. SQLite works without containers.

Check your current setup via terminal:

```bash
dotnet --version
```

##Install the template

- **Local installation (from source):**
Clone the repository and run the following command from the root directory:

```bash
dotnet new install .
```

- **NuGet installation:**
Once published, install the package globally through the command line interface:

```bash
dotnet new install RA.CleanArchitecture.Template
```

This setup runs once per machine.

## Update the template
Refresh existing packages with:

```bash
dotnet new update
```

To modify a single item:

```bash
dotnet new update RA.CleanArchitecture.Template
```

## Uninstall
Remove the entry using:

```bash
dotnet new uninstall RA.CleanArchitecture.Template
```