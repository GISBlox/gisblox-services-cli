# Introduction to GISBlox Services CLI
The GISBlox Services CLI is a standalone tool for interacting with the [GISBlox Services REST API](https://services.gisblox.com/). It enables no-code access to the API from the command-line. It will or already includes commands such as reprojecting coordinates and creating [GeoJson](https://en.wikipedia.org/wiki/GeoJSON). 

An active subscription to the GISBlox Services API is required in order to access the API. Create your free starter plan in the [GISBlox Account Center](https://account.gisblox.com/). 

## Prerequisites

In order to build and test the GISBlox Services CLI, you need to have the following installed on your machine:
- .NET 5
- Visual Studio / Visual Studio Code

## Building

Open the solution file `GISBlox.Services.CLI.sln` in Visual Studio and build it. Any NuGet dependency will be downloaded in the build process.

## Using the build output

The `gbs` executable in the `bin` directory can be run directly.

## Usage

The documentation on how to use the CLI can be found [here](https://github.com/GISBlox/gisblox-services-cli/blob/main/documentation/README.md).
