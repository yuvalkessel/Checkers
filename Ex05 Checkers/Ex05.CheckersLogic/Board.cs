using System;
using System.Text;
using System.Collections.Generic;

namespace CheckersLogic
{
    public class Board
    {
        public struct Cell
        {
            private readonly int r_Row;
            private readonly int r_Column;

            public Cell(int i_Row, int i_Column)
            {
                r_Row = i_Row;
                r_Column = i_Column;
            }

            public int Row
            {
                get
                {
                    return r_Row;
                }
            }

            public int Column
            {
                get
                {
                    return r_Column;
                }
            }
        }

        private readonly int r_Size;
        private Coin[,] m_BoardMatrix;

        internal Board(int i_Size)
        {
            r_Size = i_Size;
        }

        internal int Size
        {
            get
            {
                return r_Size;
            }
        }

        public Coin[,] BoardMatrix
        {
            get
            {
                return m_BoardMatrix;
            }
        }

        public Coin GetCoin(Cell i_Cell)
        {
            return m_BoardMatrix[i_Cell.Row, i_Cell.Column];
        }

        internal void SetBoard(List<Coin> i_Player1Coins, List<Coin> i_Player2Coins)
        {
            m_BoardMatrix = new Coin[r_Size, r_Size];
            buildBoard(i_Player1Coins, i_Player2Coins);
            setCoinsLegalMoves();
        }

        private void buildBoard(List<Coin> i_Player1Coins, List<Coin> i_Player2Coins)
        {
            int rowPlayer2;
            int numOfCoinsRows = (r_Size / 2) - 1;
            int player1CoinIndex = 0;
            int player2CoinIndex = 0;

            for (int rowPlayer1 = 0; rowPlayer1 < numOfCoinsRows; ++rowPlayer1)
            {
                rowPlayer2 = rowPlayer1 + numOfCoinsRows + 2;
                for (int column = 0; column < r_Size; ++column)
                {
                    if ((rowPlayer1 + column) % 2 != 0)
                    {
                        m_BoardMatrix[rowPlayer1, column] = i_Player1Coins[player1CoinIndex];
                        player1CoinIndex++;
                    }

                    if ((rowPlayer2 + column) % 2 != 0)
                    {
                        m_BoardMatrix[rowPlayer2, column] = i_Player2Coins[player2CoinIndex];
                        player2CoinIndex++;
                    }
                }
            }
        }

        private void setCoinsLegalMoves()
        {
            int rowToMove;
            int currentRow;
            int rowPlayer1 = (r_Size / 2) - 2;
            int rowPlayer2 = (r_Size / 2) + 1;
            Coin currentCoin;
            Cell currentCell;
            Cell currentLegalMove;

            for (int column = 0; column < r_Size; ++column)
            {
                if (m_BoardMatrix[rowPlayer1, column] != null)
                {
                    rowToMove = rowPlayer1 + 1;
                    currentRow = rowPlayer1;
                }
                else
                {
                    rowToMove = rowPlayer2 - 1;
                    currentRow = rowPlayer2;
                }

                currentCell = new Cell(currentRow, column);
                currentCoin = GetCoin(currentCell);

                if (column != 0)
                {
                    currentLegalMove = new Cell(rowToMove, column - 1);
                    currentCoin.LegalMoves.Add(currentLegalMove);
                    addToPlayerMovesIfNecessary(currentCell, currentCoin.LegalMoves, currentCoin.Owner.LegalMoves);
                }

                if (column != r_Size - 1)
                {
                    currentLegalMove = new Cell(rowToMove, column + 1);
                    currentCoin.LegalMoves.Add(currentLegalMove);
                    addToPlayerMovesIfNecessary(currentCell, currentCoin.LegalMoves, currentCoin.Owner.LegalMoves);
                }
            }
        }

        internal void UpdatePreviousCellDiagonals(Cell i_PrevCell, Cell i_NewCell)
        {
            int currentListIndex = 0;
            Cell currentCellDegree2;
            List<Cell> potentialDiagonalsDegree1 = getPotentialDiagonals(i_PrevCell, 1);
            List<Cell> potentialDiagonalsDegree2 = getPotentialDiagonals(i_PrevCell, 2);

            removeObligatoryMove(potentialDiagonalsDegree1[0], potentialDiagonalsDegree1[2], i_NewCell);
            removeObligatoryMove(potentialDiagonalsDegree1[1], potentialDiagonalsDegree1[3], i_NewCell);
            foreach (Cell currentCellDegree1 in potentialDiagonalsDegree1)
            {
                if (isInBoundries(currentCellDegree1) && isRelevantCell(currentCellDegree1, i_NewCell))
                {
                    if (GetCoin(currentCellDegree1) != null)
                    {
                        addLegalMoveIfNecessary(currentCellDegree1, i_PrevCell);
                        currentCellDegree2 = potentialDiagonalsDegree2[currentListIndex];
                        if (isInBoundries(currentCellDegree2) && (GetCoin(currentCellDegree2) != null))
                        {
                            addObligatoryMoveIfNecessary(currentCellDegree2, i_PrevCell, currentCellDegree1);
                        }
                    }
                }

                currentListIndex++;
            }
        }

        internal void UpdateNewCellDiagonals(Cell i_PrevCell, Cell i_NewCell)
        {
            int currentListIndex = 0;
            Coin currentCoinDegree1;
            Cell currentCellDegree2;
            List<Cell> potentialDiagonalsDegree1 = getPotentialDiagonals(i_NewCell, 1);
            List<Cell> potentialDiagonalsDegree2 = getPotentialDiagonals(i_NewCell, 2);

            addObligatoryMoveIfNecessary(potentialDiagonalsDegree1[0], potentialDiagonalsDegree1[2], i_NewCell);
            addObligatoryMoveIfNecessary(potentialDiagonalsDegree1[1], potentialDiagonalsDegree1[3], i_NewCell);
            foreach (Cell currentCellDegree1 in potentialDiagonalsDegree1)
            {
                if (isInBoundries(currentCellDegree1) && isRelevantCell(currentCellDegree1, i_PrevCell))
                {
                    currentCoinDegree1 = GetCoin(currentCellDegree1);
                    if (currentCoinDegree1 != null)
                    {
                        removeLegalMove(currentCellDegree1, i_NewCell);
                        currentCellDegree2 = potentialDiagonalsDegree2[currentListIndex];
                        if (isInBoundries(currentCellDegree2) && (GetCoin(currentCellDegree2) != null))
                        {
                            removeObligatoryMoveHelper(currentCellDegree2, i_NewCell);
                        }
                    }
                }

                currentListIndex++;
            }
        }

        internal void UpdateCoinMovesList(Cell i_NewCell)
        {
            int currentListIndex = 0;
            Coin movedCoin = GetCoin(i_NewCell);
            Cell currentCellDegree2;
            List<Cell> potentialDiagonalsDegree1 = getPotentialDiagonals(i_NewCell, 1);
            List<Cell> potentialDiagonalsDegree2 = getPotentialDiagonals(i_NewCell, 2);

            movedCoin.LegalMoves.Clear();
            movedCoin.ObligatoryMoves.Clear();
            foreach (Cell currentCellDegree1 in potentialDiagonalsDegree1)
            {
                if (isInBoundries(currentCellDegree1))
                {
                    if (GetCoin(currentCellDegree1) == null)
                    {
                        addLegalMoveIfNecessary(i_NewCell, currentCellDegree1);
                    }
                    else
                    {
                        currentCellDegree2 = potentialDiagonalsDegree2[currentListIndex];
                        if (isInBoundries(currentCellDegree2) && (GetCoin(currentCellDegree2) == null))
                        {
                            addObligatoryMoveHelper(i_NewCell, currentCellDegree2, currentCellDegree1);
                        }
                    }
                }

                currentListIndex++;
            }
        }

        private List<Cell> getPotentialDiagonals(Cell i_Cell, int i_Degree)
        {
            List<Cell> potentialDiagonals = new List<Cell>(4);
            Cell topLeft = new Cell(i_Cell.Row - i_Degree, i_Cell.Column - i_Degree);
            Cell topRight = new Cell(i_Cell.Row - i_Degree, i_Cell.Column + i_Degree);
            Cell bottomRight = new Cell(i_Cell.Row + i_Degree, i_Cell.Column + i_Degree);
            Cell bottomLeft = new Cell(i_Cell.Row + i_Degree, i_Cell.Column - i_Degree);

            potentialDiagonals.Add(topLeft);
            potentialDiagonals.Add(topRight);
            potentialDiagonals.Add(bottomRight);
            potentialDiagonals.Add(bottomLeft);

            return potentialDiagonals;
        }

        private void addLegalMoveIfNecessary(Cell i_CoinListCell, Cell i_CellToAdd)
        {
            Coin coinList;

            if (isInLegalDirection(i_CoinListCell, i_CellToAdd))
            {
                coinList = GetCoin(i_CoinListCell);
                GetCoin(i_CoinListCell).LegalMoves.Add(i_CellToAdd);
                addToPlayerMovesIfNecessary(i_CoinListCell, coinList.LegalMoves, coinList.Owner.LegalMoves);
            }
        }

        private void addObligatoryMoveIfNecessary(Cell i_DiagonalUpCell, Cell i_DiagonalDownCell, Cell i_CellToEat)
        {
            Coin diagonalUpCoin;
            Coin diagonalDownCoin;

            if (isInBoundries(i_DiagonalUpCell) && isInBoundries(i_DiagonalDownCell))
            {
                diagonalUpCoin = GetCoin(i_DiagonalUpCell);
                diagonalDownCoin = GetCoin(i_DiagonalDownCell);
                if ((diagonalDownCoin == null) && (diagonalUpCoin != null))
                {
                    addObligatoryMoveHelper(i_DiagonalUpCell, i_DiagonalDownCell, i_CellToEat);
                }
                else if ((diagonalDownCoin != null) && (diagonalUpCoin == null))
                {
                    addObligatoryMoveHelper(i_DiagonalDownCell, i_DiagonalUpCell, i_CellToEat);
                }
            }
        }

        private void addObligatoryMoveHelper(Cell i_CoinListCell, Cell i_CellToAdd, Cell i_CellToEat)
        {
            Coin coinList = GetCoin(i_CoinListCell);

            if (isOpponent(i_CellToEat, i_CoinListCell) && isInLegalDirection(i_CoinListCell, i_CellToEat))
            {
                coinList.ObligatoryMoves.Add(i_CellToAdd);
                addToPlayerMovesIfNecessary(i_CoinListCell, coinList.ObligatoryMoves, coinList.Owner.ObligatoryMoves);
            }
        }

        private void removeObligatoryMove(Cell i_DiagonalUpCell, Cell i_DiagonalDownCell, Cell i_NewCell)
        {
            Coin diagonalUpCoin;
            Coin diagonalDownCoin;

            if (isInBoundries(i_DiagonalUpCell) && isInBoundries(i_DiagonalDownCell))
            {
                diagonalUpCoin = GetCoin(i_DiagonalUpCell);
                diagonalDownCoin = GetCoin(i_DiagonalDownCell);
                if (diagonalUpCoin != null && (diagonalDownCoin == null || i_DiagonalDownCell.Equals(i_NewCell)))
                {
                    removeObligatoryMoveHelper(i_DiagonalUpCell, i_DiagonalDownCell);
                }
                else if (diagonalDownCoin != null && (diagonalUpCoin == null || i_DiagonalUpCell.Equals(i_NewCell)))
                {
                    removeObligatoryMoveHelper(i_DiagonalDownCell, i_DiagonalUpCell);
                }
            }
        }

        private void removeObligatoryMoveHelper(Cell i_CoinListCell, Cell i_CellToRemove)
        {
            Coin coinList = GetCoin(i_CoinListCell);

            if (coinList.ObligatoryMoves.Contains(i_CellToRemove))
            {
                coinList.ObligatoryMoves.Remove(i_CellToRemove);
                removeFromPlayerMovesIfNecessary(i_CoinListCell, coinList.ObligatoryMoves, coinList.Owner.ObligatoryMoves);
            }
        }

        private void removeLegalMove(Cell i_CellDegree1, Cell i_CellToRemove)
        {
            Coin coinDegree1 = GetCoin(i_CellDegree1);

            if (coinDegree1.LegalMoves.Contains(i_CellToRemove))
            {
                coinDegree1.LegalMoves.Remove(i_CellToRemove);
                removeFromPlayerMovesIfNecessary(i_CellDegree1, coinDegree1.LegalMoves, coinDegree1.Owner.LegalMoves);
            }
        }

        private void addToPlayerMovesIfNecessary(Cell i_CellToAdd, List<Cell> i_CoinInCellList, List<Cell> i_PlayerMovesList)
        {
            if (i_CoinInCellList.Count == 1)
            {
                i_PlayerMovesList.Add(i_CellToAdd);
            }
        }

        private void removeFromPlayerMovesIfNecessary(Cell i_CellToRemove, List<Cell> i_CoinInCellList, List<Cell> i_PlayerMovesList)
        {
            if (i_CoinInCellList.Count == 0)
            {
                i_PlayerMovesList.Remove(i_CellToRemove);
            }
        }

        private bool isInLegalDirection(Cell i_SourceCell, Cell i_TargetCell)
        {
            Coin coinToMove = m_BoardMatrix[i_SourceCell.Row, i_SourceCell.Column];

            return coinToMove.IsKing() || (coinToMove.GetDirection() + i_SourceCell.Row) == i_TargetCell.Row;
        }

        private bool isOpponent(Cell i_Cell, Cell i_DiagonalCell)
        {
            return !GetCoin(i_Cell).Owner.Equals(GetCoin(i_DiagonalCell).Owner);
        }

        internal void MoveCoin(Cell i_SourceCell, Cell i_TargetCell)
        {
            Coin coinToMove = GetCoin(i_SourceCell);

            m_BoardMatrix[i_TargetCell.Row, i_TargetCell.Column] = coinToMove;
            m_BoardMatrix[i_SourceCell.Row, i_SourceCell.Column] = null;
        }

        private bool isInBoundries(Cell i_PotentialCell)
        {
            bool isRowInBoundries = i_PotentialCell.Row >= 0 && i_PotentialCell.Row < r_Size;
            bool isColumnInBoundries = i_PotentialCell.Column >= 0 && i_PotentialCell.Column < r_Size;

            return isRowInBoundries && isColumnInBoundries;
        }

        private bool isRelevantCell(Board.Cell i_PotentialCell, Board.Cell i_CompareToCell)
        {
            return !i_PotentialCell.Equals(i_CompareToCell);
        }
    }
}