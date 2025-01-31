using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using CheckersGameLogic;

namespace CheckersUI
{
	public class CheckersBoardUI 
	{ 		
		private TableLayoutPanel m_TableLayoutPanelBoard;
		private readonly Color r_ColorFirstClick = Color.FromArgb(0, 200, 20);
		private readonly Color r_ColorBackground = Color.Black;
		private readonly Color r_ColorDefaultCellColor = Color.FromArgb(235, 235, 235);
		private readonly Color r_ColorDisabledCell = Color.FromArgb(50, 50, 50);
		private Color r_PossibleMovesColor = Color.CornflowerBlue;
		private Position m_FirstClickPosition;
		private Position m_SecondClickPosition;
		private bool m_FirstClickOccured;
		private int m_SelectedBoardSize;
		private const int k_SquareSize = 50;
		private readonly Dictionary<eCellState, Image> r_CellMapping = new Dictionary<eCellState, Image>
		{
			{ eCellState.Empty, null },
			{ eCellState.WhiteRegular,  Properties.Resources.regular_purple },
			{ eCellState.WhiteKing, Properties.Resources.king_purple },
			{ eCellState.BlackRegular, Properties.Resources.regular_green },
			{ eCellState.BlackKing, Properties.Resources.king_Green }
		};
		public event Action DestinationPositionSelected;
		public event Action SourcePositionSelected;
		public event Action SamePositionSelected;

		public CheckersBoardUI(int i_BoardSize)
		{
			m_SelectedBoardSize = i_BoardSize;
			initializeGameBoard();
		}

		private void initializeGameBoard()
		{
			PictureBox pictureBoxCell;

			m_TableLayoutPanelBoard = new TableLayoutPanel();
			m_TableLayoutPanelBoard.Width = m_SelectedBoardSize * k_SquareSize;
			m_TableLayoutPanelBoard.Height = m_SelectedBoardSize * k_SquareSize;
			m_TableLayoutPanelBoard.RowCount = m_SelectedBoardSize;
			m_TableLayoutPanelBoard.ColumnCount = m_SelectedBoardSize;
			m_TableLayoutPanelBoard.BackColor = r_ColorBackground;
			m_TableLayoutPanelBoard.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
			m_TableLayoutPanelBoard.Margin = new Padding(1);

			for (int i = 0; i < m_SelectedBoardSize; i++)
			{
				m_TableLayoutPanelBoard.RowStyles.Add(new RowStyle(SizeType.Absolute, k_SquareSize));
				m_TableLayoutPanelBoard.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, k_SquareSize));
			}

			for (int row = 0; row < m_SelectedBoardSize; row++)
			{
				for (int col = 0; col < m_SelectedBoardSize; col++)
				{
					pictureBoxCell = new PictureBox
					{
						Size = new Size(k_SquareSize, k_SquareSize),
						Dock = DockStyle.None,
						BackColor = (row + col) % 2 == 0 ? r_ColorDisabledCell : r_ColorDefaultCellColor,
						Margin = new Padding(1)
					};

					if (pictureBoxCell.BackColor == r_ColorDisabledCell)
					{
						pictureBoxCell.Enabled = false;
					}

					pictureBoxCell.Click += OnPictureBoxCell_Click;
					m_TableLayoutPanelBoard.Controls.Add(pictureBoxCell, col, row);
				}
			}
		}

		internal Position FirstClickedPosition
		{
			get
			{
				return m_FirstClickPosition;
			}
		}

		internal Position SecondClickedPosition
		{
			get
			{
				return m_SecondClickPosition;
			}
		}

		internal bool FirstClickOccured
		{
			get
			{
				return m_FirstClickOccured;
			}
			set
			{
				m_FirstClickOccured = value;
			}
		}

		internal int SquareSize
		{
			get
			{
				return k_SquareSize;
			}
		}

		internal TableLayoutPanel TableLayoutPanelBoard
		{
			get
			{
				return m_TableLayoutPanelBoard;
			}
		}

		internal void ResetHighligtedSquares(List<Position> i_PositionsTorResetColor)
		{
			PictureBox boardSquare;

			foreach (Position pos in i_PositionsTorResetColor)
			{
				boardSquare = m_TableLayoutPanelBoard.GetControlFromPosition(pos.Col, pos.Row) as PictureBox;
				boardSquare.BackColor = r_ColorDefaultCellColor;
			}
		}

		internal void RenderCurrentBoardState(eCellState[,] i_BoardState)
		{
			for (int row = 0; row < m_SelectedBoardSize; row++)
			{
				for (int col = 0; col < m_SelectedBoardSize; col++)
				{
					setCellStateUI(col, row, i_BoardState[row, col]);
				}
			}

			m_TableLayoutPanelBoard.Invalidate();
			m_TableLayoutPanelBoard.Update();
		}

		internal void PaintPossibleDestinations(List<Position> i_ValidDestinationPositions)
		{
			PictureBox boardSquare;

			foreach (Position pos in i_ValidDestinationPositions)
			{
				boardSquare = m_TableLayoutPanelBoard.GetControlFromPosition(pos.Col, pos.Row) as PictureBox;
				boardSquare.BackColor = r_PossibleMovesColor;
			}
		}

		private void OnPictureBoxCell_Click(object sender, EventArgs e)
		{
			PictureBox theSender = sender as PictureBox;

			if (!FirstClickOccured)
			{		
				m_FirstClickPosition.Row = getCellPositionRow(theSender);
				m_FirstClickPosition.Col = getCellPositionColumn(theSender);				
				theSender.BackColor = r_ColorFirstClick;
				FirstClickOccured = true;
				OnSourcePositionSelected();		
			}
			else
			{
				m_SecondClickPosition.Row = getCellPositionRow(theSender);
				m_SecondClickPosition.Col = getCellPositionColumn(theSender);

				if (userClickedSamePositionTwice())
				{
					theSender.BackColor = r_ColorDefaultCellColor;
					OnSamePositionSelected();
					
				}
				else
				{
					OnDestinationPositionSelected();
				}
				
				FirstClickOccured = false;
			}			
		}

		private bool userClickedSamePositionTwice()
		{
			bool isSameLocation; 

			isSameLocation = SecondClickedPosition.Row == FirstClickedPosition.Row && SecondClickedPosition.Col == FirstClickedPosition.Col;

			return isSameLocation;
		}

		protected virtual void OnSourcePositionSelected()
		{
			SourcePositionSelected?.Invoke();
		}

		protected virtual void OnDestinationPositionSelected()
		{
			DestinationPositionSelected?.Invoke();
		}

		protected virtual void OnSamePositionSelected()
		{
			SamePositionSelected?.Invoke();
		}

		private void setCellStateUI(int i_Col, int i_Row, eCellState i_CellState)
		{
			PictureBox buttonCell;
			
			buttonCell = TableLayoutPanelBoard.GetControlFromPosition(i_Col, i_Row) as PictureBox;
			buttonCell.Image = r_CellMapping[i_CellState];
			buttonCell.SizeMode = PictureBoxSizeMode.StretchImage;
			buttonCell.BackColor = (i_Col + i_Row) % 2 == 0 ? r_ColorDisabledCell : r_ColorDefaultCellColor;
		}

		private int getCellPositionRow(PictureBox i_TheSender)
		{			
			return TableLayoutPanelBoard.GetRow(i_TheSender);
		}

		private int getCellPositionColumn(PictureBox i_TheSender)
		{
			return TableLayoutPanelBoard.GetColumn(i_TheSender);
		}
	}
}

