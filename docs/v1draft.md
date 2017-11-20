# namu 1.0
## Declaration

### a, an operator
#### Value type
When the `a` or `an` operator is used with the value type, it works like default keyword.

```
i is an integer.
var i = default(int);
```

```
num is a number.
var num = default(double);
```

```
name is a text.
var name = default(string);
```

#### Reference type
```
player is a MusicPlayer.
var player = new MusicPlayer();
```

```
array is 1, 2, 3, 4.
var array = new[] { 1, 2, 3, 4 };
```

```
players are 10 MusicPlayers.
var players = new MusicPlayer[10]; 
```

### implicit type
```
name is "John". 
var name = "John";
```

## Access to member
```
player's Name is "Mike".
player.Name = "Mike";
```

```
player, Play the song.
player.Play(song)
```

## Operator overload

```
add 10 to player's Count.
player.Count += 10;
```

```
add " Johnson" to player's Name.
player.Name += " Johnson";
```

## Acess to static member 

```
Console, Write "Hello world!".
Console.Write("Hello World!");
```

## if 

```
if the i is 0
if the i equals 0
if (i == 0)
```

```
if the player does not exist
if there is no player
if the player is empty
if (player == null)
```

```
if the player is MusicPlayer
if (player is MusicPlayer)
```

##
```
while the snake's length equals 0  
```

## for

```
for 10 times
for (int i = 0; i < 10; i++)
```

```
// not defined
foreach (var item in collection)
```

## when
```
when box is opened
    Console, Write Line "box is opened!".

box.Opened += () => 
{
    Console.WriteLine("box is opened!");
};
```