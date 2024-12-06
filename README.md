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
show user
new being
select <beingname>
new room
```

This would add a new room to the game world, which would automatically be connected to the inital room created in startup process.

## Game concepts

### Rooms

Room is the basic building block of the application.
Command to create a new room:

```
new room
```

This command creates a new room, and automatically connects it bi-directionally to the room where user's currently selected being was.
To move the being into another room:

```
go <roomname>
```

To look for details in a room:

```
look
```
See more detailed commands in All Commands -section.

### Beings

The users can create characters inside the game called 'beings'.
Command to create a new being:

```
new being
```

Each user can have at most one selected being. Command to select a being from all available beings per user:

```
select <beingname>
```

### Items

Items are generated with item hatchers.
Command to create a new item:

```
new item
```

Command to create a new item hatcher:

```
new <itemname> item hatcher
```

### Room pools and curiosities

In addition to connections to other rooms, rooms can contain 'curiosities', which open connections to more rooms.
Players are free to explore these curiosities, which will generate more rooms on-demand-basis.
New rooms are generated from a room pool, which consists of prototypes to use for cloning new rooms.
Command to explore a room:

```
explore
```

Command to create a new room pool:

```
new room pool
```

Command to add rooms to a room pool:

```
add <roomname> in room pool <roompoolname>
```

The curiosity inside the room can refer to any single room pool.
This allows the game world to extend itself.

### All commands

```
Adds a feature in the current being. ^add feature (.+) in being$

Adds a feature to the current room that a being must have to enter. ^add feature (.+) in room$

Adds items in a craft plan. ^add (\d+) (.+) in craft plan (.+)$

Adds a room in a room pool. ^add (.+) in room pool (.+)$

Breaks an item into components, or crafts an item from components. ^(break|craft) (.+)$

Converts an item into a being. ^deploy (.+)$

Explores a curiosity in the current room, possibly revealing more rooms. ^explore$

Moves the current being in a new room. ^go (global |connected |being )?(.+)$

Moves the current being out of an inside room. ^leave$

Grants items to the current being. ^grant (\d+) item (.+)$

Shows an introduction. ^(\s*|help)$

Requests a login token from server to establish a session. ^login (.+) (.+)$

Looks at the current room. ^look$

Creates a new being. ^new being$

Creates a new plan for crafting an item. ^new (.+) craft plan$

Creates a new deployment to convert an item into the current being. ^new deploy (.+)$

Creates a new feature. ^new feature (.+)$

Creates a new item hatcher that generates items into inventories. ^new (.+) item hatcher$

Creates a new item. ^new item$

Creates a new room and connects it to the current room. ^new room$

Creates a new room pool to generate cloned rooms. ^new room pool$

Shows all items in the current being's inventory. ^my$

Selects a being to control. ^select (.+)$

Creates an offer to sell items for other items. ^sell (\d+) (.+) for (\d+) (.+)$

Sets the current being name. ^set being name (.+)$

Sets a room pool to use to generate cloned rooms in the current room. ^set curiosity (.+)$

Sets a tick interval for an item hatcher. ^set (.+) item hatcher interval (\d+)$

Sets a minimum and maximum quantity of items to generate for an item hatcher. ^set (.+) item hatcher quantity (\d+) to (\d+)$

Sets a description for an item. ^set item (.+) description (.+)$

Sets a name for an item. ^set item (.+) name (.+)$

Sets a description for a room. ^set room description (.+)$

Sets a setting for the current room to determine if it can be entered from anywhere. ^set room global access (true|false)$

Sets a room that resides inside the current being. ^set inside (.+)$

Sets a name for the current room. ^set room name (.+)$

Sets a description for a room pool. ^set room pool (.+) description (.+)$

Sets a name for a room pool. ^set room pool (.+) name (.+)$

Sets an item that is required to explore a room pool. ^set room pool (.+) required item (.+)$

Shows all commands in the application. ^show commands$

Shows all features. ^show features$

Shows all rooms that can be accessed from anywhere. ^show global rooms$

Shows an item and it's details. ^show item (.+)$

Shows all items ^show items$

Shows all item hatchers the current room has subscribed to. ^show hatchers in room$

Shows offers. Pass * to show all offers. Pass an item name to show only offers that sell those items. ^show (.*) offers$

Shows all room pools. ^show room pools$

Shows information for the currently logged in user. ^show user$

Creates a user for the given username and password. ^signup (.+) (.+)$

Takes items from the current room's inventory. ^take (\d+) (.+)$

Shows the tick count. ^time$
```
