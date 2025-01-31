using System;
using System.Drawing;
using System.Windows.Forms;

using CheckersGameLogic;

namespace CheckersUI
{
	public class FormGameSettings : Form
	{
		private Label m_LabelBoardSize;
		private RadioButton m_RadioButtonSize6;
		private RadioButton m_RadioButtonSize8;
		private RadioButton m_RadioButtonSize10;
		private Label m_LabelPlayers;
		private Label m_LabelPlayer1Name;
		private CheckBox m_CheckBoxPlayer2OrComputer;
		private TextBox m_TextBoxPlayer1Name;
		private TextBox m_TextBoxPlayer2Name;
		private Button m_ButtonDone;
		private int m_BoardSizeSelected;
		private bool m_ClosedByButtonDone = false;

		internal bool ClosedByButtonDone
		{
			get
			{
				return m_ClosedByButtonDone;
			}
			
			private set
			{
				m_ClosedByButtonDone = value;
			}
		}

		internal int BoardSizeSelected
		{
			get
			{
				return m_BoardSizeSelected;
			}

			set
			{
				m_BoardSizeSelected = value;
			}
		}

		internal string TextBoxPlayerOneName
		{
			get
			{
				return m_TextBoxPlayer1Name.Text;
			}
			set
			{
				m_TextBoxPlayer1Name.Text = value;
			}
		}

		internal string TextBoxPlayerTwoName
		{
			get
			{
				return m_TextBoxPlayer2Name.Text;
			}
			set
			{
				m_TextBoxPlayer2Name.Text = value;
			}
		}

		internal ePlayerType GameType
		{
			get
			{
				return m_CheckBoxPlayer2OrComputer.Checked == true ? ePlayerType.Human : ePlayerType.Cpu;
			}
		}
		internal FormGameSettings()
		{
			initializeComponents();
		}

		private void initializeComponents()
		{
			
			this.Icon = Properties.Resources.checkers;
			this.Text = "Game Settings";
			this.Size = new Size(280, 240);
			this.Location = new Point(16, 16);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.BackColor = Color.White;
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.ShowInTaskbar = false;

			m_LabelBoardSize = new Label();
			m_LabelBoardSize.Text = "Board Size:";
			m_LabelBoardSize.Top = 16;
			m_LabelBoardSize.Left = 16;
			this.Controls.Add(m_LabelBoardSize);

			m_RadioButtonSize6 = new RadioButton();
			m_RadioButtonSize6.Text = "6 x 6";
			m_RadioButtonSize6.AutoSize = true;
			m_RadioButtonSize6.Top = m_LabelBoardSize.Bottom + 5;
			m_RadioButtonSize6.Left = 32;
			m_RadioButtonSize6.Checked = true;
			this.Controls.Add(m_RadioButtonSize6);

			m_RadioButtonSize8 = new RadioButton();
			m_RadioButtonSize8.Text = "8 x 8";
			m_RadioButtonSize8.AutoSize = true;
			m_RadioButtonSize8.Top = m_LabelBoardSize.Bottom + 5;
			m_RadioButtonSize8.Left = m_RadioButtonSize6.Right + 24;
			this.Controls.Add(m_RadioButtonSize8);

			m_RadioButtonSize10 = new RadioButton();
			m_RadioButtonSize10.Text = "10 x 10";
			m_RadioButtonSize10.AutoSize = true;
			m_RadioButtonSize10.Top = m_LabelBoardSize.Bottom + 5;
			m_RadioButtonSize10.Left = m_RadioButtonSize8.Right + 24;
			this.Controls.Add(m_RadioButtonSize10);

			m_LabelPlayers = new Label();
			m_LabelPlayers.Text = "Players:";
			m_LabelPlayers.Top = m_RadioButtonSize6.Bottom + 10;
			m_LabelPlayers.Left = m_LabelBoardSize.Left;
			this.Controls.Add(m_LabelPlayers);

			m_LabelPlayer1Name = new Label();
			m_LabelPlayer1Name.Text = "Player 1:";
			m_LabelPlayer1Name.AutoSize = true;
			m_LabelPlayer1Name.Top = m_LabelPlayers.Bottom + 10;
			m_LabelPlayer1Name.Left = m_RadioButtonSize6.Left;
			this.Controls.Add(m_LabelPlayer1Name);

			m_CheckBoxPlayer2OrComputer = new CheckBox();
			m_CheckBoxPlayer2OrComputer.Checked = false;
			m_CheckBoxPlayer2OrComputer.Text = "Player 2:";
			m_LabelPlayer1Name.AutoSize = true;
			m_CheckBoxPlayer2OrComputer.Top = m_LabelPlayer1Name.Bottom + 10;
			m_CheckBoxPlayer2OrComputer.Left = m_LabelPlayer1Name.Left;
			this.Controls.Add(m_CheckBoxPlayer2OrComputer);

			m_TextBoxPlayer1Name = new TextBox();
			m_TextBoxPlayer1Name.Text = string.Empty;
			m_TextBoxPlayer1Name.Top = m_LabelPlayer1Name.Top;
			m_TextBoxPlayer1Name.Left = ClientSize.Width - m_TextBoxPlayer1Name.Width - 24;
			this.Controls.Add(m_TextBoxPlayer1Name);

			m_TextBoxPlayer2Name = new TextBox();
			m_TextBoxPlayer2Name.Enabled = false;
			m_TextBoxPlayer2Name.Text = "[Computer]";
			m_TextBoxPlayer2Name.Top = m_CheckBoxPlayer2OrComputer.Top;
			m_TextBoxPlayer2Name.Left = ClientSize.Width - m_TextBoxPlayer2Name.Width - 24;
			this.Controls.Add(m_TextBoxPlayer2Name);

			m_ButtonDone = new Button();
			m_ButtonDone.Left = this.ClientSize.Width - m_ButtonDone.Width - 24;
			m_ButtonDone.Text = "Done";
			m_ButtonDone.Top = m_TextBoxPlayer2Name.Bottom + 10;		
			this.Controls.Add(m_ButtonDone);

			m_ButtonDone.Click += m_ButtonDone_Click;
			m_CheckBoxPlayer2OrComputer.CheckStateChanged += m_CheckBoxPlayer2OrComputer_CheckStateChanged;
		}

		private void m_ButtonDone_Click(object sender, EventArgs e)
		{
			ClosedByButtonDone = true;
			getSizeFromRadioButtons();

			if (!isValidSettings())
			{
				MessageBox.Show("Names are invalid length - up to 10 letters and not empty", "Please try again", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.Close();
		}
		
		private int getSizeFromRadioButtons()
		{
			if (m_RadioButtonSize6.Checked)
			{
				m_BoardSizeSelected = 6;
			}
			else if (m_RadioButtonSize8.Checked)
			{
				m_BoardSizeSelected = 8;
			}
			else if (m_RadioButtonSize10.Checked)
			{
				m_BoardSizeSelected = 10;
			}

			return m_BoardSizeSelected;
		}

		private bool isValidSettings()
		{
			return GameSettingsValidator.ValidateGameSizeSelected(getSizeFromRadioButtons()) &&
				   GameSettingsValidator.ValidatePlayersNames(m_TextBoxPlayer1Name.Text, m_TextBoxPlayer2Name.Text);
		}

		private void m_CheckBoxPlayer2OrComputer_CheckStateChanged(object sender, EventArgs e)
		{
			CheckBox theSender = sender as CheckBox;

			if (theSender.CheckState == CheckState.Checked)
			{
				m_TextBoxPlayer2Name.Enabled = true;
				m_TextBoxPlayer2Name.Text = "";
			}
			else
			{
				m_TextBoxPlayer2Name.Enabled = false;
				m_TextBoxPlayer2Name.Text = "[Computer]";
			}
		}
	}
}
