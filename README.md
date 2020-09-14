# **Doctors Practice**
A simple .net Core Web API project designed to simulate scheduled appointments for a medical system. The project is focused on backend implementations. The goal is to create a project with a Domain Driven Design that works in High Availabiltiy mode.

# **Table of Contents**
1. [Features](#Features)
    1. [Web API](#Web-API)
    2. [Database](#Database)
    3. [Docker compose](#Docker)
    4. [Balance loading](#Balance-loading)
    5. [Message queuing service](#Message-queuing-service)
    6. [Email service](#Email-service)
    7. [Event sourcing](#Event-sourcing)
    8. [CQRS](#CQRS)
    9. [Security](#Security)
    10. [Unit testing](#Unit-testing)
    11. [Resilience](#Resilience)
# **Features**
## **Web API**

This feature is the core of the entire solution. The Web API is made functional with C#'s .NET core technology. The design of the project can be split up in the following functions: Models, Repositories, Middleware, Messaging Client,  Controllers, Advanced Requests & Transactions. It contains also a dockerfile for purposes of building an image of the project upon docker. Most of the advanced features such as messaging services and transactions are only developed within the Patients section of the project purely because implementation would be analog in the other sections and the main focus is introduction and learning technologies and learning industry relevant concepts.

### Models
The models are simple class representations of the tables within the database. Their properties correlate to the attributes of its respective table. The core classes are all inherited by a ReadClass which defines a constructor that decieves a datarow and creates a Patient with the specified data. Part of the models is also the DTO versions of each class which omit certain attributes of the table and serve as a bridge between the web service and database object.

Related classes: Patients, PatientDTO, Practices, PracticeDTO, Appointments, AppointmentDTO, Doctors, DoctorDTO

### Repositories
Communication with the database is structured to support the C# Repository pattern. Within connection to each table is defined by its own interface for which there can be specific implementations defined. In the case of the project the specific implementations are SQL versions that implement the interface methods and are responsible for handling communication with the database and transfering data between the project and the database. The repositories recieve the class DTO's and transfer the information within to the database or get data from the database and converts them into it's responsible class DTO. They do not use any form of ORM's and communicate directly with the database in the case of SQL repositories.

Related classes: IPatientRepositry, SQLPatientRepository, IPracticeRepository, SQLPracticeRepository, IDoctorRepository, SQLDoctorRepository, IAppointmentRepository, SQLAppointmentRepository

### Middleware
This part of the the project is responsible of logging all requests sent to the webapi. It is implemented via 2 classes: RequestResponseLoggingMiddleware and RequestResponseLoggingMiddlewareExtension. Extension serves purely as a bridge between the application builder that is summoned at startup and assigning it the adequate logging middleware which is definied in the RequestResponseLoggingMiddleware. Its primary job is simply to store the information of the requests inside of a logs.txt that is stored within the project.

Related classes: RequestResponseLoggingMiddleware, RequestResponseLoggingMiddlewareExtension

Technology: Microsoft.AspNetCore.Http, Microsoft.Extensions.Logging

### Messaging Client

Simple class and interface used for dependency injection. Uses Apache ActiveMQ NMS technology to connect to the AMQ server and depending on what is sent through the methods it can either send and read messages from a queue at a specific destination.

Related classes: AMQPatientClient, IPatientClient

Technology: Apache.NMS

### Controllers
Controllers are used to implement a REST design to the web api. Within the Controllers via dependency injection we insert the repository it is responsible for communicating, and a messaging service client so it can send and recieve messages from the server. The controllers enable RESTful design enabling one interface, in this case route, giving different answers depending on type of message that is sent to the route.

Related classes: PatientsController, PracticesController, DoctorsController, AppointmentsController

Technology: .NET Core

### Advanced requests
Currently only implemented within the Doctors section of the project it is able to process more advanced requests and return complex database models. In the project it currently can get all the patients of a specific doctor using a business layer class. The controller sends through the repositories to the business layer class and calls a method to get the patients for the doctor which then in turn returns a list of patients to the controller for it to output to the user.

Related classes: DoctorsController, DoctorLogic.

### Transactions

Since there are multiple services active within the solution the requirement of distibuted transactiosn becomes necessary to ensure ACID interaction with the database. This part of the code is part of the business layer. The PatientsController sends a request to Mediatr. Mediatr picks up this request and the entire transaction logic is actually handeled within the command handler itself. The idea is the handler tries to insert the patient information into the database, if it fails it terminates the call and returns internal server error, if it succeeds it tries to send a message to the queueing service, if the queuing service message fails the patient is removed from the database and the whole process is reverted. Every single step is verified and confirmed on an EventStore database saved as streams where each unique patient creates its own stream.

Related classes: PatientsController, CreatePatient and CreatePatientHandler

Related classes: 

## **Database**
The database is designed within Microsoft SQL Server 2019 hosted on an ubuntu operating system. It is hosted via a docker image and runs in a container. Itself consists of 4 simple tables which are as following.
> Table Appointment with the attributes: ID, Doctor_id, Patient_id, Reason, Secret

This table mostly serves as a helper table to connect a patient with a specific doctor since a patient can have multiple doctors at the same time and a doctor multiple patients. Doctor_id is a foreign key associated to the Doctor Table, while Patient_id is identical except connected to the Patient table.

>Table Doctor with the attributes:  ID, Name, Surname, Practice_id, Secret

Practice_id serves a foreign key connected to Practice table for associating the Doctor to his assigned practice.

>Table Patient with the attributes: ID, Name, Surname, Telephone, Secret

>Table Practice with the attributes: ID, Name, Address, Secret

All tables have the secret property for the purposes of simulating extra data that a web api would not recieve and work without for purposes of creating DTO objects. 

### **Data persistance**
For purposes of pertaining the data within the database between new docker containers there is a folder within the solution named SQLdata in which the database mdf files are stored and can be called upon when creating a new docker container.

## **Docker compose**
This feature is what allows other users to quickly pull and use a functioning version of the project remotely without the requirement of installing any support aside for docker. It sends a list of commands as to how to construct and run the project via docker. It also allows for any foreign party to just have this file to run the entire project without the need of pulling any other projects from any repositories.

To activate the docker-compose file via a bash console place yourself within the directory it is stored and call the following commpand:
```
docker-compose up
```
This command will pull all the required images and run containers for all of them functionally enabling the whole project.

If you desire to run multiple Web API's at once use the following command:
```
docker-compose up --scale web=X
```
Where X is the number of web services you desire to run at once.

To host multiple email services it is analog to the previous command but ***web*** is replaced with ***emailservice***

To run simultaneously multiple web api's and email services use the following command:
```
docker-compose up --scale web=X --scale emailservice=Y
```
Where X and Y are the number of the respected services you wish to run simultaneously.

### **DESCRIPTION OF DOCKER COMPOSE CONTENT**
### Web API
Pulls the latest image of the web api from the project that is located within a docker hub repository and exposes it to port 8000. Waits for the database and messaging server to build first.
### Database
Pulls a specific image of Microsoft SQL Server for Ubuntu, maps an interface for communication between the container and user. In this case it binds the port 1433 of the user to the containers port 1433. It creates an image with specific credentials for work with the project and maps where the Database data can be found.
### Balance loader
Pulls the latest image available for NGinx, maps the ports 4000 of the user to the port 4000 of the container and waits for the Web API to first be built. It also maps where to find the nginx.conf within the project and configures it correctly on the docker container. It also acts as an interface to the Identity Service on the port 4001.
### Message queuing service
Pulls an image of Apache's ActiveMQ messaging server and maps the required ports for communication between the user and container. It always waits for the AMQ .
### Email service
Like the Web API it pulls the latest image from the project that is located within a docker hub repository and is activated after the message queuing service is available.
### Identity service
Requests the latest image from docker, depends on the database to be executed first. Sets up the required ports for working with the rest of the solution.

## **Balance loading**
Balance loading multiple services to a single database via a single interface is done with the help of NGInx technology. It is configured to listen for requests sent from the Web API and via a round robin system communicates between the Web API's and single database. It's interface is accessed on port 4000.

## **Message queuing service**
The technology used here to enable the service is Apache's ActiveMQ NMS system which is a version of its ActiveMQ designed for .net core applications. It allows for producers and consumers to send and read messages via a dedicated message queuing service that they connect to that is hosted in this project via a docker image. Messages are sent into queues which enables for the services using the message queuing service to read messages even if the consumer wasn't online at the moment of the message being sent.

The added benefit of using Apache's ActiveMQ service is how it treats its queuing service. It is designed for microservice projects that are required to run in a high availabiltiy mode and hence automatically implements load balancing for multiple consumers at once via it's queues in the form of competitive consumer. What this means that only one consumer will ever recieve a message sent to the queue regardless of how many are listening.

## **Email service**
This service is its own seperate project which consists of a background worker and 2 classes for working with a message queuing service. The EmailWorker class extends the BackgroundService class to enable functionality as a background worker. Using dependency injection a messaging queuing service is added to the worker itself which it uses to connect to the message queuing service. Once connected it waits for a message to be sent with the content "PatientCreated" upon which it pretends to send an email in the form of logging that an email was sent. It also has a dockerfile created that will be used to automatically build a new image when comitted to github.

## **Event sourcing**
This concept essentially is the way all transactions are verified within the project. There is a simple eventstore database that stores all the events that happen on any one object within the project. It works by having a producer that can either create a stream or add to a stream new events and consumer which can actively wait and read for events on anyone stream if it needs some kind of confirmation. It also allows the implementations of projections for simple queries but those are not implemented in the current project. The project uses the IEventStore class which the EventStore class implements whose purpose is for the controllers to be able to connect to the Eventstore within which are methods to either create a consumer to read messages or a producer to upload information to the event store. It uses the EventStore nuget package and builds an image of EventStore to store all the information in its database in the form of simple JSON files. Persistency is enabled within the project.

## **CQRS**
Using the Mediatr nuget package this project segregates its commands and queries in the form of requests for which are in turn created handlers. The controllers simply just need to send the requests and the rest of the interaction is left to the handlers to manage. This allows for simple non-blocking asynchronous work of endpoints when a desired functionality is called.

## **Security**
Security in the project is enabled via an authentication and authorization system using OAuth technology to restrict access to the main Doctors_practice API. A seperate project named IdentityService with the help of IdentityServer4 package handles the authentication process in which it challenges the requests towards all the included clients and permitted users which are stored in a persistant MS SQL database. If the request is valid it will return an access token and if necessary an id token in a jwt format. If the request is invalid the caller will simply recieve a invalid_request response.

The IdentityService runs through an nginx interface on the port of 4001 which is than redirected internally to the actual project.

A sample of a permitted request is as following:
```
POST /connect/token HTTP/1.1
Host: localhost:4001
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials&scope=api1.read&client_id=oauthClient&client_secret=Secret
```

Furthermore the security implementation is not restricted to just IdentityService, the API it is protecting also has some authentication restrictions demanding that the token is valid and from a valid source.

Depending on the content of the token, particularly its scopes some parts of the API are restricted using Policy-based authorization checking if the correct scope is within the access token before letting a particular endpoint of the controller being called. 

A request that will work with the bearer token given in the prior example is as following:

```
GET /Patients HTTP/1.1
Host: localhost:4000
Authorization: Bearer (Replace with access token code)
```

## **Unit testing**
Testing is done within Doctors_practice.Test project in which the technologies of Xunit and Moq are used for testing specific parts of the project using cyclometric complexity as a method of covering bare minimum for the Customer class. Moq is used to mock required methods and objects without actually creating instances elsewhere i.e. it can pretend to create a database object and returns as per required for the method without actually creating an instance in the database and even connecting to it.

## **Resilience**
Resillience within the web api is secured using the Polly nuget package. Right now it only covers a porject excluded MediatR handler in which a timeout policy and retry policy are included to secure adequate response to possible problems and a method to recover from them.