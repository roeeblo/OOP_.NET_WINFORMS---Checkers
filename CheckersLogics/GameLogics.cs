using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckersLogics
{
    public class GameLogics
    {
        public Player m_CurrentPlayer { get; set; }
        private Player m_PlayerOne;
        private Player m_PlayerTwo;
        private Board m_Board;       
        private MoveDetails m_MoveDetails;
        private Random m_Random;

        public GameLogics(Player i_PlayerOne, Player i_PlayerTwo, int i_BoardSize, bool i_TwoPlayerGame)
        {
            m_PlayerOne = i_PlayerOne;
            m_Board = new Board(i_BoardSize);
            m_PlayerTwo = i_PlayerTwo;
            m_PlayerOne.SetInitalTotalPiecesNumber(i_BoardSize);
            m_PlayerTwo.SetInitalTotalPiecesNumber(i_BoardSize);
            m_CurrentPlayer = i_PlayerOne; // PlayerOne start by default
            m_MoveDetails = null;
            m_Random = new Random();
        }

        public Player OppositePlayer
        {
            get { return m_CurrentPlayer == m_PlayerOne ? m_PlayerTwo : m_PlayerOne; }
        }

        public Board Board { get { return m_Board; } }

        public Player PlayerOne { get { return m_PlayerOne; } }

        public Player PlayerTwo {  get { return m_PlayerTwo; } }

        public void ResetGameVariables()
        {
            int boardSize = m_Board.BoardSize;

            m_Board = new Board(boardSize);
            m_PlayerOne.SetInitalTotalPiecesNumber(boardSize);
            m_PlayerTwo.SetInitalTotalPiecesNumber(boardSize);
            m_CurrentPlayer = m_PlayerTwo;
        }

        private void updateGameBoardMatrix()
        {
            int capturedRow, capturedCol;
            ePieceType[,] boardMatrix = m_Board.BoardMatrix;
            int startRowIndex, startColIndex, endRowIndex, endColIndex;

            startRowIndex = m_MoveDetails.StartRowIndex;
            startColIndex = m_MoveDetails.StartColIndex;
            endRowIndex = m_MoveDetails.EndRowIndex;
            endColIndex = m_MoveDetails.EndColIndex;
            boardMatrix[startRowIndex, startColIndex] = ePieceType.Empty;

            if (Math.Abs(startRowIndex - endRowIndex) == 2 &&
                Math.Abs(startColIndex - endColIndex) == 2)
            {
                capturedRow = (startRowIndex + endRowIndex) / 2;
                capturedCol = (startColIndex + endColIndex) / 2;
                boardMatrix[capturedRow, capturedCol] = ePieceType.Empty;
            }

            if ((endRowIndex == 0 && m_MoveDetails.PieceMovedSymbol == ePieceType.RegularX) || (endRowIndex == m_Board.BoardSize - 1 && m_MoveDetails.PieceMovedSymbol == ePieceType.RegularO))
            {
                m_MoveDetails.UpgradePieceToKing();
            }

            boardMatrix[endRowIndex, endColIndex] = m_MoveDetails.PieceMovedSymbol;
        }

        private bool isMoveEndPositionEmpty(MoveDetails i_MoveDetails)
        {
            return m_Board.BoardMatrix[i_MoveDetails.EndRowIndex, i_MoveDetails.EndColIndex] == ePieceType.Empty;
        }

        private bool isMoveDiagonalForward(MoveDetails i_MoveDetails)
        {
            int rowDiff = Math.Abs(i_MoveDetails.StartRowIndex - i_MoveDetails.EndRowIndex);
            int colDiff = Math.Abs(i_MoveDetails.StartColIndex - i_MoveDetails.EndColIndex);
            bool isDiagonal = rowDiff == colDiff && rowDiff != 0;
            bool isSingleTileMove = rowDiff == 1;
            bool isCaptureMove = rowDiff == 2 && this.isCaptureMove(i_MoveDetails);
            bool isForwardMove = this.isForwardMove(i_MoveDetails);

            return isForwardMove && isDiagonal && (isSingleTileMove || isCaptureMove);
        }

        private bool isForwardMove(MoveDetails i_MoveDetails)
        {
            bool isForwardMove = true;
            ePieceType pieceType = m_Board.BoardMatrix[i_MoveDetails.StartRowIndex, i_MoveDetails.StartColIndex];

            if (!isKingPiece(pieceType))
            {
                if (i_MoveDetails.EndRowIndex > i_MoveDetails.StartRowIndex && i_MoveDetails.PieceMovedSymbol == ePieceType.RegularX)
                {
                    isForwardMove = false;
                }
                else if (i_MoveDetails.EndRowIndex < i_MoveDetails.StartRowIndex && i_MoveDetails.PieceMovedSymbol == ePieceType.RegularO)
                {
                    isForwardMove = false;
                }
            }

            return isForwardMove;
        }

        private bool isCaptureMove(MoveDetails i_MoveDetails)
        {
            int middleRow = (i_MoveDetails.StartRowIndex + i_MoveDetails.EndRowIndex) / 2;
            int middleCol = (i_MoveDetails.StartColIndex + i_MoveDetails.EndColIndex) / 2;
            ePieceType middlePieceSymbol = m_Board.BoardMatrix[middleRow, middleCol];
            bool isMiddleOpponentPiece = !m_CurrentPlayer.CheckIfOwnPiece(middlePieceSymbol);
            bool isTargetEmpty = isMoveEndPositionEmpty(i_MoveDetails);

            return isMiddleOpponentPiece && isTargetEmpty && middlePieceSymbol != ePieceType.Empty;
        }

        public static bool ValidateYesNoSelectionInput(string i_UserInput)
        {
            string inputToUpper = i_UserInput?.ToUpper();

            if (string.IsNullOrWhiteSpace(inputToUpper) || (inputToUpper != "Y" && inputToUpper != "N"))
            {
                throw new Exception("Invalid input! Please select Y or N.");
            }

            return true;
        }

        public bool IsCaptureMoveAvailableOnBoard()
        {
            bool isCaptureMoveAvailable = false;
            int boardSize = m_Board.BoardSize;

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (!m_CurrentPlayer.CheckIfOwnPiece(m_Board.BoardMatrix[i, j]))
                    {
                        continue;
                    }

                    if (CheckIfPieceCanCapture(i, j))
                    {
                        isCaptureMoveAvailable = true;
                        break;
                    }
                }

                if (isCaptureMoveAvailable)
                {
                    break;
                }
            }

            return isCaptureMoveAvailable;
        }

        public bool CheckIfPieceCanCapture(int i_RowIndex, int i_ColIndex)
        {
            bool canCapture = false;
            MoveDetails potentialMove;
            int potentialEndRowIndex, potentialEndColIndex;
            int[] rowOffsets = { -2, -2, 2, 2 };
            int[] colOffsets = { -2, 2, -2, 2 };
            int offsetsLength;

            if (!isKingPiece(m_Board.BoardMatrix[i_RowIndex, i_ColIndex]))
            {
                if (m_CurrentPlayer.CheckIfOwnPiece(ePieceType.RegularX))
                {
                    rowOffsets = new int[] { -2, -2 };
                    colOffsets = new int[] { -2, 2 };
                }
                else
                {
                    rowOffsets = new int[] { 2, 2 };
                    colOffsets = new int[] { -2, 2 };
                }
            }

            offsetsLength = rowOffsets.Length;

            for (int i = 0; i < offsetsLength; i++)
            {
                potentialEndRowIndex = i_RowIndex + rowOffsets[i];
                potentialEndColIndex = i_ColIndex + colOffsets[i];

                if (potentialEndRowIndex < 0 || potentialEndRowIndex >= m_Board.BoardSize ||
                    potentialEndColIndex < 0 || potentialEndColIndex >= m_Board.BoardSize)
                {
                    continue;
                }

                potentialMove = new MoveDetails(
                    $"{(char)(i_RowIndex + 'A')}{(char)(i_ColIndex + 'a')}>{(char)(potentialEndRowIndex + 'A')}{(char)(potentialEndColIndex + 'a')}",
                    m_Board);

                if (isCaptureMove(potentialMove))
                {
                    canCapture = true;
                    break;
                }
            }

            return canCapture;
        }

        private bool isKingPiece(ePieceType i_PieceType)
        {
            return i_PieceType == ePieceType.KingX || i_PieceType == ePieceType.KingO;
        }

        public void InitiateGameForfiet()
        {
            OppositePlayer.AddToTotalPoints(calculatePointsFromPieces(OppositePlayer));

            throw new Exception(String.Format("{0} has quit the game. {1} WON!", m_CurrentPlayer.PlayerName, OppositePlayer.PlayerName));
        }

        private int calculatePointsFromPieces(Player i_PlayerToCheck)
        {
            int totalPlayerPoints, totalOpponentPoints, resultPoints;

            totalPlayerPoints = totalOpponentPoints = 0;

            foreach (ePieceType piece in m_Board.BoardMatrix)
            {
                if (i_PlayerToCheck.CheckIfOwnPiece(piece))
                {
                    totalPlayerPoints += isKingPiece(piece) ? 4 : 1;
                }
                else if (piece != ePieceType.Empty)
                {
                    totalOpponentPoints += isKingPiece(piece) ? 4 : 1;
                }
            }

            resultPoints = totalPlayerPoints - totalOpponentPoints;

            return resultPoints < 0 ? 0 : resultPoints;
        }

        public void CheckGameOverConditions()
        {
            HashSet<MoveDetails> currentPlayerAvailableMoves;
            HashSet<MoveDetails> oppositePlayerAvailableMoves;
            int currentPlayerPoints, oppositePlayerPoints;
            string currentPlayerName, oppositePlayerName;

            currentPlayerName = m_CurrentPlayer.PlayerName;
            oppositePlayerName = OppositePlayer.PlayerName;
            currentPlayerPoints = calculatePointsFromPieces(m_CurrentPlayer);
            oppositePlayerPoints = calculatePointsFromPieces(OppositePlayer);

            if (OppositePlayer.TotalPiecesLeft == 0)
            {
                m_CurrentPlayer.AddToTotalPoints(currentPlayerPoints);
                throw new Exception(String.Format("{0} has no pieces left. {1} WON!", oppositePlayerName, currentPlayerName));
            }

            oppositePlayerAvailableMoves = findAvailableMovesOnBoard(OppositePlayer);

            if (oppositePlayerAvailableMoves.Count == 0)
            {
                currentPlayerAvailableMoves = findAvailableMovesOnBoard(m_CurrentPlayer);
                if (currentPlayerAvailableMoves.Count == 0)
                {
                    if (currentPlayerPoints > oppositePlayerPoints)
                    {
                        m_CurrentPlayer.AddToTotalPoints(currentPlayerPoints);
                        throw new Exception(String.Format("None has available moves. By points calculation {0} WON!", currentPlayerName));
                    }
                    else if (currentPlayerPoints < oppositePlayerPoints)
                    {
                        OppositePlayer.AddToTotalPoints(oppositePlayerPoints);
                        throw new Exception(String.Format("None has available moves. By points calculation {0} WON!", oppositePlayerName));
                    }

                    throw new Exception("None has avilable moves. TIE!");
                }

                m_CurrentPlayer.AddToTotalPoints(currentPlayerPoints);
                throw new Exception(String.Format("{0} has no available moves. {1} WON!", oppositePlayerName, currentPlayerName));
            }
        }

        private HashSet<MoveDetails> findAvailableMovesOnBoard(Player i_PlayerToCheck)
        {
            int boardSize = m_Board.BoardSize;
            HashSet<MoveDetails> availableRegularMoves = new HashSet<MoveDetails>();
            HashSet<MoveDetails> availableCaptureMoves = new HashSet<MoveDetails>();
            HashSet<MoveDetails> availableMoves;
            const bool v_CaptureCheck = true;
            ePieceType piece;

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    piece = m_Board.BoardMatrix[row, col];

                    if (!i_PlayerToCheck.CheckIfOwnPiece(piece))
                    {
                        continue;
                    }

                    getOffsetsForPiece(piece, out int[] regularRowOffsets, out int[] regularColOffsets,
                                     out int[] captureRowOffsets, out int[] captureColOffsets);

                    // Add available capture moves
                    addValidMoves(row, col, captureRowOffsets, captureColOffsets, availableCaptureMoves, v_CaptureCheck);
                    // Add available regular moves
                    addValidMoves(row, col, regularRowOffsets, regularColOffsets, availableRegularMoves, !v_CaptureCheck);
                }
            }

            availableMoves = availableCaptureMoves.Count != 0 ? availableCaptureMoves : availableRegularMoves;

            return availableMoves;
        }

        private void getOffsetsForPiece(ePieceType i_Piece, out int[] o_RegularRowOffsets, out int[] o_RegularColOffsets, out int[] o_CaptureRowOffsets, out int[] o_CaptureColOffsets)
        {
            if (isKingPiece(i_Piece))
            {
                o_RegularRowOffsets = new[] { -1, -1, 1, 1 };
                o_RegularColOffsets = new[] { -1, 1, -1, 1 };
                o_CaptureRowOffsets = new[] { -2, -2, 2, 2 };
                o_CaptureColOffsets = new[] { -2, 2, -2, 2 };
            }
            else if (i_Piece == ePieceType.RegularX)
            {
                o_RegularRowOffsets = new[] { -1, -1 };
                o_RegularColOffsets = new[] { -1, 1 };
                o_CaptureRowOffsets = new[] { -2, -2 };
                o_CaptureColOffsets = new[] { -2, 2 };
            }
            else // PieceType.RegularO
            {
                o_RegularRowOffsets = new[] { 1, 1 };
                o_RegularColOffsets = new[] { -1, 1 };
                o_CaptureRowOffsets = new[] { 2, 2 };
                o_CaptureColOffsets = new[] { -2, 2 };
            }
        }

        private void addValidMoves(int i_StartRow, int i_StartCol, int[] i_RowOffsets, int[] i_ColOffsets, HashSet<MoveDetails> o_AvailableMoves, bool i_IsCaptureCheck)
        {
            int endRow, endCol, capturedRow, capturedCol;
            ePieceType capturedPieceSymbol;

            for (int i = 0; i < i_RowOffsets.Length; i++)
            {
                endRow = i_StartRow + i_RowOffsets[i];
                endCol = i_StartCol + i_ColOffsets[i];
                capturedRow = (i_StartRow + endRow) / 2;
                capturedCol = (i_StartCol + endCol) / 2;

                if (!isWithinBounds(endRow, endCol) || m_Board.BoardMatrix[endRow, endCol] != ePieceType.Empty)
                {
                    continue;
                }

                capturedPieceSymbol = m_Board.BoardMatrix[capturedRow, capturedCol];

                string move = $"{(char)(i_StartRow + 'A')}{(char)(i_StartCol + 'a')}>{(char)(endRow + 'A')}{(char)(endCol + 'a')}";
                MoveDetails potentialMove = new MoveDetails(move, m_Board);

                if (i_IsCaptureCheck && !isCaptureMove(potentialMove))
                {
                    continue;
                }

                o_AvailableMoves.Add(potentialMove);
            }
        }

        private bool isWithinBounds(int row, int col)
        {
            return row >= 0 && row < m_Board.BoardSize && col >= 0 && col < m_Board.BoardSize;
        }

        public string HandleComputerTurnLogics()
        {
            StringBuilder movesString = new StringBuilder();
            HashSet<MoveDetails> availableMoves = findAvailableMovesOnBoard(m_PlayerTwo);

            selectRandomMove(availableMoves);
            updateGameBoardMatrix();
            movesString.Append(m_MoveDetails.ToString() + " ");

            if (m_MoveDetails.m_IsCaptureMove)
            {
                handleCaptureSequence(movesString);
            }        

            return movesString.ToString();
        }

        private void selectRandomMove(HashSet<MoveDetails> i_AvailableMoves)
        {
            
            List<MoveDetails> availableMovesList = i_AvailableMoves.ToList();
            int randomIndex = m_Random.Next(availableMovesList.Count);

            m_MoveDetails = availableMovesList[randomIndex];
            m_MoveDetails.m_IsCaptureMove = isCaptureMove(m_MoveDetails);
        }

        private void handleCaptureSequence(StringBuilder o_MovesString)
        {
            const bool v_IsCaptureCheck = true;

            do
            {
                OppositePlayer.UpdateTotalPiecesNumberAfterCapture();

                HashSet<MoveDetails> captureMoves = new HashSet<MoveDetails>();
                getOffsetsForPiece(m_MoveDetails.PieceMovedSymbol, out int[] regularRowOffsets, out int[] regularColOffsets, out int[] captureRowOffsets, out int[] captureColOffsets);
                addValidMoves(m_MoveDetails.EndRowIndex, m_MoveDetails.EndColIndex, captureRowOffsets, captureColOffsets, captureMoves, v_IsCaptureCheck);

                if (captureMoves.Count == 0)
                {
                    break;
                }

                selectRandomMove(captureMoves);
                updateGameBoardMatrix();
                o_MovesString.Append(m_MoveDetails.ToString() + " ");
            }
            while (CheckIfPieceCanCapture(m_MoveDetails.EndRowIndex, m_MoveDetails.EndColIndex));
        }

        public bool HandlePlayerTurnLogics(string i_MoveInput)
        {
            bool canDoExtraMove = false;
            MoveDetails lastMoveDetails = null;
            
            if (m_MoveDetails != null)
            {
                lastMoveDetails = m_MoveDetails;
            }            

            try
            {
                m_MoveDetails = new MoveDetails(i_MoveInput, m_Board);

                if(isCaptureMove(m_MoveDetails))
                {
                    m_MoveDetails.m_IsCaptureMove = true;
                    OppositePlayer.UpdateTotalPiecesNumberAfterCapture();
                }

                validateMoveRulesIntegrity(lastMoveDetails);
            }
            catch (Exception ex)
            {
                m_MoveDetails = null;
                throw new Exception(ex.Message);
            }
            
            if (CheckIfPieceCanCapture(m_MoveDetails.EndRowIndex, m_MoveDetails.EndColIndex) && m_MoveDetails.m_IsCaptureMove)
            {
                canDoExtraMove = true;
            }

            updateGameBoardMatrix();

            return canDoExtraMove;
        }

        private void validateMoveRulesIntegrity(MoveDetails i_LastMoveDetails)
        {
            if (!m_CurrentPlayer.CheckIfOwnPiece(m_MoveDetails.PieceMovedSymbol))
            {
                throw new Exception("Invalid move. Can only move your own pieces.");
            }

            if (!isMoveEndPositionEmpty(m_MoveDetails))
            {
                throw new Exception("Invalid move. Space is occupied.");
            }

            if (!isMoveDiagonalForward(m_MoveDetails))
            {
                throw new Exception("Invalid move. Can only move one tile diagonally forward (unless king/capturing).");
            }

            if (IsCaptureMoveAvailableOnBoard() && !m_MoveDetails.m_IsCaptureMove)
            {
                throw new Exception("Invalid move. Must perform capture move when available.");
            }

            if(i_LastMoveDetails != null)
            {
                if (m_CurrentPlayer.CheckIfOwnPiece(i_LastMoveDetails.PieceMovedSymbol) && !m_MoveDetails.m_IsCaptureMove &&
                i_LastMoveDetails.EndRowIndex != m_MoveDetails.StartRowIndex && i_LastMoveDetails.EndColIndex != m_MoveDetails.StartColIndex)
                {
                    throw new Exception("Invalid move. Extra turn must be a capture with same piece.");
                }
            }          
        }
    }
}