using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckersLogics
{
    public class Player
    {
        public const int k_MaxUserNameLength = 20;
        private string m_PlayerName;
        private int m_TotalPoints;
        private int m_TotalPieces;
        private bool m_IsComputer;
        private string m_Symbol;
        private HashSet<ePieceType> m_Pieces;

        // constructor
        public Player(string i_UserNameInput, bool i_TwoPlayersGame, bool i_IsPlayerOne, string i_Symbol)
        {
            ePieceType regularPiece, kingPiece;

            regularPiece = i_IsPlayerOne ? ePieceType.RegularX : ePieceType.RegularO;
            kingPiece = i_IsPlayerOne ? ePieceType.KingX : ePieceType.KingO;
            m_PlayerName = i_UserNameInput;
            m_TotalPieces = 0;
            m_IsComputer = !i_IsPlayerOne && !i_TwoPlayersGame;
            m_Symbol = i_Symbol;
            m_Pieces = new HashSet<ePieceType> { regularPiece, kingPiece };
        }

        public bool IsComputer { get { return m_IsComputer; } }

        public string PlayerName { get { return m_PlayerName; } }

        public string Symbol { get { return m_Symbol; } }

        public int TotalPiecesLeft { get { return m_TotalPieces; } }

        public int TotalPoints { get { return m_TotalPoints; } }

        public void SetInitalTotalPiecesNumber(int i_BoardSize)
        {
            m_TotalPieces = calculateInitialTotalPieces(i_BoardSize);
        }

        public void UpdateTotalPiecesNumberAfterCapture()
        {
            m_TotalPieces--;
        }

        private int calculateInitialTotalPieces(int i_BoardSize)
        {
            int rowsOccupied = i_BoardSize / 2 - 1;
            int piecesPerRow = i_BoardSize / 2;

            return rowsOccupied * piecesPerRow;
        }

        public bool CheckIfOwnPiece(ePieceType i_PieceType)
        {
            return m_Pieces.Contains(i_PieceType);
        }

        public static bool ValidateUserName(string i_userNameInput)
        {
            const int maxUserNameLength = Player.k_MaxUserNameLength;
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(i_userNameInput))
            {
                isValid = false;
                throw new Exception("User name cannot be empty. Please try again.");
            }
            else if (i_userNameInput.Length > maxUserNameLength)
            {
                isValid = false;
                throw new Exception($"User name cannot exceed {maxUserNameLength} characters. Please try again.");
            }
            else if (i_userNameInput.Any(char.IsWhiteSpace))
            {
                isValid = false;
                throw new Exception("User name cannot contain spaces. Please try again.");
            }

            return isValid;
        }

        public void AddToTotalPoints(int i_PointsFromPieces)
        {
            m_TotalPoints += i_PointsFromPieces;
        }
    }
}
