# Basic Redis Chat App Demo

Showcases how to impliment chat app in ASP.NET Core, SignalR and Redis.

## Try it out

#### Deploy to Heroku

<p>
    <a href="https://heroku.com/deploy" target="_blank">
        <img src="https://www.herokucdn.com/deploy/button.svg" alt="Deploy to Heorku" />
    </a>
</p>

#### Deploy to Google Cloud

<p>
    <a href="https://deploy.cloud.run" target="_blank">
        <img src="https://deploy.cloud.run/button.svg" alt="Run on Google Cloud" width="150px"/>
    </a>
</p>

## How it works?

![How it works](docs/screenshot001.png)

## How to run it locally?

#### Run frontend

```sh
cd client
yarn install
yarn start
```

#### Run backend

```sh
dotnet restore
dotnet build
dotnet run
```
