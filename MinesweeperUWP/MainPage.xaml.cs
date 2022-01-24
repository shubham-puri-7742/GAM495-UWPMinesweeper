using MinesweeperUWP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MinesweeperUWP
{
    public sealed partial class MainPage : Page
    {
        // Game grid: A list of tiles with clickable buttons
        private List<Tile> tileList = new List<Tile>();
        private List<Button> buttonList = new List<Button>();
        // Solid colours for the tile states
        private SolidColorBrush lightGrey = new SolidColorBrush(Colors.LightGray);
        private SolidColorBrush lightSlateGrey = new SolidColorBrush(Colors.LightSlateGray);
        private SolidColorBrush black = new SolidColorBrush(Colors.Black);

        // Row, column, and mine counts
        private int rows = 0;
        private int cols = 0;
        private int mineTotal = 0; // This is set at the start of the game and never changed again
        private int mines = 0; // Basically 'mines left unflagged', so always <= mineTotal

        // Game state and a bool to check if active
        private string gameState;
        private bool gameActive = true;

        public MainPage() => this.InitializeComponent();

        // (Re)Starts the game
        private void restartGame()
        {
            // Empty the game status (win/lose text), set the state to active
            statusTextBlock.Text = "";
            gameActive = true;
            // If any tiles exist, clear them
            if (tileList.Count() != 0)
            {
                tileList.Clear();
            }
            // If any buttons exist, clear them
            if (buttonList.Count() != 0)
            {
                buttonList.Clear();
            }
            // If any row and column definitions exist, clear them
            if (GameGrid.RowDefinitions.Count != 0 && GameGrid.ColumnDefinitions.Count != 0)
            {
                GameGrid.Children.Clear();
            }
        }

        private void easyFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            // 9 x 9, 10 mines
            rows = cols = 9;
            mines = mineTotal = 10;

            // Display the mine count
            mineCounterTextBlock.Text = $"{mines}";

            // Initialise game grid
            restartGame();
            tileCreator();
            gridCreator();
        }

        private void mediumFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            // 16 x 16, 40 mines
            rows = cols = 16;
            mines = mineTotal = 40;

            // Display the mine count
            mineCounterTextBlock.Text = $"{mines}";

            // Initialise game grid
            restartGame();
            tileCreator();
            gridCreator();
        }

        private void hardFlyoutItemClick(object sender, RoutedEventArgs e)
        {
            // 15 x 30, 99 mines
            rows = 16;
            cols = 30;
            mines = mineTotal = 99;

            // Display the mine count
            mineCounterTextBlock.Text = $"{mines}";

            // Initialise game grid
            restartGame();
            tileCreator();
            gridCreator();
        }

        #region Tile Creator
        private void tileCreator()
        {
            // Exactly what the names say
            populateTileList();
            createTileMines();
            createTileNeighbours();
        }

        private void populateTileList()
        {
            Tile t;
            // For each row
            for (int i = 0; i < rows; ++i)
            {
                // For each column
                for (int j = 0; j < cols; ++j)
                {
                    // Add a new tile (row, col) to the list of tiles
                    t = new Tile(i, j);
                    tileList.Add(t);
                }
            }
        }

        private void createTileMines()
        {
            Random rand = new Random(); // Initialise a random object

            // As long as the number of mines on the board is less than the total number of mines
            while (tileList.FindAll(x => x.IsMine).Count() < mineTotal)
            {
                // Get a random (valid) row and column for a mine...
                int mineRow = rand.Next(rows);
                int mineCol = rand.Next(cols);

                // ... And if there is not already a mine there,
                if (!(tileList.Find(x => x.Row == mineRow && x.Col == mineCol).IsMine))
                {
                    // Make it a mine tile
                    tileList.Find(x => x.Row == mineRow && x.Col == mineCol).IsMine = true;
                }
            }
        }

        private void createTileNeighbours()
        {
            Tile t;
            // For each row
            for (int i = 0; i < rows; ++i)
            {
                // For each column
                for (int j = 0; j < cols; ++j)
                {
                    // Get the tile at (i, j)
                    t = tileList.Find(x => x.Row == i && x.Col == j);

                    // Each condition is basically a bounds-check, so there are no neighbours that are out of bounds
                    // Inside each if statement, we add the neighbour as detailed in the corresponding comment
                    if (i + 1 < rows) // Below
                    {
                        t.Neighbours.Add(tileList.Find(x => x.Row == (i + 1) && x.Col == j));
                    }
                    if (i - 1 >= 0) // Above
                    {
                        t.Neighbours.Add(tileList.Find(x => x.Row == (i - 1) && x.Col == j));
                    }
                    if (j + 1 < cols) // Right
                    {
                        t.Neighbours.Add(tileList.Find(x => x.Row == i && x.Col == (j + 1)));
                    }
                    if (j - 1 >= 0) // Left
                    {
                        t.Neighbours.Add(tileList.Find(x => x.Row == i && x.Col == (j - 1)));
                    }
                    if (i + 1 < rows && j + 1 < cols) // Bottom-right
                    {
                        t.Neighbours.Add(tileList.Find(x => x.Row == (i + 1) && x.Col == (j + 1)));
                    }
                    if (i - 1 >= 0 && j + 1 < cols) // Top-right
                    {
                        t.Neighbours.Add(tileList.Find(x => x.Row == (i - 1) && x.Col == (j + 1)));
                    }
                    if (i + 1 < rows && j - 1 >= 0) // Bottom-left
                    {
                        t.Neighbours.Add(tileList.Find(x => x.Row == (i + 1) && x.Col == (j - 1)));
                    }
                    if (i - 1 >= 0 && j - 1 >= 0) // Top-left
                    {
                        t.Neighbours.Add(tileList.Find(x => x.Row == (i - 1) && x.Col == (j - 1)));
                    }
                }
            }
        }
        #endregion

        #region Button Creator
        private void buttonCreator()
        {
            // For each row
            for (int i = 0; i < rows; ++i)
            {
                // For each column
                for (int j = 0; j < cols; ++j)
                {
                    // Create a new button and bind two actions (LMB & RMB)
                    Button b = new Button();
                    b.Tapped += B_Tapped;
                    b.RightTapped += B_RightTapped;
                    // Add the button to the list of buttons
                    buttonList.Add(b);
                    // Make it a child of the game grid
                    GameGrid.Children.Add(b);
                    // Set its row and column as that of the corresponding tile, (i, j)
                    Grid.SetRow(b, i);
                    Grid.SetColumn(b, j);
                }
            }
        }
        #endregion

        #region Grid Creator
        private void gridCreator()
        {
            RowDefinition rowDef;
            ColumnDefinition colDef;

            for (int i = 0; i < rows; ++i)
            {
                // Create a new row definition for each row
                rowDef = new RowDefinition();
                GameGrid.RowDefinitions.Add(rowDef);
            }
            for (int j = 0; j < cols; ++j)
            {
                // Create a new column definition for each column
                colDef = new ColumnDefinition();
                GameGrid.ColumnDefinitions.Add(colDef);
            }

            // Create all buttons
            buttonCreator();

            /*
             * DEBUG : Cheating at Minesweeper
            
            foreach (Button b in buttonList)
            {
                // If the corresponding tile is a mine
                if (tileList.ElementAt(buttonList.IndexOf(b)).IsMine)
                {
                    // Highlight it in red
                    b.Background = new SolidColorBrush(Colors.Red);
                }
            }
            */
        }
        #endregion

        #region Events
        // Tap event (= LMB)
        private void B_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Get the index of the sender button and get the corresponding tile
            Button b = (Button)sender;
            int index = buttonList.IndexOf(b);
            Tile t = tileList.ElementAt(index);

            // If game is active AND the tile is not flagged, ambiguous, or already clicked
            if (gameActive && !(t.IsFlagged) && !(t.IsAmbiguous) && !(t.IsClicked))
            {
                // If the tile is not a mine
                if (!(t.IsMine))
                {
                    // Process a non-mine click
                    nonMineClick((Button)sender, t);
                }
                else // is a mine
                {
                    // Trigger the mine
                    mineClick((Button)sender, index);
                }
            }
        }
        // Right tap event (= RMB). We cycle through NOTHING -> FLAG -> AMBIGUOUS -> NOTHING
        private void B_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // Get the index of the sender button and get the corresponding tile
            Button b = (Button)sender;
            int index = buttonList.IndexOf(b);
            Tile t = tileList.ElementAt(index);
            
            // If the game is active and the tile is not clicked
            if (gameActive && !(t.IsClicked))
            {
                // If the tile is not flagged or ambiguous
                if (!(t.IsFlagged) && !(t.IsAmbiguous))
                {
                    // Flag it
                    b.Content = "F"; // Could use a flag image but sticking to text art here
                    t.IsFlagged = true;
                    // Tentatively count one mine found and update the mine count
                    --mines;
                    mineCounterTextBlock.Text = $"{mines}";
                }
                // If flagged but not ambiguous
                else if (t.IsFlagged && (!t.IsAmbiguous))
                {
                    // Mark ambiguous
                    b.Content = "?";
                    t.IsFlagged = false;
                    t.IsAmbiguous = true;
                    // Since the last state was flagged, we increment the mine count here
                    ++mines;
                    mineCounterTextBlock.Text = $"{mines}";
                }
                else // Back to unmarked
                {
                    // Clear the button content. Mark it unflagged and unambiguous
                    b.Content = "";
                    t.IsFlagged = false;
                    t.IsAmbiguous = false;
                }
            }

            // The part below augments a second RMB click under special conditions (see below) to act as a way to reveal ALL neighbours... At the slight risk of stepping on a mine
            // If clicked in an active game with no ambiguous neighbours, not a mine itself, but with some mines around, though none unflagged (@_@)
            if (t.IsClicked && t.Neighbours.Count(x => x.IsAmbiguous) == 0 && t.MinesAround != 0 && !t.IsMine && t.Neighbours.Count(x => x.IsFlagged) == t.MinesAround && gameActive)
            {
                // For each neighbour
                foreach (Tile tile in t.Neighbours)
                {
                    // If unflagged with some mines around
                    if (tile.MinesAround != 0 && !tile.IsFlagged)
                    {
                        // Grey it out, label it with the number of mine neighbours, and mark it clicked
                        buttonList.ElementAt(tileList.IndexOf(tile)).Background = lightGrey;
                        buttonList.ElementAt(tileList.IndexOf(tile)).Content = $"{tile.MinesAround}";
                        tile.IsClicked = true;
                    }
                    // If no mines around
                    else if (tile.MinesAround == 0)
                    {
                        // Simply grey it out
                        buttonList.ElementAt(tileList.IndexOf(tile)).Background = lightGrey;
                    }
                }
            }
            // If clicked in an active game with no ambiguous neighbours, not a mine itself, but with some mines around (@_@)
            else if (t.IsClicked && t.Neighbours.Count(x => x.IsAmbiguous) == 0 && t.MinesAround != 0 && !t.IsMine && gameActive)
            {
                // For each neighbour
                foreach (Tile tile in t.Neighbours)
                {
                    // If it is a mine
                    if (tile.IsMine)
                    {
                        // Trigger it
                        mineClick(buttonList.ElementAt(tileList.IndexOf(tile)), tileList.IndexOf(tile));
                    }
                }
            }
        }
        #endregion

        #region Tap logic
        // Flood fill logic
        private void floodFill(Tile tile)
        {
            // For each neighbour of a tile
            foreach (Tile t in tile.Neighbours)
            {
                // If valid (i.e. not out of bounds) and unclicked
                if (t.Row >= 0 && tile.Row < rows && tile.Col >= 0 && tile.Col < cols && !(t.IsClicked)) // Bounds checks
                {
                    // If with no mines around it
                    if (t.MinesAround == 0)
                    {
                        // Grey it out, mark it clicked, and recursively flood fill it
                        buttonList.ElementAt(tileList.IndexOf(t)).Background = lightGrey;
                        t.IsClicked = true; // Get here and it clicks that 'IsClicked' is technically a misnomer. IsRevealed may be more revealing of its true nature (see also the last if-block under nonMineClick)
                        floodFill(t);
                        // This recursion is what creates the effect of clearing blocks of empty tiles
                    }
                    // Condition harmless but really superfluous, because does anyone know what it means to have -2 mines around you?
                    else if (t.MinesAround > 0)
                    {
                        // Grey it out, label it with the number of mines in its neighbourhood, and mark it clicked
                        buttonList.ElementAt(tileList.IndexOf(t)).Background = lightGrey;
                        buttonList.ElementAt(tileList.IndexOf(t)).Content = $"{t.MinesAround}";
                        t.IsClicked = true;
                    }
                }
            }
        }

        // Check for victory
        private void winCheck()
        {
            // Set the victory state and end the game
            gameState = "Victory!";
            statusTextBlock.Text = gameState;
            gameActive = false;
        }

        // Handle clicks on non-mine tiles
        private void nonMineClick(Button button, Tile t)
        {
            // Grey the tile out
            button.Background = lightGrey;
            // If there are mines around it
            if (t.MinesAround != 0)
            {
                // Display the number of mines neighbouring it on the tile
                button.Content = $"{t.MinesAround}";
                // And mark it clicked
                t.IsClicked = true;
            }
            else if (t.MinesAround == 0) // Condition harmless but upon reflection, also not really necessary, because which tile in the world has -1 mines around it?
            {
                // See flood fill logic
                floodFill(t);
            }

            // If all the non-mine tiles have been clicked (or otherwise revealed - see flood fill logic)
            if (tileList.FindAll(x => x.IsClicked && !x.IsMine).Count() == (tileList.Count() - mineTotal))
            {
                // Game won
                winCheck();
            }
        }
        // Event for clicking (stepping) on mines
        private void mineClick(Button button, int index)
        {
            // Basically, reveal the entire minefield
            foreach (Button b in buttonList)
            {
                // Get the corresponding tile
                Tile t = tileList.ElementAt(buttonList.IndexOf(b));
                // If it is a mine
                if (t.IsMine)
                {
                    // Set its background to black
                    b.Background = black;
                }
                // Else if there is a mine around it
                else if (t.MinesAround != 0)
                {
                    // Give it a light grey colour and label it with the number of mines in its neighbours
                    b.Background = lightGrey;
                    b.Content = $"{t.MinesAround}";
                }
                else // Not a mine, no neighbours are mines
                {
                    // Just make it grey
                    b.Background = lightGrey;
                }
            }

            // End the game and display the end state
            gameState = "Game Over";
            statusTextBlock.Text = gameState;
            gameActive = false;
        }
        #endregion
    }
}