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
                    // TODO: CLICK BEHAVIOUR (Event handling)
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
             * DEBUG : Cheating at Minesweeper (comment out for a fair game)
            */
            foreach (Button b in buttonList)
            {
                // If the corresponding tile is a mine
                if (tileList.ElementAt(buttonList.IndexOf(b)).IsMine)
                {
                    // Highlight it in red
                    b.Background = new SolidColorBrush(Colors.Red);
                }
            }
            /**/
        }
        #endregion
    }
}