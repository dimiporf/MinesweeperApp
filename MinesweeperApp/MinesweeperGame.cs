using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;

namespace MinesweeperApp
{
    public class MinesweeperGame
    {
        private readonly Grid _gameGrid;
        private readonly int _rows;
        private readonly int _columns;
        private readonly int _mineCount;
        private Button[,] _buttons;
        private int[,] _mineGrid;

        public MinesweeperGame(Grid gameGrid, int rows, int columns, int mineCount)
        {
            _gameGrid = gameGrid;
            _rows = rows;
            _columns = columns;
            _mineCount = mineCount;
        }

        public void InitializeGame()
        {
            InitializeGameGrid();
            PlaceMines();
            CalculateAdjacentMines();
        }

        private void InitializeGameGrid()
        {
            _buttons = new Button[_rows, _columns];
            _gameGrid.RowDefinitions.Clear();
            _gameGrid.ColumnDefinitions.Clear();
            _gameGrid.Children.Clear();

            for (int row = 0; row < _rows; row++)
            {
                _gameGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            for (int col = 0; col < _columns; col++)
            {
                _gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    var button = new Button
                    {
                        BackgroundColor = Colors.LightGray
                    };
                    button.Clicked += OnButtonClicked;
                    _gameGrid.Children.Add(button);
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    _buttons[row, col] = button;
                }
            }
        }

        private void PlaceMines()
        {
            _mineGrid = new int[_rows, _columns];
            Random random = new Random();
            int minesPlaced = 0;

            while (minesPlaced < _mineCount)
            {
                int row = random.Next(_rows);
                int col = random.Next(_columns);

                if (_mineGrid[row, col] == 0)
                {
                    _mineGrid[row, col] = -1; // -1 indicates a mine
                    minesPlaced++;
                }
            }
        }

        private void CalculateAdjacentMines()
        {
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    if (_mineGrid[row, col] == -1)
                        continue;

                    int mineCount = 0;
                    for (int r = -1; r <= 1; r++)
                    {
                        for (int c = -1; c <= 1; c++)
                        {
                            int newRow = row + r;
                            int newCol = col + c;

                            if (newRow >= 0 && newRow < _rows && newCol >= 0 && newCol < _columns && _mineGrid[newRow, newCol] == -1)
                            {
                                mineCount++;
                            }
                        }
                    }

                    _mineGrid[row, col] = mineCount;
                }
            }
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);

            if (_mineGrid[row, col] == -1)
            {
                // Mine clicked, game over logic
                button.Text = "M";
                button.BackgroundColor = Colors.Red;
                App.Current.MainPage.DisplayAlert("Game Over", "You clicked on a mine!", "OK");
            }
            else
            {
                // Reveal the number of adjacent mines
                button.Text = _mineGrid[row, col].ToString();
                button.BackgroundColor = Colors.White;

                // Optionally, reveal surrounding cells if the count is 0
                if (_mineGrid[row, col] == 0)
                {
                    RevealAdjacentCells(row, col);
                }

                CheckWinCondition();
            }
        }

        private void RevealAdjacentCells(int row, int col)
        {
            for (int r = -1; r <= 1; r++)
            {
                for (int c = -1; c <= 1; c++)
                {
                    int newRow = row + r;
                    int newCol = col + c;

                    if (newRow >= 0 && newRow < _rows && newCol >= 0 && newCol < _columns)
                    {
                        var button = _buttons[newRow, newCol];
                        if (button.Text == string.Empty)
                        {
                            button.Text = _mineGrid[newRow, newCol].ToString();
                            button.BackgroundColor = Colors.White;

                            if (_mineGrid[newRow, newCol] == 0)
                            {
                                RevealAdjacentCells(newRow, newCol);
                            }
                        }
                    }
                }
            }
        }

        private void CheckWinCondition()
        {
            int cellsRevealed = 0;

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    if (_buttons[row, col].BackgroundColor == Colors.White)
                    {
                        cellsRevealed++;
                    }
                }
            }

            if (cellsRevealed == (_rows * _columns - _mineCount))
            {
                App.Current.MainPage.DisplayAlert("Congratulations", "You've won!", "OK");
            }
        }
    }
}
