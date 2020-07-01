I started making a neopets bot in my spare time and i've also decided to make it open-source for anyone that wants to fork it or just read in general or join in the fun!

My neopets pal:
Written in C# - Anthony reeder
https://github.com/A...er/MyNeopetsPal

Intro:

This isn't necessarily the best code and I've cut alot of corners but it's fairly clean and so far proven to be fairly robust. There is alot of reusable code that i plan on tidying up later. At this point I really just wanted to get something functional and profitable (as i'm poor in neopets).

Current functionality as of 01/07/2020:
- Runs multiple accounts simultaneity using different proxies
- Buying snowballs every 30minutes
- Does trudy once a day (after 10AM local time due to time differences)
- Buys desert scratchcards every 4 hours.

Screenshot:
https://imgur.com/a/9Syjftv

Simple explanation:
To run is very simple, make sure all your accounts and proxies are in the database generated when you run the code for the first time. This can be done via Sqlite (I'll add built-in functions for this later).

Press Start and off it goes. The bot will fire up a new thread for each user, who will then be assigned a manager and timer which will raise a timed event every 1-20minutes (each user is random). Every timed event the timer will be reset and the Sqlitedatabase will be checked to see if we can perform any activity. Only 1 activity is performed per cycle per user.

As this is all data driven (almost) this allows the program to expand for as many activities/dailies as we would like without having to really add much code or impeding pc performance. An extra bonus in using data is that we arn't sending any unnecessary requests to the server to check if an activity is ready. This enables us to check as often as we would like with 0 added risk to getting banned/frozen. The basis of the modmanager is built around the data. Currently i'm adding separate logic for each daily activity though this will be changed later to make it far more dynamic (so that no coding is really required to add new dailies/training/whatever).
It will essentially allow even someone with no programming background to pick any-part of neopets and automate it as part of this program for all of their accounts.


Detailed explantion (The code):
Coming soon...