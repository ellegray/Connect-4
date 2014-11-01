using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Dg2894_Connect4
{
    class Piece : Canvas
    {
        //dictionary for player's colors
        //could've done differently, but figured why not use something we learned recently
        public static Dictionary<Boolean, SolidColorBrush> playerColor = new Dictionary<Boolean, SolidColorBrush>();

        //size of circles
        //side of player pieces
        //offset of board
        //space between spots on board
        public static int SIZE = 75;
        public static int SMALLERSIZE = 70;
        public static int OFFSET = 30;
        public static int SPACING = 5;

        int row;
        int column;
        string color;

        public int Row { get { return row; } }
        public int Column { get { return column; } }
        public string Color { get { return color; } }

        Ellipse pp;

        //create a piece
        public Piece(int x, int y, Boolean player)
            : base()
        {
            pp = new Ellipse();
            //so there's a small border
            pp.Width = SMALLERSIZE;
            pp.Height = SMALLERSIZE;
            //color of piece depends on which player's turn
            pp.Fill = playerColor[player];
            //for detecting winners
            color = pp.Fill.ToString();

            //add piece to canvas
            this.Children.Add(pp);
            //put piece in correct place
            this.Placement(x, y);
        }

        public void Placement(int x, int y)
        {
            this.column = x;
            this.row = y;

            //puts piece in correct place on canvas to match playing board

            Thickness from = new Thickness(2.5 + OFFSET + x * (SIZE + SPACING), 0, 0, 0);

            Thickness to = new Thickness(
                      2.5 + OFFSET + x * (SIZE + SPACING),
                      107.5 + OFFSET + y * (SIZE + SPACING), 0, 0);

            ThicknessAnimation dropAnimation = new ThicknessAnimation(from, to, TimeSpan.FromSeconds(0.5));

            this.BeginAnimation(Canvas.MarginProperty, dropAnimation);

        }
    }
}
