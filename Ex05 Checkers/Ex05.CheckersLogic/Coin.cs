using System;
using System.Collections.Generic;

namespace CheckersLogic
{
    public class Coin
    {
        public enum eCoinSymbol
        {
            X,
            O,
            U,
            K
        }

        private readonly List<Board.Cell> r_LegalMoves;
        private readonly List<Board.Cell> r_ObligatoryMoves;
        private readonly Player r_Owner;
        private eCoinSymbol m_Symbol;
        private bool m_IsInGame;

        internal Coin(Player i_Owner)
        {
            r_Owner = i_Owner;
            r_LegalMoves = new List<Board.Cell>(4);
            r_ObligatoryMoves = new List<Board.Cell>(4);
        }

        internal Player Owner
        {
            get
            {
                return r_Owner;
            }
        }

        internal List<Board.Cell> LegalMoves
        {
            get
            {
                return r_LegalMoves;
            }
        }

        internal List<Board.Cell> ObligatoryMoves
        {
            get
            {
                return r_ObligatoryMoves;
            }
        }

        public eCoinSymbol Symbol
        {
            get
            {
                return m_Symbol;
            }
            set
            {
                m_Symbol = value;
            }
        }

        internal bool IsInGame
        {
            get
            {
                return m_IsInGame;
            }
            set
            {
                m_IsInGame = value;
            }
        }

        internal void SetToKing()
        {
            m_Symbol = m_Symbol.Equals(eCoinSymbol.O) ? eCoinSymbol.U : eCoinSymbol.K;
        }

        internal bool IsKing()
        {
            return m_Symbol.Equals(eCoinSymbol.K) || m_Symbol.Equals(eCoinSymbol.U);
        }

        internal int GetDirection()
        {
            return m_Symbol.Equals(eCoinSymbol.O) ? 1 : (-1);
        }
    }
}