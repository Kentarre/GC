# GC

Realtime browser-based math game for up to 10 concurrent users. The game is structured as a continuous series of rounds, where all 
connected players compete to submit the correct answer first. All players are presented with the challenge and have to answer whether 
the proposed answer is correct using a simple yes/no choice. The first player to submit a correct answer gets 1 point for the round and completes the round.
A new round starts in 5 seconds after the end of last one.

## Server
F5 to run application in Visual Studio

## SignalR configuration
Copy server application url from browser and replace it in App.js of web project on line 16. It should be place where SignalR hub connection is configured. 
In the end you should get url e.g. 'https://localhost:portnumber/game'

## Client 
Go to web project folder 'GC.web' and open console, 'npm install' and 'npm run' to start React web server

## Tech-stack 
```.NET Core 3.1, SignalR, ReactJS, Bootstrap```
