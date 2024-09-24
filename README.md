## Calendar

Calendar client-server application with CLI.

### Table of Contents

- Introduction
- Getting Started
- Available Commands
- Examples
- Troubleshooting
- Additional Resources



### Introduction

Welcome to the Calendar Console Application! Calendar is a command-line tool that allows you to create and manage planned events. This document provides an overview of the application's features, available commands, and usage instructions.



### Getting Started

In repository are available Visual Studio solutions for both client("src/client/calendar_client.sln") and server("src/server/calendar_server.sln") projects, from which the executables can be build.

### Available Commands

The Calendar Console Application supports various commands for managing user accounts and events . Below is a list of available commands:

Non-logged-in:
- `help`: Shows the help information for available commands.
- `createUser`: Creates a new user in the calendar system.
- `login:` Logs the user into the calendar system.
- `exit`: Exits the application.

Logged-in:
- `help`: Shows the help information for available commands.
- `logout`: Logs the user out of the calendar system.
- `changeName`: Changes the name of the current user.
- `changePassword`: Changes the password of the current user.
- `add`: Adds a new event to the calendar.
- `edit [id]`: Edits the event with the specified ID.
- `duplicate [id]`: Duplicates the event with the specified ID.
- `delete [id]`: Deletes the event with the specified ID.
- `show [id]`: Displays the details of the event with the specified ID.
- `list [date]`: Lists the events in the user's calendar.
- `next`: Shows the events for the next week/month.
- `previous`: Shows the events for the previous week/month.
- `current`: Shows the events for the current week/month.
- `view [time span]`: Views the events for a specified time span (week, month, or upcoming).
- `exit`: Exits the application.

For entering commands is available tab completion and navigating through history with UpArrow and DownArrow

### Examples
Here are some examples of how to use the Calendar Console Application:

Add new event:
`login`
After entering command the dialog starts:
```
Name:
user
Password:
...
```


### Troubleshooting

If you encounter any issues while using the Calendar Console Application, consider the following tips:

- Double-check the command syntax and arguments.
- Ensure that connection to server is available.


### Additional Resources
For more information and assistance, you can refer to the following resources:

Documentation: [Full Documentation](https://jevcakj.github.io/csharp-calendar/)
Contact Support: [Mail](janjevca@seznam.cz)