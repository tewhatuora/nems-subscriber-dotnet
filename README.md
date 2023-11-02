# Solace QueueConsumer DotNet Demo

A simple Solace queue consumer written using .NET 7.

### Requirements
Docker

### Installation Instructions


1. Modify the `properties.json` file and set the Host, VPNName, UserName, Password, and QueueName settings.
2. Run `docker-compose up` in the root folder.

The application should install itself and start up. Once started it will connect to the Solace instance and start listening on the specified queue. Once a message is sent to the queue the consumer will pick it up and should display the message contents to the terminal.