using CheckersLogics;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameUI
{
    internal class SettingsForm : Form
    {
        private Label lblBoardSize, lblPlayers, lblPlayer1, lblIcon1, lblIcon2;
        private RadioButton rb6x6, rb8x8, rb10x10;
        private TextBox txtPlayer1, txtPlayer2;
        private Button btnDone;
        private ComboBox cbIcons1, cbIcons2;
        private CheckBox cbPlayer2;
        private string[] m_Icons = { "X", "O", "😀", "😎", "😇", "🤖", "🐱", "🐶", "🎃", "👻", "🤡", "👽", "🤪" };

        public SettingsForm()
        {
            initializeCustomComponents();
        }

        private void initializeCustomComponents()
        {
            this.Font = new Font("Tahoma", 11);
            this.Text = "Game Settings";
            this.BackColor = Color.LightGray;
            this.Size = new Size(370, 350);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblBoardSize = new Label { Text = "Board Size:", Location = new Point(20, 20), Width = 100 };
            rb6x6 = new RadioButton { Text = "6 x 6", Location = new Point(20, 45), Checked = true };
            rb8x8 = new RadioButton { Text = "8 x 8", Location = new Point(130, 45) };
            rb10x10 = new RadioButton { Text = "10 x 10", Location = new Point(235, 45) };

            lblPlayers = new Label { Text = "Players:", Location = new Point(20, 75), Width = 100 };
            lblPlayer1 = new Label { Text = "Player 1:", Location = new Point(30, 105) };
            txtPlayer1 = new TextBox { Location = new Point(130, 100), Size = new Size(200, 30) };

            cbPlayer2 = new CheckBox { Text = "Player 2:", Location = new Point(20, 137), Checked = false };
            cbPlayer2.CheckedChanged += cbPlayer2_CheckedChanged;

            txtPlayer2 = new TextBox { Location = new Point(130, 135), Size = new Size(200, 30), Text = "[Computer]", Enabled = false };

            lblIcon1 = new Label { Text = "P1 Icon:", Location = new Point(20, 190) };
            cbIcons1 = new ComboBox { Location = new Point(130, 185), DropDownStyle = ComboBoxStyle.DropDownList, Width = 200 };
            cbIcons1.Items.AddRange(m_Icons);
            cbIcons1.SelectedIndex = 0;

            lblIcon2 = new Label { Text = "P2 Icon:", Location = new Point(20, 220) };
            cbIcons2 = new ComboBox { Location = new Point(130, 215), DropDownStyle = ComboBoxStyle.DropDownList, Width = 200 };
            cbIcons2.Items.AddRange(m_Icons);
            cbIcons2.SelectedIndex = 1;

            btnDone = new Button { Text = "Done", Location = new Point(150, 250), Size = new Size(100, 40) };

            Controls.AddRange(new Control[] { lblBoardSize, rb6x6, rb8x8, rb10x10, lblPlayers, lblPlayer1, txtPlayer1, cbPlayer2, txtPlayer2, lblIcon1, cbIcons1, lblIcon2, cbIcons2, btnDone });

            btnDone.Click += btnDone_Click;
        }

        private void cbPlayer2_CheckedChanged(object sender, EventArgs e)
        {
            txtPlayer2.Enabled = cbPlayer2.Checked;

            if (!cbPlayer2.Checked)
            {
                txtPlayer2.Text = "[Computer]";
            }
            else
            {
                txtPlayer2.Text = "";
            }
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            const bool v_IsPlayerOne = true;
            int boardSize = rb8x8.Checked ? 8 : rb10x10.Checked ? 10 : 6;
            string player1Name = string.IsNullOrEmpty(txtPlayer1.Text) ? "Player 1" : txtPlayer1.Text;
            string player2Name = cbPlayer2.Checked ? string.IsNullOrEmpty(txtPlayer2.Text) ? "Player 2" : txtPlayer2.Text : "Computer";
            bool twoPlayerGame = cbPlayer2.Checked;
            string player1Icon = cbIcons1.SelectedItem.ToString();
            string player2Icon = cbIcons2.SelectedItem.ToString();

            if (player1Icon == player2Icon)
            {
                MessageBox.Show("Can't use same icons");
            }
            else
            {
                Player player1 = new Player(player1Name, twoPlayerGame, v_IsPlayerOne, player1Icon);
                Player player2 = new Player(player2Name, twoPlayerGame, !v_IsPlayerOne, player2Icon);

                GameLogics gameLogics = new GameLogics(player1, player2, boardSize, twoPlayerGame);

                GameForm gameForm = new GameForm(gameLogics);
                gameForm.Show();
                this.Hide();
            }
        }
    }
}