using System;
using System.Windows.Forms;
using CheckersLogic;

namespace Ex05.CheckersUI
{
    internal class CellButton : Button
    {
        private readonly Board.Cell r_BoardLocation;

        internal CellButton(int i_Row, int i_Column)
        {
            r_BoardLocation = new Board.Cell(i_Row, i_Column);
        }

        internal Board.Cell BoardLocation
        {
            get
            {
                return r_BoardLocation;
            }
        }
    }
}
