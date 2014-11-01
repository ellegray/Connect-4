using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dg2894_Connect4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Boolean onePlayer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnePlayer_Click(object sender, RoutedEventArgs e)
        {
            onePlayer = true;
            GameWindow myGame = new GameWindow(this, onePlayer);
            myGame.ShowDialog();
        }

        private void TwoPlayer_Click(object sender, RoutedEventArgs e)
        {
            onePlayer = false;
            GameWindow myGame = new GameWindow(this, onePlayer);
            myGame.ShowDialog();
        }

    }
}
