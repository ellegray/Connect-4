using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dg2894_Connect4
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        MainWindow owner;

        //stuff for grid
        private static int ROWS = 6;
        private static int COLUMNS = 7;
        private int spotsLeft = COLUMNS * ROWS;
        Piece[,] playingGrid = new Piece[COLUMNS, ROWS];

        //stuff for player
        Boolean firstPlayer = true;
        Boolean computerPlayer;
        private int blueWins = 0;
        private int yellowWins = 0;
        private SolidColorBrush blueCircle = new SolidColorBrush(Colors.DarkCyan);
        private SolidColorBrush yellowCircle = new SolidColorBrush(Colors.Yellow);
        private SolidColorBrush blackCircle = new SolidColorBrush(Colors.Black);
        private Random random = new Random();
        private MediaElement backgroundMusic;

        public GameWindow(MainWindow myOwner, Boolean onePlayer)
        {
            InitializeComponent();

            //cue music!
            PlayMusic();

            owner = myOwner;

            //if it's single player or two player
            if (onePlayer)
            {
                computerPlayer = true;
            }
            else
                computerPlayer = false;

            //make background circles
            BackgroundGrid(COLUMNS, ROWS);

            //add player's colors to dictionary
            Piece.playerColor[true] = new SolidColorBrush(Colors.DarkCyan);
            Piece.playerColor[false] = new SolidColorBrush(Colors.Yellow);

            //show it's blue player's turn first
            BlueTurn.Fill = blueCircle;
        }

        private void BackgroundGrid(int X_SIZE, int Y_SIZE)
        {
            //create 7x6 background grid
            for (int y = 0; y < Y_SIZE; y++)
            {
                for (int x = 0; x < X_SIZE; x++)
                {
                    Ellipse e = new Ellipse();
                    e.Width = Piece.SIZE;
                    e.Height = Piece.SIZE;
                    e.Fill = new SolidColorBrush(Colors.AliceBlue);
                    myGrid.Children.Add(e);

                    e.HorizontalAlignment = HorizontalAlignment.Left;
                    e.VerticalAlignment = VerticalAlignment.Top;

                    Thickness bg = new Thickness(
                        Piece.OFFSET + x * (Piece.SIZE + Piece.SPACING),
                       105 + Piece.OFFSET + y * (Piece.SIZE + Piece.SPACING), 0, 0);
                    e.Margin = bg;
                }
            }
        }

        //added background music (creative commons)
        private void PlayMusic()
        {
            backgroundMusic = new MediaElement();
            backgroundMusic.Source = new Uri(@"..\..\Music\bgMusic.mp3", UriKind.Relative);
            backgroundMusic.Volume = 5;
            backgroundMusic.LoadedBehavior = MediaState.Manual;
            myGrid.Children.Add(backgroundMusic);
            backgroundMusic.Play();

        }

        private void DropPiece(int column, Boolean player)
        {
            int nullCount = 0;

            //find out how many spaces down the piece needs to drop
            for (int y = 0; y < ROWS; y++)
            {
                if (playingGrid[column, y] == null)
                {
                    nullCount++;
                }
            }

            //drop the piece
            if (nullCount != 0)
            {
                //create + place piece
                //pass in which column, how many spaces, which player
                Piece p = new Piece(column, nullCount, player);
                p.Placement(column, nullCount - 1);
                myGrid.Children.Add(p);
                //put it in the grid
                playingGrid[p.Column, p.Row] = p;
            }

            //subtract from spots left in the grid
            spotsLeft--;

            //if all spots are taken
            if (spotsLeft == 0)
            {
                MessageBox.Show("You both lose");
                StartOver();
            }

            //switch players
            SwitchPlayers();
            //look for winners
            WinnerDetection();

            if (computerPlayer && firstPlayer == false)
            {
                randomPlayer();
            }
        }

        //makes list of empty spaces and adds the columns to a list
        private List<int> GetEmptySpots()
        {
            List<int> empties = new List<int>();
            for (int y = 0; y < ROWS; y++)
            {
                for (int x = 0; x < COLUMNS; x++)
                {
                    if (playingGrid[x, y] == null)
                    {
                        int xPos = x;
                        empties.Add(xPos);
                    }
                }
            }

            return empties;
        }

        //places random piece
        private async void randomPlayer()
        {
            //place 1 piece
            int howMany = 1;
            //delay placing the piece until after human piece is placed
            await Task.Delay(500);
            List<int> emptySpots = GetEmptySpots();
            //place it a random column
            while (howMany > 0)
            {
                int theColumn = random.Next(0, emptySpots.Count);
                DropPiece(emptySpots[theColumn], firstPlayer);
                howMany--;
            }
        }

        private void SwitchPlayers()
        {
            //change to second player
            if (firstPlayer)
            {
                BlueTurn.Fill = blackCircle;
                YellowTurn.Fill = yellowCircle;
                firstPlayer = false;
            }
            //change to first player
            else if (firstPlayer == false)
            {
                BlueTurn.Fill = blueCircle;
                YellowTurn.Fill = blackCircle;
                firstPlayer = true;
            }
        }

        private void WinnerDetection()
        {
            //look for wins vertically, horizontally, diagonally
            VerticleWins();
            HorizontalWins();
            DiagnolWins();
        }

        private void VerticleWins()
        {
            //go through each column
            for (int x = 0; x < COLUMNS; x++)
            {
                //for each column, look down and see if there's 4 of the same color in a line
                for (int y = 0; y < ROWS - 3; y++)
                {
                    if (playingGrid[x, y] != null
                        && playingGrid[x, y].Color == playingGrid[x, y + 1].Color
                        && playingGrid[x, y].Color == playingGrid[x, y + 2].Color
                        && playingGrid[x, y].Color == playingGrid[x, y + 3].Color)
                    {
                        //if there is, show who won
                        TheWinnings();
                    }
                }
            }
        }

        private void HorizontalWins()
        {
            //go through each row
            for (int y = 0; y < ROWS; y++)
            {
                //for each row, look across and see if there's 4 of the same color in a line
                for (int x = 0; x < COLUMNS - 3; x++)
                {
                    if (playingGrid[x, y] != null && playingGrid[x + 1, y] != null
                        && playingGrid[x + 2, y] != null && playingGrid[x + 3, y] != null
                        && playingGrid[x, y].Color == playingGrid[x + 1, y].Color
                        && playingGrid[x, y].Color == playingGrid[x + 2, y].Color
                        && playingGrid[x, y].Color == playingGrid[x + 3, y].Color)
                    {
                        //if there's a win, show who won
                        TheWinnings();
                    }
                }
            }
        }

        private void DiagnolWins()
        {
            //high left to low right
            for (int x = 0; x < COLUMNS - 3; x++)
            {
                for (int y = 0; y < ROWS - 3; y++)
                {
                    //starting from top left, add 1 to column and row 
                    //and see if there's 4 of the same color in a line
                    if (playingGrid[x, y] != null && playingGrid[x + 1, y + 1] != null
                        && playingGrid[x + 2, y + 2] != null && playingGrid[x + 3, y + 3] != null
                        && playingGrid[x, y].Color == playingGrid[x + 1, y + 1].Color
                        && playingGrid[x, y].Color == playingGrid[x + 2, y + 2].Color
                        && playingGrid[x, y].Color == playingGrid[x + 3, y + 3].Color)
                    {
                        //if there's a win, show who won
                        TheWinnings();
                    }
                }
            }

            //low left to high right
            for (int x = 0; x < COLUMNS - 3; x++)
            {
                for (int y = ROWS - 1; y >= 3; y--)
                {
                    //starting from bottom left, add 1 to column and subtract 1 from row
                    //and see if there's 4 of the same color in a line
                    if (playingGrid[x, y] != null && playingGrid[x + 1, y - 1] != null
                        && playingGrid[x + 2, y - 2] != null && playingGrid[x + 3, y - 3] != null
                        && playingGrid[x, y].Color == playingGrid[x + 1, y - 1].Color
                        && playingGrid[x, y].Color == playingGrid[x + 2, y - 2].Color
                        && playingGrid[x, y].Color == playingGrid[x + 3, y - 3].Color)
                    {
                        //if there's a win, show who won
                        TheWinnings();
                    }
                }
            }
        }

        private void TheWinnings()
        {
            //if it's first player's turn
            //yellow won, add to yellow's score
            //start the game over, play again?
            if (firstPlayer)
            {
                MessageBox.Show("Yellow Wins!");
                yellowWins++;
                StartOver();
            }
            //if it's second player's turn
            //blue won, add to blue's score
            //start the game over, play again?
            else if (firstPlayer == false)
            {
                MessageBox.Show("Blue Wins!");
                blueWins++;
                StartOver();
            }
        }

        private void StartOver()
        {
            //redraw background grid
            BackgroundGrid(COLUMNS, ROWS);

            //clear playing grid
            for (int x = 0; x < COLUMNS; x++)
            {
                for (int y = 0; y < ROWS; y++)
                {
                    playingGrid[x, y] = null;
                }
            }

            //reset number of spots left
            spotsLeft = ROWS * COLUMNS;
            //reset to first player's turn
            firstPlayer = true;
            BlueTurn.Fill = blueCircle;
            YellowTurn.Fill = blackCircle;
            //show scores
            P1Wins.Content = blueWins;
            P2Wins.Content = yellowWins;
        }


        //drops piece down corresponding column
        private void one_Click(object sender, RoutedEventArgs e)
        {
            DropPiece(0, firstPlayer);
        }

        private void two_Click(object sender, RoutedEventArgs e)
        {

            DropPiece(1, firstPlayer);
        }

        private void three_Click(object sender, RoutedEventArgs e)
        {
            DropPiece(2, firstPlayer);
        }

        private void four_Click(object sender, RoutedEventArgs e)
        {
            DropPiece(3, firstPlayer);
        }

        private void five_Click(object sender, RoutedEventArgs e)
        {
            DropPiece(4, firstPlayer);
        }

        private void six_Click(object sender, RoutedEventArgs e)
        {
            DropPiece(5, firstPlayer);
        }

        private void seven_Click(object sender, RoutedEventArgs e)
        {
            DropPiece(6, firstPlayer);
        }

        private void startOver_Click(object sender, RoutedEventArgs e)
        {
            StartOver();
        }

        //clear scores of both players
        private void ClearScores_Click(object sender, RoutedEventArgs e)
        {
            blueWins = 0;
            yellowWins = 0;
            P1Wins.Content = blueWins;
            P2Wins.Content = yellowWins;
        }

        private void EndGame_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}