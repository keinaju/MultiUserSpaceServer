# Multi User Space Server

## Introduction to MUS

Multi User Space (MUS) is a game project that delivers a MMO-like text-adventure solution.
The game is meant to run in a single centralized system / game server.
Players can then connect to the game with client applications and input their actions with text commands.

This repository is a prototype for a game server, and does not provide a compatible client application.
Solution is implemented with ASP.NET Core.
The application needs a database to save users and game state, so EntityFramework Core was chosen as a O/RM solution.
Current implementation uses Pomelo.EntityFrameworkCore.MySql Nuget-package for MySQL connection.
However, the database could be any other SQL-implementation.

The game application is meant to provide full behavior and zero content.
The application creates one initial user and room to provide a starting state to build on.
After this, content of the game is up to the users, as they are expected to build the game during runtime.
Users can create rooms inside the game which represent the world inside the game.
This structure is very modular and supports various kinds of implementations, which are up to the users and their creative capabilites.
This is a one-of-a-kind game concept, which was the main motivation for creating this solution.

## Example commands

Example of first commands would be:
```
signup <user> <password>
login <user> <password>
user
new being
pick <beingname>
new room
```

This would effectively add a new room to the game world, which would automatically be connected to the inital room created in startup process.

## Game concepts

### Rooms

Room is the basic building block of MUS application.
Command to create a new room:
```
new room
```
This command creates a new room, and automatically connects it bi-directionally to the room where user's currently picked being was.
To move the being into another room:
```
go to <roomname>
```
To look for details in a room:
```
look
```

### Beings

The users can create characters inside the game called 'beings'.
Command to create a new being:
```
new being
```
Each user can have at most one selected being. Command to pick a being from all available beings per user:
```
pick <beingname>
```

### Items

Items are generated with item generators.
Command to create a new item:
```
new item
```
Command to create a new item generator:
```
new <itemname> item generator
```

### Room pools and obscurities

In addition to connections to other rooms, rooms can contain 'obscurities', which open connections to more rooms.
Players are free to explore these obscurities, which will generate more rooms on-demand-basis.
Command to explore a room:
```
explore
```

The previously created rooms can be registered in user defined room pools.
Command to create a new room pool:
```
new room pool
```
Command to add rooms to a room pool:
```
add <roomname> in room pool <roompoolname>
```
Room pools provide a list of prototype rooms that can be used to generate more of them.
The obscurity inside the room can refer to any single room pool.
This allows the game world to extend itself.
