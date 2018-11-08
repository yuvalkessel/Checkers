using System.Collections.Generic;

namespace CheckersLogic
{
    public class Player
    {
        internal enum eCoinsSign
        {
            X,
            O
        }

        internal enum eType
        {
            Human,
            Computer
        }

        private readonly string r_Name;
        private readonly eType r_Type;
        private readonly List<Board.Cell> r_ObligatoryMoves;
        private readonly List<Board.Cell> r_LegalMoves;
        private readonly eCoinsSign r_CoinsSign;
        private readonly List<Coin> r_Coins;
        private int m_Points;

        internal Player(string i_Name, eType i_Type, eCoinsSign i_CoinsSign, int i_NumberOfCoins)
        {
            r_Name = i_Name;
            r_Type = i_Type;
            r_CoinsSign = i_CoinsSign;
            m_Points = 0;
            r_LegalMoves = new List<Board.Cell>();
            r_ObligatoryMoves = new List<Board.Cell>();
            r_Coins = new List<Coin>(i_NumberOfCoins);
            initializeCoins();
        }
    
        public string Name
        {
            get
            {
                return r_Name;
            }
        }

        internal List<Coin> Coins
        {
            get
            {
                return r_Coins;
            }
        }

        internal List<Board.Cell> ObligatoryMoves
        {
            get
            {
                return r_ObligatoryMoves;
            }
        }

        internal List<Board.Cell> LegalMoves
        {
            get
            {
                return r_LegalMoves;
            }
        }

        internal eType Type
        {
            get
            {
                return r_Type;
            }
        }

        internal eCoinsSign CoinsSign
        {
            get
            {
                return r_CoinsSign;
            }
        }

        public int Points
        {
            get
            {
                return m_Points;
            }
            set
            {
                m_Points = value;
            }
        }

        internal int CalcPoints()
        {
            int totalPoints = 0;

            foreach (Coin currentCoin in Coins)
            {
                if (currentCoin.IsInGame)
                {
                    totalPoints += currentCoin.IsKing() ? 4 : 1;
                }
            }

            return totalPoints;
        }

        private void initializeCoins()
        {
            Coin newCoin;

            for (int i = 0; i < r_Coins.Capacity; ++i)
            {
                newCoin = new Coin(this);
                r_Coins.Add(newCoin);
            }
        }
    }
}