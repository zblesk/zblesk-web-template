# Vue 3 + .NET 6 + Bulma + others

This is a template/sample/skeleton of an application that bundles what I wanted to use as a basis for a few web apps. As is often the case with modern web development, getting all of the components to play together can be a big hassle, if you're not using a stack that's directly supported by first-party toolchains. When I finally got it the way I wanted it, to build the [Obskurnee book club app](https://zblesk.net/blog/tag/obskurnee-book-club/), I thought I might as well extract the base from it and re-use it. 

**This is not a complete/polished app, and it won't be. It's just the desired components set up and wired to be used together.** Most (but not all) of the features are demonstrated in the code in one way or another. You could either fork the code and start modifying it, or just copy-paste pieces of config if you're trying to set up a stack of your own. 

A lot of the heavy lifting was done by others: I have relied heavily on [VueCliMiddleware for dotnet](https://github.com/EEParker/aspnetcore-vueclimiddleware) and Alexandre Malavasi's work. 

By default, the app runs on SQLite with Entity Framework Core. EF makes it really easy to switch out the database backend - I've used this with MariaDB and Postgres and in both cases I only needed to reference the appropriate Nuget, configure the connection in the `Setup` class and everything else worked automatically.

[![Build Status](https://bzzz.zble.sk/api/badges/zblesk/zblesk-web-template/status.svg)](https://bzzz.zble.sk/zblesk/zblesk-web-template)

# The stack

These parts are already set up and should just work: 

## Frontend

- **Vue 3** for the frontend
  - With **Vuex**
  - Routing with **vue-router**
  - Localization with **vue-i18n**
  - **Single-file components**
  - ... with **Sass**

- **Mitt** as an event bus (mostly for notifications)
- **vue-toaster** for displaying toast notifications
- **Bulma** styles **with customization**
- **SignalR** for real-time, bi-directional client-server communication
- The obvious **Axios** for requests

## Backend

- **.NET 6**
  - With the **hierarchical config** set up
  - and dependency injection, attribute routing, attribute auth 
  - **Localization** of server-side strings
- **Entity Framework Core 6** for the data layer
  - With **automatic migrations** on app startup
- **Authentication**
  - Works with **claims, roles, and role-claim assignments**.  
  - An example of **resource-based authorization + related policy** also available
  - With **JWT**
  - SignalR hub also authenticated
  - **No cookies.** Tried that first, couldn't make it work with Axios and SignalR 
  - Hint: The first user to register will be an Admin ℹ
  - So you don't have issues with creating your first user, the min password length has been set to 1. I suggest changing it to at least 13 in `appsettings.json`. ⚠

- **SQLite** by default; easy to switch to other DBs supported by EF
- **Serilog** for logging
  - With **request logging enrichments**

- **VueCliMiddleware** for Vue support
- **NewtonsoftJson** for handling the response deserialization, as System.Text.Json has issues with possible circular refs
- The excellent **Markdig** for Markdown rendering support; it's also easy to modify the generator. It's not a part of this template, but **[here](https://zblesk.net/blog/adding-spoiler-support-to-markdown-with-markdig/)** is how to add 'spoiler' support.

## Integrations & the rest

- Basic **Matrix** connection - made for sending notification to a room
- **Mailgun mailer** 
  - with a fake mailer service that's turned on in the config by default - which just logs any sent emails to the log sink 
- **Backup service** that periodically make a snapshot of your SQLite DB
- **Docker** file included. Based on **Alpine**. Build and run easily.
- **Drone** config also included; just fill in the credentials and Drone will build and publish to Docker Hub. Look, it even has a badge. [![Build Status](https://bzzz.zble.sk/api/badges/zblesk/zblesk-web-template/status.svg)](https://bzzz.zble.sk/zblesk/zblesk-web-template)

# Building

It's a standard .net project. If you open it in Visual Studio 2022, all you need to do should be to hit 'run'. 

In case of issues, you can go to `zblesk-web\ClientApp`, run `npm install`, then try the dotnet build again.

## Docker

For Docker, just `docker build -t zbleskwebtemplate .`

To run it, `docker run --rm -p 8080:8080 zbleskwebtemplate`