using Microsoft.Maui.Controls;
using System;

namespace MinesweeperApp
{
    public partial class MainPage : ContentPage
    {
        private MinesweeperGame _minesweeperGame;

        public MainPage()
        {
            InitializeComponent();
            _minesweeperGame = new MinesweeperGame(GameGrid, 10, 10, 20);
            _minesweeperGame.InitializeGame();
        }
    }
}
