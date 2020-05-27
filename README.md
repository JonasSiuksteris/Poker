# Poker Site
Multiplayer web poker game using Blazor webassembly, EF, SignalR

## Technologies being used

Front-End - Blazor WebAssembly

Back-End - ASP.NET Core with EF and SignalR

## Plans for Iterations

### First iteration

1. Player can connect to their account.
2. Players can connect to one table and see each other.

### Second iteration

1. Players can participate in a full game of texas holdem poker.
2. Players can bet virtual currency(no winnings).
3. Players hands are evaluated with hand strength evaluator.

### Third iteration

1. Winnings are distributed among participating player according to poker rules.
2. Admin can control table parameters, such as player amount, small bet.

### Fourth iteration

1. Players in a room can communicate with each other
2. Players can note usefull information about opponents

## Iterations progress

### First iteration

1. Player can connect to their account.

Using Identity from EF and AuthenticationStateProvider from .NET Core players are able
to connect to their accounts with default group being User.

2. Players can connect to one table and see each other.

Using EF new table of tables was added to database. Each user can see all tables that are
created and join them. After joining they are connected to SignalR hub table group.

### Second iteration

1. Players can participate in a full game of texas holdem poker.

Poker game engine is created in SignalR Hub. When player joins instance is created till there are no connected players left.

2. Players can bet virtual currency(no winnings).

When players join game they have to select amount that they want to deposit to the game. After that they can raise/call with their deposited money.

3. Players hands are evaluated with hand strength evaluator.

Evaluator gets 7 cards from community table and from player hand after that it evaluates hand from high(royal flush) to low(high card) using linq statments.

### Third iteration

1. Winnings are distributed among participating player according to poker rules.

After every round of a game pots are created with participating players. At the end of the game pots winners are decided and money distributed.

2. Admin can control table parameters, such as player amount, small bet.

Added additional parameters to tables database table. Admin can modify them in admin control panel

### Fourth iteration

1. Players in a room can communicate with each other

Players that are connected to table SignalR hub can notify each other with their messages. Also game engine can send information to players.

2. Players can note usefull information about opponents

New table is created in database to store information about players. It has one to many relationship with IdentityUser table.
Players can modify text when they see the players playing in the same table they are.


