using System;
using System.Text;
using System.Collections.Generic;

namespace CheckersLogic
{
    public class Game
    {
        private readonly Board r_Board;
        private readonly Player r_Player1;
        private readonly Player r_Player2;
        private Player m_CurrentTurn;
        private Random m_RandomMovePicker;
        private Board.Cell m_SourceCell;
        private Board.Cell m_TargetCell;
        private bool m_IsEatingMove;
        private bool m_HasSwitchedTurn;
        private bool m_IsWin;
        private bool m_IsDraw;
        private string m_WinnerName;

        public Game(string i_Player1Name, string i_Player2Name, int i_BoardSize)
        {
            Player.eType player1Type = Player.eType.Human;
            Player.eType player2Type = i_Player2Name.Equals("Computer") ? Player.eType.Computer : Player.eType.Human;
            int numberOfCoinsPerPlayer = calcNumberOfCoinsPerPlayer(i_BoardSize);

            r_Board = new Board(i_BoardSize);
            r_Player1 = new Player(i_Player1Name, player1Type, Player.eCoinsSign.O, numberOfCoinsPerPlayer);
            r_Player2 = new Player(i_Player2Name, player2Type, Player.eCoinsSign.X, numberOfCoinsPerPlayer);
            setRandomMovePickerIfNecessary();
        }

        public Board Board
        {
            get
            {
                return r_Board;
            }
        }

        public Player Player1
        {
            get
            {
                return r_Player1;
            }
        }

        public Player Player2
        {
            get
            {
                return r_Player2;
            }
        }

        public Player CurrentTurn
        {
            get
            {
                return m_CurrentTurn;
            }
        }

        public Board.Cell SourceCell
        {
            get
            {
                return m_SourceCell;
            }
        }

        public Board.Cell TargetCell
        {
            get
            {
                return m_TargetCell;
            }
        }

        public bool IsEatingMove
        {
            get
            {
                return m_IsEatingMove;
            }
        }

        public bool IsWin
        {
            get
            {
                return m_IsWin;
            }
        }

        public bool IsDraw
        {
            get
            {
                return m_IsDraw;
            }
        }
        
        public string WinnerName
        {
            get
            {
                return m_WinnerName;
            }
        }

        public void SetGame()
        {
            m_CurrentTurn = r_Player1;
            m_HasSwitchedTurn = true;
            setPlayersMovesLists();
            setPlayersCoins();
            r_Board.SetBoard(r_Player1.Coins, r_Player2.Coins);
        }

        private int calcNumberOfCoinsPerPlayer(int i_BoardSize)
        {
            return (i_BoardSize / 2) * ((i_BoardSize / 2) - 1);
        }

        private void setRandomMovePickerIfNecessary()
        {
            if (r_Player2.Type.Equals(Player.eType.Computer))
            {
                m_RandomMovePicker = new Random();
            }
        }
        
        public void SetComputerRandomMoveCells()
        {
            if (m_CurrentTurn.ObligatoryMoves.Count != 0)
            {
                m_SourceCell = getRandomCell(m_CurrentTurn.ObligatoryMoves);
                m_TargetCell = getRandomCell(r_Board.GetCoin(m_SourceCell).ObligatoryMoves);
            }
            else
            {
                m_SourceCell = getRandomCell(m_CurrentTurn.LegalMoves);
                m_TargetCell = getRandomCell(r_Board.GetCoin(m_SourceCell).LegalMoves);
            }
        }

        private Board.Cell getRandomCell(List<Board.Cell> i_MovesList)
        {
            int randomMoveIndex = m_RandomMovePicker.Next(0, i_MovesList.Count - 1);

            return i_MovesList[randomMoveIndex];
        }

        private void setPlayersMovesLists()
        {
            setPlayerMovesLists(r_Player1);
            setPlayerMovesLists(r_Player2);
        }

        private void setPlayerMovesLists(Player i_Player)
        {
            i_Player.ObligatoryMoves.Clear();
            i_Player.LegalMoves.Clear();
        }

        private void setPlayersCoins()
        {
            setPlayerCoins(r_Player1);
            setPlayerCoins(r_Player2);
        }

        private void setPlayerCoins(Player i_Player)
        {
            foreach (Coin currentCoin in i_Player.Coins)
            {
                currentCoin.Symbol = i_Player.CoinsSign.Equals(Player.eCoinsSign.X) ? Coin.eCoinSymbol.X : Coin.eCoinSymbol.O;
                currentCoin.IsInGame = true;
                currentCoin.LegalMoves.Clear();
                currentCoin.ObligatoryMoves.Clear();
            }
        }

        public bool IsLegalOption(Board.Cell i_SourceCell, Board.Cell i_TargetCell)
        {
            return m_HasSwitchedTurn ? isLegalMove(i_SourceCell, i_TargetCell) : isLegalContinuousMove(i_SourceCell, i_TargetCell);
        }

        public bool IsLegalSourceCell(Board.Cell i_SourceCell)
        {
            Coin coinSource = r_Board.GetCoin(i_SourceCell);

            return coinSource != null && coinSource.Owner.Equals(m_CurrentTurn);
        }

        private bool isLegalMove(Board.Cell i_SourceCell, Board.Cell i_TargetCell)
        {
            bool isInObligatoryMoves;
            bool isInLegalMoves;
            bool playerHasObligatoryMoves = HasContinuousMoves();
            Coin coinSource = r_Board.GetCoin(i_SourceCell);
            Coin coinTarget = r_Board.GetCoin(i_TargetCell);

            isInObligatoryMoves = playerHasObligatoryMoves && coinSource.ObligatoryMoves.Contains(i_TargetCell);
            isInLegalMoves = (coinSource.ObligatoryMoves.Count == 0) && coinSource.LegalMoves.Contains(i_TargetCell);

            return (coinTarget == null) && (isInObligatoryMoves || (!playerHasObligatoryMoves && isInLegalMoves));
        }

        private bool isLegalContinuousMove(Board.Cell i_SourceCell, Board.Cell i_TargetCell)
        {
            bool isSourceCellLegal = i_SourceCell.Equals(m_TargetCell);
            bool isTargetCellLegal = (r_Board.GetCoin(i_TargetCell) == null) && r_Board.GetCoin(i_SourceCell).ObligatoryMoves.Contains(i_TargetCell);

            return isSourceCellLegal && isTargetCellLegal;
        }

        public bool HasContinuousMoves()
        {
            return (m_CurrentTurn.ObligatoryMoves.Count == 0) ? false : true;
        }

        private Player getOpponent()
        {
            return m_CurrentTurn.Equals(r_Player1) ? r_Player2 : r_Player1;
        }

        public void UpdateMove(Board.Cell i_SourceButtonCell, Board.Cell i_TargetButtonCell)
        {
            Coin coinToMove;
            bool hasBecomeKing;
            bool hasNoFollowingObligatoriesMoves;

            m_SourceCell = i_SourceButtonCell;
            m_TargetCell = i_TargetButtonCell;
            coinToMove = r_Board.GetCoin(m_SourceCell);
            hasBecomeKing = isSettingToKingNeeded(coinToMove);
            m_IsEatingMove = coinToMove.ObligatoryMoves.Contains(m_TargetCell);
            coinToMove.Owner.LegalMoves.Remove(m_SourceCell);
            coinToMove.Owner.ObligatoryMoves.Remove(m_SourceCell);
            r_Board.MoveCoin(m_SourceCell, m_TargetCell);
            if (hasBecomeKing)
            {
                coinToMove.SetToKing();
            }

            if (m_IsEatingMove)
            {
                handleEatingMove();
            }

            r_Board.UpdateCoinMovesList(m_TargetCell);
            r_Board.UpdatePreviousCellDiagonals(m_SourceCell, m_TargetCell);
            r_Board.UpdateNewCellDiagonals(m_SourceCell, m_TargetCell);
            hasNoFollowingObligatoriesMoves = coinToMove.ObligatoryMoves.Count == 0;
            m_HasSwitchedTurn = hasBecomeKing || !m_IsEatingMove || (m_IsEatingMove && hasNoFollowingObligatoriesMoves);
            if (m_HasSwitchedTurn)
            {
                updateGameStatus();
                m_CurrentTurn = getOpponent();
            }
        }

        public Board.Cell GetEatenCell()
        {
            int eatenRow = (m_SourceCell.Row + m_TargetCell.Row) / 2;
            int eatenCol = (m_SourceCell.Column + m_TargetCell.Column) / 2;

            return new Board.Cell(eatenRow, eatenCol);
        }

        private void handleEatingMove()
        {
            Board.Cell eatenCell = GetEatenCell();
            Coin eatenCoin = r_Board.GetCoin(eatenCell);

            r_Board.BoardMatrix[eatenCell.Row, eatenCell.Column] = null;
            eatenCoin.Owner.LegalMoves.Remove(eatenCell);
            eatenCoin.Owner.ObligatoryMoves.Remove(eatenCell);
            eatenCoin.IsInGame = false;
            r_Board.UpdatePreviousCellDiagonals(eatenCell, m_TargetCell);
        }

        private bool isSettingToKingNeeded(Coin i_CoinToMove)
        {
            bool isTopAndX = m_TargetCell.Row == 0 && m_CurrentTurn.CoinsSign.Equals(Player.eCoinsSign.X);
            bool isBottomAndY = m_TargetCell.Row == r_Board.Size - 1 && m_CurrentTurn.CoinsSign.Equals(Player.eCoinsSign.O);

            return !i_CoinToMove.IsKing() && (isTopAndX || isBottomAndY);
        }

        private void updateGameStatus()
        {
            if (m_IsWin = hasNoMovesLeft(getOpponent()))
            {
                m_CurrentTurn.Points += calcWinnerPoints(m_CurrentTurn, getOpponent());
                m_WinnerName = m_CurrentTurn.Name;
            }

            m_IsDraw = hasNoMovesLeft(r_Player1) && hasNoMovesLeft(r_Player2);
        }

        private bool hasNoMovesLeft(Player i_Player)
        {
            return (i_Player.LegalMoves.Count == 0) && (i_Player.ObligatoryMoves.Count == 0);
        }

        private int calcWinnerPoints(Player i_Winner, Player i_Loser)
        {
            return i_Winner.CalcPoints() - i_Loser.CalcPoints();
        }
    }
}