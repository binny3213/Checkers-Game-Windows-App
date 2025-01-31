using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using CheckersGameLogic;

namespace CheckersUI
{
	public class GameUI : Form 
	{
		private CheckersBoardUI m_BoardUI;
		private ScoreBoardUI m_ScoreBoardUI;
		private List<Position> m_OptionalDestinations;
		private Game m_Game;

		public GameUI(Game i_Game)
		{
			m_Game = i_Game;
			initializeGameUI();			
		}
		
		private void initializeGameUI()
		{
			this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			this.Text = "Damka";
			m_BoardUI = new CheckersBoardUI(m_Game.BoardSizeInt);				
			m_ScoreBoardUI = new ScoreBoardUI(m_Game.PlayerOneName, m_Game.PlayerTwoName, m_Game.BoardSizeInt);	
			
			m_ScoreBoardUI.TableLayoutPanel.Dock = DockStyle.Top;
			m_ScoreBoardUI.UpdateTurnPictureBox(m_Game.PlayerToMove.PlayerColor);

			setFormDimensionsBasedOnBoardSize(m_Game.BoardSizeInt);
			
			this.Controls.Add(m_ScoreBoardUI.TableLayoutPanel);
			this.Controls.Add(m_BoardUI.TableLayoutPanelBoard);

			m_BoardUI.TableLayoutPanelBoard.Anchor = AnchorStyles.None;
			m_BoardUI.TableLayoutPanelBoard.Left = (this.ClientSize.Width - m_BoardUI.TableLayoutPanelBoard.Width) / 2;
			m_BoardUI.TableLayoutPanelBoard.Top = ClientSize.Height - m_BoardUI.TableLayoutPanelBoard.Height - 20;
			m_BoardUI.RenderCurrentBoardState(m_Game.GetBoardStateSnapshot());

			m_BoardUI.DestinationPositionSelected += m_Board_DestinationPositionSelected;
			m_BoardUI.SourcePositionSelected += m_BoardUI_SourcePositionSelected;
			m_BoardUI.SamePositionSelected += m_BoardUI_SamePositionSelected;
			m_Game.GameEnded += m_Game_GameEnded;
			m_Game.ComputerMoveExecuted += m_Game_ComputerMoveExecuted;
		}

		private void m_BoardUI_SamePositionSelected()
		{
			if (m_OptionalDestinations != null && m_OptionalDestinations.Count > 0)
			{
				m_BoardUI.ResetHighligtedSquares(m_OptionalDestinations);
			}			
		}

		private void m_Game_ComputerMoveExecuted()
		{
			m_BoardUI.RenderCurrentBoardState(m_Game.GetBoardStateSnapshot());
		}

		private void m_BoardUI_SourcePositionSelected()
		{
			int row = m_BoardUI.FirstClickedPosition.Row;
			int col = m_BoardUI.FirstClickedPosition.Col;
					
			m_OptionalDestinations = m_Game.GetPossibleDestinationCells(new Position(row,col));
			m_BoardUI.PaintPossibleDestinations(m_OptionalDestinations);
		}

		private void m_Game_GameEnded()
		{
			string gameResultString = string.Empty;
			DialogResult userWantsAnotherRound;

			m_BoardUI.RenderCurrentBoardState(m_Game.GetBoardStateSnapshot());			
			updateScoreBoard();		
			gameResultString = (m_Game.GameWinner == null) ? "Tie!" : $"{m_Game.GameWinner.PlayerName} Won!";
			userWantsAnotherRound =  MessageBox.Show(
				$"{gameResultString}{Environment.NewLine}AnotherRound?"
				,"Damka", 
				MessageBoxButtons.YesNo, 
				MessageBoxIcon.Question, 
				MessageBoxDefaultButton.Button1);

			if (userWantsAnotherRound == DialogResult.Yes)
			{
				m_Game.StartNewGame();
				m_BoardUI.FirstClickOccured = false; 
			}
			else
			{
				this.Close();
			}			
		}

		private void updateScoreBoard()
		{
			m_ScoreBoardUI.UpdateScoreLabels(m_Game.PlayerOneTotalScore, m_Game.PlayerTwoTotalScore);
		}

		private void m_Board_DestinationPositionSelected()
		{
			int startRow;
			int startCol;
			int destRow;
			int destCol;
			PictureBox start;
			PictureBox end;
			Move move;

			startRow = m_BoardUI.FirstClickedPosition.Row;
			startCol = m_BoardUI.FirstClickedPosition.Col;
			destRow = m_BoardUI.SecondClickedPosition.Row;
			destCol = m_BoardUI.SecondClickedPosition.Col;
			start = m_BoardUI.TableLayoutPanelBoard.GetControlFromPosition(startCol, startRow) as PictureBox;
			end = m_BoardUI.TableLayoutPanelBoard.GetControlFromPosition(destCol, destRow) as PictureBox;
			move = getMoveFromUserClicks(startRow, startCol, destRow, destCol);
			
			if (m_Game.ValidateMove(move))
			{
				m_Game.ExecuteHumanPlayerMove(move);
				m_ScoreBoardUI.UpdateTurnPictureBox(m_Game.PlayerToMove.PlayerColor);
			}
			else 
			{
				start.BackColor = Color.Red;
				end.BackColor = Color.Red;
				MessageBox.Show("Invalid move", "Invalid Move Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
						
			m_BoardUI.RenderCurrentBoardState(m_Game.GetBoardStateSnapshot());			
		}

		private void setFormDimensionsBasedOnBoardSize(int i_BoardSizeInt)
		{
			int width;
			int height;
			
			height = m_BoardUI.SquareSize * i_BoardSizeInt + m_ScoreBoardUI.TableLayoutPanel.Height + 50;
			width = m_BoardUI.SquareSize * i_BoardSizeInt + 50;
			this.ClientSize = new Size(width,height);
		}

		private Move getMoveFromUserClicks(int i_StartRow, int i_StartCol, int i_DestRow, int i_DestCol)
		{
			bool isPotentialCapture;
			Move move;

			isPotentialCapture = (Math.Abs(i_StartRow - i_DestRow) == 2 && (Math.Abs(i_StartCol - i_DestCol) == 2));
			move = new Move(m_BoardUI.FirstClickedPosition, m_BoardUI.SecondClickedPosition, isPotentialCapture);
			
			return move;
		}
	}
}

