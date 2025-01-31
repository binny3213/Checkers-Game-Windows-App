using System.Windows.Forms;
using System.Drawing;

using CheckersGameLogic;

namespace CheckersUI
{
	public class ScoreBoardUI 
    {		
		private Label m_LabelPlayerOneName;
		private Label m_LabelPlayerTwoName;
		private Label m_LabelPlayerOneScore;
		private Label m_LabelPlayerTwoScore;
		private TableLayoutPanel m_TableLayoutScoreBoard;
		private PictureBox m_PictureBoxTurn;		
		private Label m_LabelCurrentTurn;
		private float m_FontSize;
		private readonly Color r_PlayerOneLabelColor = Color.FromArgb(149, 61, 172);
		private readonly Color r_PlayerTwoLabelColor = Color.FromArgb(0, 200, 83);

		private float FontSize
		{
			get
			{
				return m_FontSize;
			}
		}

		public Panel TableLayoutPanel
		{
			get { return m_TableLayoutScoreBoard; }
		}

		public ScoreBoardUI(string i_PlayerOneName, string i_PlayerTwoName, int i_BoardSize)
		{
			setFontSizeBasedOnGameBoardSizeSelected(i_BoardSize);
			initializeComponent();
			updateNameLabels(i_PlayerOneName, i_PlayerTwoName);
			updateScoreBoard();
		}

		private void updateNameLabels(string i_PlayerOneName, string i_PlayerTwoName)
		{
			this.m_LabelPlayerOneName.Text = $"{i_PlayerOneName} :";
			this.m_LabelPlayerTwoName.Text = $"{i_PlayerTwoName} :";
		}
	
		private void initializeComponent()
		{
			this.m_LabelPlayerOneName = new Label();
			this.m_LabelPlayerTwoName = new Label();
			this.m_LabelPlayerOneScore = new Label();
			this.m_LabelPlayerTwoScore = new Label();
			this.m_TableLayoutScoreBoard = new TableLayoutPanel();		
			this.m_PictureBoxTurn = new PictureBox();		
			this.m_LabelCurrentTurn = new Label();
			//this.m_TableLayoutScoreBoard.SuspendLayout();		

			m_TableLayoutScoreBoard.RowCount = 2;
			m_TableLayoutScoreBoard.ColumnCount = 5;
			m_TableLayoutScoreBoard.Dock = DockStyle.Fill;
			m_TableLayoutScoreBoard.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
			m_TableLayoutScoreBoard.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
			m_TableLayoutScoreBoard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
			m_TableLayoutScoreBoard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
			m_TableLayoutScoreBoard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
			m_TableLayoutScoreBoard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
			m_TableLayoutScoreBoard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));

			m_LabelPlayerOneName.Font = new Font("Liberation Mono", FontSize, FontStyle.Bold);
			m_LabelPlayerOneName.AutoSize = true;
			m_LabelPlayerOneName.TextAlign = ContentAlignment.MiddleCenter;
			m_LabelPlayerOneName.Anchor = AnchorStyles.None;
			m_LabelPlayerOneName.ForeColor = r_PlayerOneLabelColor;

			m_LabelPlayerTwoName.Font = new Font("Liberation Mono", FontSize, FontStyle.Bold);
			m_LabelPlayerTwoName.AutoSize = true;
			m_LabelPlayerTwoName.TextAlign = ContentAlignment.MiddleCenter;
			m_LabelPlayerTwoName.Anchor = AnchorStyles.None;
			m_LabelPlayerTwoName.ForeColor = r_PlayerTwoLabelColor;

			m_LabelPlayerOneScore.Font = new Font("Liberation Mono", FontSize, FontStyle.Bold);
			m_LabelPlayerOneScore.AutoSize = true;
			m_LabelPlayerOneScore.TextAlign = ContentAlignment.MiddleCenter;
			m_LabelPlayerOneScore.Anchor = AnchorStyles.None;

			m_LabelPlayerTwoScore.Font = new Font("Liberation Mono", FontSize, FontStyle.Bold);
			m_LabelPlayerTwoScore.AutoSize = true;
			m_LabelPlayerTwoScore.TextAlign = ContentAlignment.MiddleCenter;
			m_LabelPlayerTwoScore.Anchor = AnchorStyles.None;

			m_LabelCurrentTurn.Font = new Font("Liberation Mono", FontSize, FontStyle.Bold);
			m_LabelCurrentTurn.AutoSize = true;
			m_LabelCurrentTurn.TextAlign = ContentAlignment.MiddleCenter;
			m_LabelCurrentTurn.Anchor = AnchorStyles.None;
			m_LabelCurrentTurn.Text = "Turn";
			
			m_PictureBoxTurn.Size = new Size(35, 35);
			m_PictureBoxTurn.SizeMode = PictureBoxSizeMode.CenterImage;
			m_PictureBoxTurn.Anchor = AnchorStyles.None; 
			m_PictureBoxTurn.SizeMode = PictureBoxSizeMode.StretchImage;		
			
			m_TableLayoutScoreBoard.Controls.Add(m_LabelPlayerOneName, 0, 0);
			m_TableLayoutScoreBoard.Controls.Add(m_LabelPlayerOneScore, 1, 0);
			m_TableLayoutScoreBoard.Controls.Add(m_PictureBoxTurn, 2, 1);
			m_TableLayoutScoreBoard.Controls.Add(m_LabelCurrentTurn, 2, 0);			
			m_TableLayoutScoreBoard.Controls.Add(m_LabelPlayerTwoName, 3, 0);
			m_TableLayoutScoreBoard.Controls.Add(m_LabelPlayerTwoScore, 4, 0);
			//this.m_TableLayoutScoreBoard.ResumeLayout(false);
			//this.m_TableLayoutScoreBoard.PerformLayout();			
			//((System.ComponentModel.ISupportInitialize)(this.m_PictureBoxTurn)).EndInit();			
		}

		public void UpdateScoreLabels(int i_PlayerOneScore, int i_PlayerTwoScore)
		{			
			this.m_LabelPlayerOneScore.Text = i_PlayerOneScore.ToString();
			this.m_LabelPlayerTwoScore.Text = i_PlayerTwoScore.ToString();
			updateScoreBoard();
        }

        private void updateScoreBoard()
		{
			m_TableLayoutScoreBoard.Invalidate(); 
		}

		public void UpdateTurnPictureBox(eColor i_PlayerToMoveColor)
		{			
			if (i_PlayerToMoveColor == eColor.White)
			{
				m_PictureBoxTurn.Image = Properties.Resources.regular_purple;
			}
			else
			{
				m_PictureBoxTurn.Image = Properties.Resources.regular_green;
			}			
		}

		private void setFontSizeBasedOnGameBoardSizeSelected(int i_BoardSize)
		{
			m_FontSize = (float)(i_BoardSize * 1.5);
		}
    }
}
