# Multi User Space Server

## Introduction to MUS

Multi User Space (MUS) is a game project that delivers a MMO-like text-adventure solution.
The game is meant to run in a single centralized system / game server.
Players can then connect to the game with client applications and input their actions with text commands.

This repository is a game server, and does not provide a compatible client application.
To see a compatible client application, check out [TIB](https://github.com/keinaju/TIB).

This solution is implemented with ASP.NET Core.
The application needs a database to save users and game state, so EntityFramework Core was chosen as a O/RM solution.
Current implementation uses Pomelo.EntityFrameworkCore.MySql Nuget-package for MySQL connection.
However, the database could as well be any other SQL-implementation.

The game application is meant to provide full behavior and zero content.
The application creates one initial user and room to provide a starting state to build on.
After this, content of the game is up to the users, as they are expected to build the game during runtime.
Users can create room modules inside the game which represent the world inside the game.
This structure is very modular and supports various kinds of implementations, which are up to the users and their creative capabilites.
This is a one-of-a-kind game concept, which was the main motivation for creating this solution.

## Instructions

See the game manual at [Kingdom -game](https://github.com/keinaju/Kingdom/blob/master/manual/manual.txt).
