# GAM495-UWPMinesweeper
### C#-UWP Minesweeper App

Building on my IT230 work, which included an introduction to the Universal Windows Platform, I chose to work on implementing the logic for a classic game in C# using UWP for its visual interface.

Although UWP is not as commonly used for games, its use suited the nature of Minesweeper as a game that involves a grid of tiles with mainly two click functions - a layout that could be designed easily using XAML.

The main elements of the game logic include initialising a grid with random tiles as mines (while making sure no mine spawns out of bounds), handling left clicks (revealing either the number of mines in the immediate neighbourhood OR a block of tiles with a border with numbers indicating the number of mines in its neighbourhood, or all the mines after triggering a failure state), and right clicks (flagging potential mines, or marking ambiguous tiles).

Unlike the Poker project, the central part of this program is event-handling. Almost everything is decided by where the player clicks, including the initialisation of the game world (the minefield) on selecting the difficulty.

From a mathematical perspective, the fundamental problem in Minesweeper is detecting and counting neighbours. This is handled using LINQ queries using a function to check all the (8, unless at the edge of the world) adjoining elements based on their x and y values. If there are no mines in the neighbourhood, this process is repeated recursively to reveal a block of tiles.

Flagged and ambiguous tiles currently show up as 'F' (which, in retrospect, sort of resembles a flag) and '?', while the mines are hidden (red in debug mode which can only be activated from the source code) and simply black tiles when clicked; however, the exact way these tiles are displayed is independent of the game logic.

### Reflection

The first iteration of this project was just the XAML without the game logic. It initialised three minefields based on the selected difficulty and had a field for a victory/defeat text at the top (though it could not be displayed due to there being no game logic). The debug code to show the mines comes from this iteration, when randomisation was tested.

The second iteration added most of the core logic of left clicks, including revealing the number of adjacent mines, revealing tiles recursively, and ending the game in failure on clicking a mine. Bounds checking was the most important part here, because while most of the board has eight neighbours, that is not true of the tiles at the edges.

The right click logic (including disabling left clicks on flagged/ambiguous tiles) was added last.

Not counting sporadic typos or missed booleans in the if-statements with many conditions (subsequently fixed in testing), I did not encounter significant challenges in developing the project, except an odd error where the victory text didn't show up on the screen despite marking all the mines. Its cause remains unknown as it could not be replicated in subsequent playtests.
