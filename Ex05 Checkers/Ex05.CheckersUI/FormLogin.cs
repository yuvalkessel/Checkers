using System;
using System.Windows.Forms;
using System.Drawing;
using System.Media;

namespace Ex05.CheckersUI
{
    internal class FormLogin : Form
    {
        private readonly SoundPlayer r_IllegalSound = new SoundPlayer(Properties.Resources.ErrorSound);
        private readonly Label labelBoardSize = new Label();
        private readonly Label labelPlayers = new Label();
        private readonly Label labelPlayer1 = new Label();
        private readonly CheckBox checkBoxPlayer2 = new CheckBox();
        private readonly TextBox textBoxPlayer1Name = new TextBox();
        private readonly TextBox textBoxPlayer2Name = new TextBox();
        private readonly RadioButton radioButtonSize6 = new RadioButton();
        private readonly RadioButton radioButtonSize8 = new RadioButton();
        private readonly RadioButton radioButtonSize10 = new RadioButton();
        private readonly Button buttonDone = new Button();
        
        internal FormLogin()
        {
            initializeComponents();
        }

        private void initializeComponents()
        {
            SuspendLayout();
            initializeLoginForm();
            initializeBoardSizeLabel();
            initializePlayersLabel();
            initializePlayer1Label();
            initializePlayer2CheckBox();
            initializePlayer1NameTextBox();
            initializePlayer2NameTextBox();
            initializeSize6RadioButton();
            initializeSize8RadioButton();
            initializeSize10RadioButton();
            initializeDoneButton();
            Controls.AddRange(new Control[]
            {
                labelBoardSize,
                radioButtonSize6,
                radioButtonSize8,
                radioButtonSize10,
                labelPlayers,
                labelPlayer1,
                checkBoxPlayer2,
                textBoxPlayer1Name,
                textBoxPlayer2Name,
                buttonDone
            });
            ResumeLayout(false);
        }

        private void initializeBoardSizeLabel()
        {
            labelBoardSize.Font = new Font("Segoe UI", 10.875F, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            labelBoardSize.Location = new Point(32, 24);
            labelBoardSize.Size = new Size(170, 40);
            labelBoardSize.Text = "Board Size:";
            labelBoardSize.AutoSize = true;
            labelBoardSize.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void initializePlayersLabel()
        {
            labelPlayers.Font = new Font("Segoe UI", 10.875F, FontStyle.Bold);
            labelPlayers.Location = new Point(32, 160);
            labelPlayers.Size = new Size(124, 40);
            labelPlayers.Text = "Players:";
            labelPlayers.AutoSize = true;
            labelPlayers.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void initializePlayer1Label()
        {
            labelPlayer1.Font = new Font("Segoe UI", 9F);
            labelPlayer1.Location = new Point(65, 227);
            labelPlayer1.Size = new Size(104, 39);
            labelPlayer1.Text = "Player 1:";
            labelPlayer1.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void initializePlayer2CheckBox()
        {
            checkBoxPlayer2.Font = new Font("Segoe UI", 9F);
            checkBoxPlayer2.Location = new Point(71, 291);
            checkBoxPlayer2.Size = new Size(139, 39);
            checkBoxPlayer2.Text = "Player 2:";
            checkBoxPlayer2.CheckStateChanged += new EventHandler(player2CheckBox_Changed);
        }

        private void initializePlayer1NameTextBox()
        {
            textBoxPlayer1Name.Font = new Font("Segoe UI", 9F);
            textBoxPlayer1Name.Location = new Point(233, 227);
            textBoxPlayer1Name.Size = new Size(220, 39);
            textBoxPlayer1Name.TextAlign = HorizontalAlignment.Center;
        }

        private void initializePlayer2NameTextBox()
        {
            textBoxPlayer2Name.Font = new Font("Segoe UI", 9F);
            textBoxPlayer2Name.Location = new Point(233, 291);
            textBoxPlayer2Name.Size = new Size(220, 39);
            textBoxPlayer2Name.Text = "[Computer]";
            textBoxPlayer2Name.Enabled = false;
            textBoxPlayer2Name.TextAlign = HorizontalAlignment.Center;
        }

        private void initializeSize6RadioButton()
        {
            radioButtonSize6.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            radioButtonSize6.Location = new Point(65, 87);
            radioButtonSize6.Size = new Size(97, 36);
            radioButtonSize6.Text = "6 x 6";
            radioButtonSize6.AutoSize = true;
            radioButtonSize6.Checked = true;
        }

        private void initializeSize8RadioButton()
        {
            radioButtonSize8.Font = new Font("Segoe UI", 9F);
            radioButtonSize8.Location = new Point(195, 87);
            radioButtonSize8.Size = new Size(97, 36);
            radioButtonSize8.Text = "8 x 8";
            radioButtonSize8.AutoSize = true;
        }

        private void initializeSize10RadioButton()
        {
            radioButtonSize10.Font = new Font("Segoe UI", 9F);
            radioButtonSize10.Location = new Point(325, 87);
            radioButtonSize10.Size = new Size(123, 36);
            radioButtonSize10.Text = "10 x 10";
            radioButtonSize10.AutoSize = true;
        }

        private void initializeDoneButton()
        {
            buttonDone.Font = new Font("Segoe UI", 10.125F, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            buttonDone.Location = new Point(291, 369);
            buttonDone.Size = new Size(162, 44);
            buttonDone.Text = "Done";
            buttonDone.Click += new EventHandler(doneButton_Click);
        }

        private void initializeLoginForm()
        {
            AutoScaleDimensions = new SizeF(12F, 25F);
            ClientSize = new Size(494, 450);
            Text = "Game Settings";
            AutoScaleMode = AutoScaleMode.Font;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            FormClosed += new FormClosedEventHandler(exitForm_Click);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
            Icon = Properties.Resources.ChessBoard;
        }

        private void exitForm_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            FormGame formGame;

            if (checkButtonClickValidation(textBoxPlayer1Name) && checkButtonClickValidation(textBoxPlayer2Name))
            {
                FormClosed -= exitForm_Click;
                this.Close();
                this.Hide();
                formGame = new FormGame(textBoxPlayer1Name.Text, textBoxPlayer2Name.Text, getSelectedBoardSize());
                formGame.ShowDialog();
            }
        }

        private bool checkButtonClickValidation(TextBox i_PlayerNameTextBox)
        {
            bool isValid = true;

            if (i_PlayerNameTextBox.Text.Equals(string.Empty))
            {
                r_IllegalSound.Play();
                MessageBox.Show("Please enter the player's name in the relevant text box.");
                isValid = false;
            }
            else if (i_PlayerNameTextBox.Text.Length > 10 || i_PlayerNameTextBox.Text.Contains(" ") || i_PlayerNameTextBox.Text.Contains("\t"))
            {
                r_IllegalSound.Play();
                MessageBox.Show(string.Format(
                    "The player's name you entered is invalid.{0}It should be spaces free and max 10 characters length.{0}Please try again.",
                    Environment.NewLine));
                isValid = false;
            }

            return isValid;
        }

        private void player2CheckBox_Changed(object sender, EventArgs e)
        {
            if (checkBoxPlayer2.Checked)
            {
                textBoxPlayer2Name.Enabled = true;
                textBoxPlayer2Name.Text = string.Empty;
            }
            else
            {
                textBoxPlayer2Name.Enabled = false;
                textBoxPlayer2Name.Text = "[Computer]";
            }
        }

        private int getSelectedBoardSize()
        {
            int selectedBoardSize;

            if (radioButtonSize6.Checked)
            {
                selectedBoardSize = 6;
            }
            else if (radioButtonSize8.Checked)
            {
                selectedBoardSize = 8;
            }
            else
            {
                selectedBoardSize = 10;
            }

            return selectedBoardSize;
        }
    }
}
