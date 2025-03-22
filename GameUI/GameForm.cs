using System;
using System.Drawing;
using System.Windows.Forms;
using CheckersLogics;

namespace GameUI
{
    internal class GameForm : Form
    {
        private const int k_ButtonSize = 60;
        private const int k_BoardHorizontalOffset = 20;
        private const int k_BoardVerticalOffset = 40;
        private const int k_ScoreHorizontalOffset = 80;
        private const int k_ScoreVerticalOffset = 8;
        private const int k_FormPaddingHorizontal = 60;
        private const int k_FormPaddingVertical = 100;
        private const string k_FontName = "Tahoma";
        private const int k_FormFontSize = 15;
        private const int k_ButtonFontSize = 20;
        private const string k_FormTitle = "Damka";    
        private readonly Color r_SelectedBtnColor = Color.Aqua;
        private readonly Color r_DefaultPieceBtnColor = Color.Gray;
        private readonly Color r_DefualtNonPieceBtnColor = Color.LightGray;
        private GameLogics m_GameLogics;
        private Button[,] m_ButtonBoard;
        private EventHandler OnButtonBoardClick;
        private string m_Player1Symbol;
        private string m_Player2Symbol;
        private Button m_SelectedButton = null;
        private bool m_ButtonWasClicked = false;
        private Font m_FormFont;
        private Font m_ButtonFont;
        private Label player1Score;
        private Label player2Score;       

        internal GameForm(GameLogics i_GameLogics)
        {
            m_GameLogics = i_GameLogics;
            initializeCustomComponents();
        }

        private void initializeCustomComponents()
        {
            int boardSize = m_GameLogics.Board.BoardSize;

            m_FormFont = new Font(k_FontName, k_FormFontSize);
            m_ButtonFont = new Font(k_FontName, k_ButtonFontSize);
            m_Player1Symbol = m_GameLogics.PlayerOne.Symbol;
            m_Player2Symbol = m_GameLogics.PlayerTwo.Symbol;
            initializeForm(boardSize);
            initializeScoreLabels();
            initializeGameBoard(boardSize);
        }

        private void initializeForm(int i_BoardSize)
        {
            this.Text = k_FormTitle;
            this.Font = m_FormFont;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.Size = new Size(i_BoardSize * k_ButtonSize + k_FormPaddingHorizontal, i_BoardSize * k_ButtonSize + k_FormPaddingVertical);
        }

        private void initializeScoreLabels()
        {
            player1Score = new Label { AutoSize = true, Location = new Point(40, 8) };
            player2Score = new Label { AutoSize = true };
            updateScoreLabels();
            player2Score.Location = new Point(this.ClientSize.Width - player2Score.Width - k_ScoreHorizontalOffset, k_ScoreVerticalOffset);
            highlightCurrentPlayerScore();
            this.Controls.Add(player1Score);
            this.Controls.Add(player2Score);
        }

        private void initializeGameBoard(int i_BoardSize)
        {
            m_ButtonBoard = new Button[i_BoardSize, i_BoardSize];
            OnButtonBoardClick = new EventHandler(buttonBoard_Click);
            for (int i = 0; i < i_BoardSize; i++)
            {
                for (int j = 0; j < i_BoardSize; j++)
                {
                    initializeButton(i, j);
                }
            }
        }

        private void initializeButton(int i_Row, int i_Col)
        {
            m_ButtonBoard[i_Row, i_Col] = new Button
            {
                Size = new Size(k_ButtonSize, k_ButtonSize),
                Location = new Point(i_Col * k_ButtonSize + k_BoardHorizontalOffset, i_Row * k_ButtonSize + k_BoardVerticalOffset),
                Font = m_ButtonFont
            };

            m_ButtonBoard[i_Row, i_Col].Click += new EventHandler(buttonBoard_Click);
            this.Controls.Add(m_ButtonBoard[i_Row, i_Col]);
            updateBoardVisuals(i_Row, i_Col, m_GameLogics.Board.BoardMatrix[i_Row, i_Col]);


            // Disable buttons for non-playable squares
            if ((i_Row + i_Col) % 2 == 0)
            {
                m_ButtonBoard[i_Row, i_Col].Enabled = false;
            }
        }

        private void buttonBoard_Click(object sender, EventArgs e)
        {
            const bool v_IsSelectButton = true;
            Button clickedButton = sender as Button;

            if (clickedButton.BackColor != r_SelectedBtnColor && !string.IsNullOrEmpty(clickedButton.Text) && !m_ButtonWasClicked)
            {
                togglePieceSelection(clickedButton, v_IsSelectButton);
            }
            else if (clickedButton.BackColor == r_SelectedBtnColor)
            {
                togglePieceSelection(clickedButton, !v_IsSelectButton);
            }
            else if (m_ButtonWasClicked && clickedButton.BackColor == r_DefaultPieceBtnColor)
            {
                attemptMove(clickedButton);
            }
        }

        private void togglePieceSelection(Button i_ClickedButton, bool i_IsSelecting)
        {
            if (i_IsSelecting)
            {
                i_ClickedButton.BackColor = r_SelectedBtnColor;
                m_ButtonWasClicked = true;
                m_SelectedButton = i_ClickedButton;
            }
            else
            {
                i_ClickedButton.BackColor = r_DefaultPieceBtnColor;
                m_ButtonWasClicked = false;
            }
        }

        private void attemptMove(Button i_EndButton)
        {
            try
            {
                handleMove(i_EndButton);
            }
            catch (Exception ex)
            {
                handleMoveException(ex);
            }

            m_ButtonWasClicked = false;
        }

        private void handleMove(Button i_EndButton)
        {
            Point startButtonLocation = getButtonIndex(m_SelectedButton);
            Point endButtonLocation = getButtonIndex(i_EndButton);
            string moveString = createMoveString(startButtonLocation, endButtonLocation);

            handlePlayerTurn(moveString);

            if (m_GameLogics.m_CurrentPlayer.IsComputer)
            {
                handleComputerTurn();
            }
        }

        private string createMoveString(Point i_StartLocation, Point i_EndLocation)
        {
            return $"{(char)(i_StartLocation.X + 'A')}{(char)(i_StartLocation.Y + 'a')}>{(char)(i_EndLocation.X + 'A')}{(char)(i_EndLocation.Y + 'a')}";
        }

        private void handlePlayerTurn(string i_MoveString)
        {
            if (!m_GameLogics.HandlePlayerTurnLogics(i_MoveString))
            {
                switchTurns();
            }
            else
            {
                updateBoard();
            }
        }

        private void handleComputerTurn()
        {
            m_GameLogics.HandleComputerTurnLogics();
            switchTurns();
        }

        private void switchTurns()
        {
            updateBoard();
            checkGameStatus();
            m_GameLogics.m_CurrentPlayer = m_GameLogics.OppositePlayer;

            highlightCurrentPlayerScore();
        }

        private void highlightCurrentPlayerScore()
        {
            if (m_GameLogics.m_CurrentPlayer == m_GameLogics.PlayerOne)
            {
                player1Score.ForeColor = Color.Green;
                player2Score.ForeColor = Color.Black;
            }
            else
            {
                player1Score.ForeColor = Color.Black;
                player2Score.ForeColor = Color.Green;
            }
        }

        private void handleMoveException(Exception ex)
        {
            MessageBox.Show(ex.Message, "Invalid Move", MessageBoxButtons.OK);
            if (m_SelectedButton != null)
            {
                m_SelectedButton.BackColor = r_DefaultPieceBtnColor;
            }
        }

        private void checkGameStatus()
        {
            try
            {
                m_GameLogics.CheckGameOverConditions();
            }
            catch (Exception ex)
            {
                handleGameOver(ex);
            }
        }

        private void handleGameOver(Exception ex)
        {
            string msg = $"{ex.Message}{Environment.NewLine}Another Round?";
            DialogResult result = MessageBox.Show(msg, "Game Over", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                initiateGameReset();
            }
            else
            {
                Application.Exit();
            }
        }

        private void initiateGameReset()
        {
            m_GameLogics.ResetGameVariables();
            updateBoard();
            updateScoreLabels();
            m_SelectedButton = null;
            m_ButtonWasClicked = false;
        }

        private void updateBoard()
        {
            int boardSize = m_GameLogics.Board.BoardSize;

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    updateBoardVisuals(i, j, m_GameLogics.Board.BoardMatrix[i, j]);
                }
            }
        }

        private void updateBoardVisuals(int i_RowIndex, int i_ColIndex, ePieceType i_PieceType)
        {
            string buttonText = "";
            Font buttonFont = m_ButtonFont;

            if ((i_RowIndex + i_ColIndex) % 2 == 1)
            {
                m_ButtonBoard[i_RowIndex, i_ColIndex].BackColor = r_DefaultPieceBtnColor;
            }
            else
            {
                m_ButtonBoard[i_RowIndex, i_ColIndex].BackColor = r_DefualtNonPieceBtnColor;
            }

            switch (i_PieceType)
            {
                case ePieceType.Empty:
                    buttonText = "";
                    break;
                case ePieceType.RegularX:
                    buttonText = m_Player1Symbol;
                    break;
                case ePieceType.RegularO:
                    buttonText = m_Player2Symbol;
                    break;
                case ePieceType.KingX:
                    buttonText = string.Format("👑{0}{1}", Environment.NewLine, m_Player1Symbol);
                    buttonFont = new Font("Tahoma", 13);
                    break;
                case ePieceType.KingO:
                    buttonText = string.Format("👑{0}{1}", Environment.NewLine, m_Player2Symbol);
                    buttonFont = new Font("Tahoma", 13);
                    break;
            }

            m_ButtonBoard[i_RowIndex, i_ColIndex].Text = buttonText;
            m_ButtonBoard[i_RowIndex, i_ColIndex].Font = buttonFont;
            m_ButtonBoard[i_RowIndex, i_ColIndex].TextAlign = ContentAlignment.MiddleCenter;
        }

        private void updateScoreLabels()
        {
            player1Score.Text = string.Format("{0} {1}: {2}", m_Player1Symbol, m_GameLogics.PlayerOne.PlayerName, m_GameLogics.PlayerOne.TotalPoints);
            player2Score.Text = string.Format("{0} {1}: {2}", m_Player2Symbol, m_GameLogics.PlayerTwo.PlayerName, m_GameLogics.PlayerTwo.TotalPoints);
        }

        private Point getButtonIndex(Button i_Button)
        {
            int boardSize = m_GameLogics.Board.BoardSize;
            Point buttonPoint = new Point();

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (m_ButtonBoard[i, j] == i_Button)
                    {
                        buttonPoint = new Point(i, j);
                        return buttonPoint;
                    }
                }
            }
            return buttonPoint;
        }
    }
}