# NEMS Guaranteed Subscriber .NET

This is a consumer application designed to help subscribers connect to the NEMS broker. This consumer application has been written in .NET 8. This project is an evolving project with new capability being added as new requirements are identified.

The project has been structured in a way where subscribers add their customer code to the `EventLoader.cs` program. All other programs will be managed by the NEMS team.

#### NOTE:

The program has been desigend to acknowledge all messages. If a subscriber wants to drop a message, don't thow an exception, supress the processing of the message is acknowledged successfully. If the message fails due to a system fault, if that fault hadn't occured it would have processed successfully, then an exception should be raised. The exception will cause the acknowledgement to fail and the message will remain on the queue for future processing.

### Installation Instructions (Docker)

#### Requirements:

- Docker

#### Installation:

1. Clone this repo into a folder of your choice.
2. Open the `source/properties.json` file. Replace the PLACEHOLDER variables to meet your connection details.
3. Run `docker-compose up --build` in the root folder.

The application should install itself and start up. Once started it will connect to the Solace instance and start listening on the specified queue. Once a message is sent to the queue the consumer will pick it up and should display the message contents to the terminal.

### Installation Instructions (Local)

#### Requirements:

- .NET Version 8.0
- IDE capable of running .NET 8.0

#### Installation:

1.  Clone this repo into a folder of your choice.
2.  Move the `properties.json` into the source folder
3.  Open the `source/properties.json` file. Replace the PLACEHOLDER variables to meet your connection details.
4.  To run the application, make sure you are in the `source` directory run the command

    `dotnet run`
