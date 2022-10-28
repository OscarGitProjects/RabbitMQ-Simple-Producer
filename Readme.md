﻿This is a implementation of a message sender/producer to a queue in RabbitMQ.
In this version i have implemented queue with messages, direct exchange queue, topic exchange queue, header exchange queue and fanout exchange queue
You use this together with the sender/producer in the project RabbitMQ-Simple-Consumer.

First you run the message consumer. The RabbitMQ-Simple-Consumer project. In the menu you choose what kind of queue you want to use.
Then you run the producer of messages. The RabbitMQ-Simple-Producer project. In the menu you choose what kind of queue you want to send the messages. 
The choices in the differen applications have to be the same. Then you should see messages from the producer listed in the console.

This application will send 25 message to a queue in RabbitMQ.
You use this together with the receiver/consumer of the messages in  the project RabbitMQ-Simple-Consumer.
You can run more then one receiver/consumer.


RabbitMQ is a open source message broker. https://www.rabbitmq.com/

RabbitMQ docker image https://registry.hub.docker.com/_/rabbitmq/


When i run this i have RabbitMQ running in a Docker container.

You can get the docker image with this command.
docker run -it --rm --name Rabbit1 -p 5672:5672 -p 15672:15672 rabbitmq:3.10-management

Portnummer 5672 is where RabbitMQ is running.
Portnumber 15672 is for the RabbitMQ Management webpage.
Default username and password is guest.

After you have installed the docker image for the first time you can remove --rm from the command