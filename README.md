# StockX-Discord-Bot
.NET core application to add a StockX price finding bot to your discord server.

## Configuration
```C#
//Insert your bots token here
string token = "";
```
In the program.cs file, enter the 'Client Secret' token for your bot from the discord developer portal.

## Usage
Once the bot is up and running, there are several commands available.

1. !stockx help - This will provide a guide on all available commands.
2. !stockx <itemname> - This will output the highest bid and lowest ask for all of the sizes
3. !stockx <itemname>, <US shoe size> - This will output the highest bid, lowest ask and total payout for the given size.

