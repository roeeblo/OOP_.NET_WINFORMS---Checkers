using System;
using System.Text.RegularExpressions;

namespace CheckersLogics
{
    internal class MoveDetails
    {
        internal bool m_IsCaptureMove { get; set; }
        private int r_StartRowIndex;
        private int r_StartColIndex;
        private int r_EndRowIndex;
        private int r_EndColIndex;
        private ePieceType m_PieceMovedSymbol;

        // constructor
        public MoveDetails(string i_MoveInput, Board i_Board)
        {            
            try
            {
                validateMoveFormat(i_MoveInput);
                initializeDataMembers(i_MoveInput);
                isMoveInBounds(i_Board.BoardSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            m_PieceMovedSymbol = i_Board.BoardMatrix[r_StartRowIndex, r_StartColIndex];
        }

        public int StartRowIndex { get { return r_StartRowIndex; } }

        public int StartColIndex { get { return r_StartColIndex; } }

        public int EndRowIndex { get { return r_EndRowIndex; } }

        public int EndColIndex { get { return r_EndColIndex; } }

        public ePieceType PieceMovedSymbol { get { return m_PieceMovedSymbol; } }       

        private void initializeDataMembers(string i_MoveInput)
        {
            string[] movePositions;
            string startPos, endPos;
            char startRow, startCol, endRow, endCol;

            movePositions = i_MoveInput.Split('>');
            startPos = movePositions[0].Trim();
            endPos = movePositions[1].Trim();
            startRow = startPos[0];
            startCol = startPos[1];
            endRow = endPos[0];
            endCol = endPos[1];
            r_StartRowIndex = startRow - 'A';
            r_StartColIndex = startCol - 'a';
            r_EndRowIndex = endRow - 'A';
            r_EndColIndex = endCol - 'a';
            m_IsCaptureMove = false;
        }

        internal void UpgradePieceToKing() 
        {
            m_PieceMovedSymbol = m_PieceMovedSymbol == ePieceType.RegularX ? ePieceType.KingX : ePieceType.KingO;
        }

        private void isMoveInBounds(int i_BoardSize)
        {
            if (r_StartRowIndex < 0 || r_StartRowIndex >= i_BoardSize ||
                r_StartColIndex < 0 || r_StartColIndex >= i_BoardSize ||
                r_EndRowIndex < 0 || r_EndRowIndex >= i_BoardSize ||
                r_EndColIndex < 0 || r_EndColIndex >= i_BoardSize)
            {
                throw new Exception("Move out of bounds.");
            }
        }

        private void validateMoveFormat(string i_MoveInput)
        {
            string validMovePattern = @"^[A-Ja-j]([a-j])>[A-Ja-j]([a-j])$";
            Regex regex = new Regex(validMovePattern);
            Match match = regex.Match(i_MoveInput);

            if (!match.Success)
            {
                throw new Exception("Invalid format. Please use the format: RowCol>RowCol (e.g. 'Fb>Fa')");
            }
        }

        public override string ToString()
        {
            string start, end, moveResult;
            
            start = $"{(char)(r_StartRowIndex + 'A')}{(char)(r_StartColIndex + 'a')}";
            end = $"{(char)(r_EndRowIndex + 'A')}{(char)(r_EndColIndex + 'a')}";
            moveResult = String.Format("{0}>{1}", start, end);

            return moveResult;
        }
    }
}