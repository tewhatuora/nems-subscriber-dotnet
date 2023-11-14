# NEMS Guaranteed Subscriber .NET

This is a simple NEMS queue consumer console app written using .NET 7. If an error occurs the stacktrace will be printed to the console.

### Installation Instructions (Docker)

#### Requirements:
 - Docker

#### Installation:
1. Clone this repo into a folder of your choice.
2. Modify the `properties.json` file and set the Host, VPNName, UserName, Password, and QueueName settings.
3. Run `docker-compose up --build` in the root folder.

The application should install itself and start up. Once started it will connect to the Solace instance and start listening on the specified queue. Once a message is sent to the queue the consumer will pick it up and should display the message contents to the terminal.

### Installation Instructions (Visual Studio)
#### Requirements:
 - .NET Version 7.0
 - Visual Studio 2022

 #### Installation:
 1. Clone this repo into a folder of your choice.
 2. Copy the ```requirements.json``` into the source folder 
 3. Open the .sln file with Visual Studio
 4. Select the ```source/requirements.json``` file and under the Properties panel set ```Copy to Output``` to ```Copy if newer```
 5. Open the ```source/requirements.json``` file and set the Host, VPNName, UserName, Password, and QueueName settings
