﻿This is a implementation of a message sender/producer to a queue in RabbtiMQ.

RabbitMQ is a open source message broker. https://www.rabbitmq.com/
RabbitMQ docker image https://registry.hub.docker.com/_/rabbitmq/

This application will send 100 message to queue in RabbitMQ with the name of simpleMessage
You use this together with the receiver/consumer of the messages in  the project RabbitMQ-Simple-Consumer.
You can run more then one receiver/consumer.


When i run this i have RabbitMQ running in a Docker container

You can get the docker image with this command
docker run -it --rm --name Rabbit1 -p 5672:5672 -p 15672:15672 rabbitmq:3.10-management

Portnummer 5672 is where RabbitMQ is running
Portnumber 15672 is for the RabbitMQ Management webpage
Default username and password is guest

After you have installed the docker image for the first time you can remove --rm from the command