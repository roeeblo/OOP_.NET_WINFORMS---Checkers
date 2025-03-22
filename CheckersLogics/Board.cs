using System;

namespace CheckersLogics
{
    public class Board
    {
        private int m_BoardSize;
        private ePieceType[,] m_BoardMatrix;

        // constructor
        public Board(int i_BoardSize)
        {
            m_BoardSize = i_BoardSize;
            m_BoardMatrix = initializeMatrix();
        }

        public int BoardSize { get { return m_BoardSize; } }

        public ePieceType[,] BoardMatrix { get { return m_BoardMatrix; } }

        public static bool ValidateBoardSize(string i_InputSize)
        {
            bool isValid = true;

            if (i_InputSize != "6" && i_InputSize != "8" && i_InputSize != "10")
            {
                isValid = false;
                throw new Exception("Invalid size. Please enter 6, 8, or 10:");
            }

            return isValid;
        }

        private ePieceType[,] initializeMatrix()
        {
            ePieceType[,] BoardMatrix = new ePieceType[m_BoardSize, m_BoardSize];
            for (int i = 0; i < m_BoardSize; i++)
            {
                for (int j = 0; j < m_BoardSize; j++)
                {
                    // Fill top rows with 'O' and bottom rows with 'X', and leave middle rows empty
                    if (i < m_BoardSize / 2 - 1 && (i + j) % 2 == 1)
                    {
                        BoardMatrix[i, j] = ePieceType.RegularO;
                    }
                    else if (i > m_BoardSize / 2 && (i + j) % 2 == 1)
                    {
                        BoardMatrix[i, j] = ePieceType.RegularX;
                    }
                    else
                    {
                        BoardMatrix[i, j] = ePieceType.Empty;
                    }
                }
            }

            return BoardMatrix;
        }
    }
}
