using System;
using System.Windows.Forms;
using System.Drawing;
using System.Media;
using CheckersLogic;

namespace Ex05.CheckersUI
{
    internal class FormGame : Form
    {
        private const int k_Margin = 10;
        private const int k_CellSize = 50;
        private const int k_BoardTop = 50;
        private const int k_BoardLeft = k_Margin;
        private const int k_CoinSize = k_CellSize - 20;
        private readonly SoundPlayer r_IllegalSound = new SoundPlayer(Properties.Resources.ErrorSound);
        private readonly SoundPlayer r_BiteSound = new SoundPlayer(Properties.Resources.BiteSound);
        private readonly Image r_Player1Image = Properties.Resources.O;
        private readonly Image r_Player2Image = Properties.Resources.X;
        private readonly Image r_Player1KingImage = Properties.Resources.U;
        private readonly Image r_Player2KingImage = Properties.Resources.K;
        private readonly FormLogin fromLogin = new FormLogin();
        private readonly Label labelPlayer1Name = new Label();
        private readonly Label labelPlayer2Name = new Label();
        private readonly Label labelPlayer1Score = new Label();
        private readonly Label labelPlayer2Score = new Label();
        private readonly CellButton[,] r_GameBoard;
        private readonly Game r_Game;
        private readonly string r_Player1Name;
        private readonly string r_Player2Name;
        private readonly int r_BoardSize;
        private readonly bool r_IsComputerPlayer;
        private CellButton m_SourceButtonCell;
        private CellButton m_TargetButtonCell;
        private bool m_IsGameOver;

        internal FormGame(string i_Player1Name, string i_Player2Name, int i_BoardSize)
        {
            r_Player1Name = i_Player1Name;
            if (i_Player2Name.Equals("[Computer]"))
            {
                r_Player2Name = "Computer";
                r_IsComputerPlayer = true;
            }
            else
            {
                r_Player2Name = i_Player2Name;
                r_IsComputerPlayer = false;
            }

            r_BoardSize = i_BoardSize;
            r_GameBoard = new CellButton[r_BoardSize, r_BoardSize];
            r_Game = new Game(r_Player1Name, r_Player2Name, r_BoardSize);
            initializeComponents();
            setFormGame();
        }

        private void setFormGame()
        {
            setFormSize();
            setPlayersNameAndScore();
            buildBoard();
            setNewRound();
        }

        private void initializeComponents()
        {
            SuspendLayout();
            initializePlayerLabel(labelPlayer1Name);
            initializePlayerLabel(labelPlayer2Name);
            initializePlayerScoreLabel(labelPlayer1Score);
            initializePlayerScoreLabel(labelPlayer2Score);
            initializeGameForm();
            Controls.AddRange(new Control[]
            {
                labelPlayer1Name,
                labelPlayer2Name,
                labelPlayer1Score,
                labelPlayer2Score
            });
            ResumeLayout(false);
        }

        private void initializeGameForm()
        {
            MaximizeBox = false;
            MinimizeBox = false;
            Text = "Checkers";
            Icon = Properties.Resources.ChessBoard;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void initializePlayerLabel(Label i_PlayerLabel)
        {
            i_PlayerLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            i_PlayerLabel.AutoSize = true;
        }
        
        private void initializePlayerScoreLabel(Label i_PlayerScoreLabel)
        {
            i_PlayerScoreLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            i_PlayerScoreLabel.AutoSize = true;
        }

        private void setFormSize()
        {
            int boardSize = k_CellSize * r_BoardSize;

            ClientSize = new Size(boardSize + (k_Margin * 2), boardSize + (k_BoardTop * 2));
        }

        private void setPlayersNameAndScore()
        {
            int boardSize = k_CellSize * r_BoardSize;
            int player1LabelsWidth;
            int player2LabelsWidth;
     
            labelPlayer1Name.Text = r_Player1Name + ":";
            labelPlayer2Name.Text = r_Player2Name + ":";
            player1LabelsWidth = labelPlayer1Name.Width + labelPlayer1Score.Width;
            player2LabelsWidth = labelPlayer2Name.Width + labelPlayer2Score.Width;
            labelPlayer1Name.Top = k_BoardTop - labelPlayer1Name.Height - 10;
            labelPlayer1Name.Left = (ClientSize.Width / 2) - (player1LabelsWidth / 2);
            labelPlayer1Score.Left = labelPlayer1Name.Right + 5;
            labelPlayer1Score.Top = labelPlayer1Name.Top;
            labelPlayer2Name.Top = k_BoardTop + boardSize + 10;
            labelPlayer2Name.Left = (ClientSize.Width / 2) - (player2LabelsWidth / 2);
            labelPlayer2Score.Left = labelPlayer2Name.Right + 5;
            labelPlayer2Score.Top = labelPlayer2Name.Top;
        }

        private void setBoard()
        {
            buildBoard();
            arrangeCoins();
        }

        private void buildBoard()
        {
            int boardSize = r_BoardSize;
            int currentCellLeft = k_BoardLeft;
            int currentCellTop = k_BoardTop;

            for (int row = 0; row < boardSize; row++)
            {
                for (int column = 0; column < boardSize; column++)
                {
                    r_GameBoard[row, column] = createNewCell(row, column, currentCellLeft, currentCellTop);
                    currentCellLeft += k_CellSize;
                }

                currentCellTop += k_CellSize;
                currentCellLeft = k_BoardLeft;
            }
        }

        private CellButton createNewCell(int i_Row, int i_Column, int i_Left, int i_Top)
        {
            CellButton currentButton = new CellButton(i_Row, i_Column);

            currentButton.Left = i_Left;
            currentButton.Top = i_Top;
            currentButton.BackColor = Color.White;
            currentButton.FlatStyle = FlatStyle.Flat;
            currentButton.Size = new Size(k_CellSize, k_CellSize);
            currentButton.Click += new EventHandler(cellButton_Click);
            Controls.Add(currentButton);

            return currentButton;
        }

        private void arrangeCoins()
        {
            Coin currentCoin;
            Coin[,] logicalBoardMatrix = r_Game.Board.BoardMatrix;
            int halfBoardSize = r_BoardSize / 2;

            for (int row = 0; row < r_BoardSize; ++row)
            {
                for (int column = 0; column < r_BoardSize; ++column)
                {
                    currentCoin = logicalBoardMatrix[row, column];
                    if (currentCoin != null)
                    {
                        r_GameBoard[row, column].Image = currentCoin.Symbol.ToString().Equals("O") ? r_Player1Image : r_Player2Image;
                    }
                    else
                    {
                        r_GameBoard[row, column].Image = null;
                        if (!((row == halfBoardSize || row == halfBoardSize - 1) && (row + column) % 2 != 0))
                        {
                            disableCell(r_GameBoard[row, column]);
                        }
                    }
                }
            }
        }

        private void disableCell(CellButton i_CellButton)
        {
            i_CellButton.Enabled = false;
            i_CellButton.FlatStyle = FlatStyle.Flat;
            i_CellButton.BackColor = Color.DarkGray;
        }

        private void cellButton_Click(object sender, EventArgs e)
        {
            CellButton clickedCellButton = sender as CellButton;
            bool isValidMove;

            if (m_SourceButtonCell == null)
            {
                if (r_Game.IsLegalSourceCell(clickedCellButton.BoardLocation))
                {
                    clickedCellButton.BackColor = Color.LightBlue;
                    m_SourceButtonCell = clickedCellButton;
                }
                else
                {
                    r_IllegalSound.Play();
                }
            }
            else
            {
                if (clickedCellButton == m_SourceButtonCell)
                {
                    clickedCellButton.BackColor = Color.White;
                    m_SourceButtonCell = null;
                }
                else
                {
                    isValidMove = r_Game.IsLegalOption(m_SourceButtonCell.BoardLocation, clickedCellButton.BoardLocation);

                    if (isValidMove)
                    {
                        m_TargetButtonCell = clickedCellButton;
                        r_Game.UpdateMove(m_SourceButtonCell.BoardLocation, m_TargetButtonCell.BoardLocation);
                        handleCoinMove();
                        handleComputerMoveIfNecessary();
                    }
                    else
                    {
                        r_IllegalSound.Play();
                        MessageBox.Show("Illegal Move! Please try again.");
                        m_SourceButtonCell.BackColor = Color.White;
                        m_SourceButtonCell = null;
                    }
                }
            }
        }

        private void handleCoinMove()
        {
            updateTargetAndSourceCellCoinImage();
            handleEatingMoveIfNecessary();
            evaluateAndUpdateGameStatus();
        }

        private void handleComputerMoveIfNecessary()
        {
            if (!m_IsGameOver && r_IsComputerPlayer && r_Game.CurrentTurn.Name.Equals("Computer"))
            {
                do
                {
                    r_Game.SetComputerRandomMoveCells();
                    m_SourceButtonCell = r_GameBoard[r_Game.SourceCell.Row, r_Game.SourceCell.Column];
                    m_TargetButtonCell = r_GameBoard[r_Game.TargetCell.Row, r_Game.TargetCell.Column];
                    r_Game.UpdateMove(m_SourceButtonCell.BoardLocation, m_TargetButtonCell.BoardLocation);
                    handleCoinMove();
                }
                while (r_Game.CurrentTurn.Name.Equals("Computer") && r_Game.HasContinuousMoves());
            }
        }

        private void updateTargetAndSourceCellCoinImage()
        {
            switch (r_Game.Board.GetCoin(m_TargetButtonCell.BoardLocation).Symbol)
            {
                case Coin.eCoinSymbol.O:
                    m_TargetButtonCell.Image = r_Player1Image;
                    break;
                case Coin.eCoinSymbol.X:
                    m_TargetButtonCell.Image = r_Player2Image;
                    break;
                case Coin.eCoinSymbol.U:
                    m_TargetButtonCell.Image = r_Player1KingImage;
                    break;
                case Coin.eCoinSymbol.K:
                    m_TargetButtonCell.Image = r_Player2KingImage;
                    break;
            }

            m_SourceButtonCell.BackColor = Color.White;
            m_SourceButtonCell.Image = null;
            m_SourceButtonCell = null;
        }

        private void handleEatingMoveIfNecessary()
        {
            Board.Cell eatenCell;

            if (r_Game.IsEatingMove)
            {
                eatenCell = r_Game.GetEatenCell();

                r_BiteSound.Play();
                r_GameBoard[eatenCell.Row, eatenCell.Column].Image = null;
            }
        }

        private void evaluateAndUpdateGameStatus()
        {
            if (r_Game.IsWin)
            {
                handleEndOfGame(string.Format("{1} Won!{0}Another Round?", Environment.NewLine, r_Game.WinnerName));
            }
            else if (r_Game.IsDraw)
            {
                handleEndOfGame(string.Format("Tie!{0}Another Round?", Environment.NewLine));
            }
            else
            {
                switchTurn();
            }
        }

        private void switchTurn()
        {
            if (r_Game.CurrentTurn.Name.Equals(r_Player1Name))
            {
                colorCurrentPlayerName(labelPlayer1Name, labelPlayer2Name);
            }
            else
            {
                colorCurrentPlayerName(labelPlayer2Name, labelPlayer1Name);
            }
        }

        private void colorCurrentPlayerName(Label i_CurrentPlayerNameLabel, Label i_PrevPlayerNameLabel)
        {
            i_CurrentPlayerNameLabel.ForeColor = Color.FromArgb(54, 145, 176);
            i_PrevPlayerNameLabel.ForeColor = Color.Black;
        }

        private void handleEndOfGame(string i_Message)
        {
            DialogResult dialogResult = MessageBox.Show(i_Message, "Checkers", MessageBoxButtons.YesNo);

            m_IsGameOver = true;
            if (dialogResult.Equals(DialogResult.Yes))
            {
                setNewRound();
            }
            else
            {
                Application.Exit();
            }
        }

        private void setNewRound()
        {
            r_Game.SetGame();
            arrangeCoins();
            updateScoreStatus();
            switchTurn();
            m_IsGameOver = false;
            m_SourceButtonCell = null;
            m_TargetButtonCell = null;
        }

        private void updateScoreStatus()
        {
            labelPlayer1Score.Text = r_Game.Player1.Points.ToString();
            labelPlayer2Score.Text = r_Game.Player2.Points.ToString();
        }
    }
}
