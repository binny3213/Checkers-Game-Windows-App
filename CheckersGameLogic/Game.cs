using System;
using System.Collections.Generic;

namespace CheckersGameLogic
{
	public class Game
	{
		private CheckersBoard m_Board;
		private Player m_Player1;
		private Player m_Player2;
		private readonly Random r_RandomInstanceForMoveGeneration = new Random(); 
		
		public event Action GameEnded;
		public event Action ComputerMoveExecuted;

		public Game(eBoardSize i_BoardSize, string i_PlayerOneName, string i_PlayerTwoName, ePlayerType i_PlayerType)
		{
			m_Board = new CheckersBoard(i_BoardSize, this);
			m_Player1 = new Player(i_PlayerOneName, ePlayerType.Human, eColor.White);
			m_Player2 = new Player(i_PlayerTwoName, i_PlayerType, eColor.Black);
			PlayerOneTotalScore = 0;
			PlayerTwoTotalScore = 0;
			StartNewGame();
		}

		public Player PlayerToMove { get; private set; }

		public Player PlayerWhoPlayedLast { get; private set; }

		public Player GameWinner { get; private set; }
	
		public bool IsGameRunning { get; private set; }
		
		internal bool IsInConsecutiveCapturesMode { get; private set; }

		public int BoardSizeInt
		{
			get
			{
				return m_Board.BoardSizeInt;
			}
		}

		public string PlayerOneName
		{
			get
			{
				return m_Player1.PlayerName;
			}
		}

		public string PlayerTwoName
		{
			get
			{
				return m_Player2.PlayerName;
			}
		}

		public int PlayerOneTotalScore
		{
			get
			{
				return m_Player1.PlayerTotalScore;
			}
			set
			{
				m_Player1.PlayerTotalScore = value;
			}
		}

		public int PlayerTwoTotalScore
		{
			get
			{
				return m_Player2.PlayerTotalScore;
			}
			set
			{
				m_Player2.PlayerTotalScore = value;
			}
		}

		public List<Position> GetPossibleDestinationCells(Position i_Position)
		{
			Position startPos;
			Position endPos;
			List<Position> validDestinationCells = new List<Position>();

			foreach (Move mv in PlayerToMove.m_ListPlayerRegularMoves)
			{
				startPos = mv.Start;
				endPos = mv.End;

				if (startPos == i_Position)
				{
					validDestinationCells.Add(endPos);
				}
			}

			foreach (Move mv in PlayerToMove.m_ListPlayerCaptureMoves)
			{
				startPos = mv.Start;
				endPos = mv.End;

				if (startPos == i_Position)
				{
					validDestinationCells.Add(endPos);
				}
			}

			return validDestinationCells;
		}

		public void StartNewGame()
		{			
			IsGameRunning = true;
			PlayerToMove = m_Player1; 
			PlayerWhoPlayedLast = null;
			IsInConsecutiveCapturesMode = false;
			GameWinner = null;

			m_Board.SetToBoardStartingPosition();
			clearMovesListsForBothPlayers(); //relevant for repeated games.
			generateBothListsOfMovesForPlayer(PlayerToMove);
		}

		public bool ValidateMove(Move i_Move)
		{
			bool isValidMove;

			bool isInCaptureMoves = isMoveInListCaptureMoves(PlayerToMove, i_Move);
			bool isInRegularMoves = isMoveInListRegularMoves(PlayerToMove, i_Move);

			if (PlayerToMove.m_ListPlayerCaptureMoves.Count > 0)
			{
				isValidMove = isInCaptureMoves;
			}
			else
			{
				isValidMove = isInRegularMoves;
			}

			return isValidMove;
		}

		public void ExecuteHumanPlayerMove(Move i_Move)
		{					
			ExecuteMoveAndCheckConsecutiveCaptures(i_Move);
			if ( IsGameRunning && PlayerToMove.PlayerType == ePlayerType.Cpu )
			{
				executeComputerMove();
				OnComputerMoveExecuted();
			}
		}

		public eCellState[,] GetBoardStateSnapshot() 
		{
			int size = m_Board.BoardSizeInt;
			eCellState[,] boardStateSnapshot = new eCellState[size, size];

			for (int row = 0; row < size; row++)
			{
				for (int col = 0; col < size; col++)
				{
					boardStateSnapshot[row, col] = m_Board.GetCellStateFromBoard(new Position(row, col));
				}
			}

			return boardStateSnapshot;
		}

		private void executeComputerMove()
		{
			bool isHasCapture;
			
			generateBothListsOfMovesForPlayer(PlayerToMove);
			isHasCapture = checkIfHasCapture(PlayerToMove);

			if (isHasCapture)
			{
				executeSeriesOfComputerMovesCaptures();
			}
			else
			{
				executeOneNonCaptureMoveForComputer();
			}

			switchTurn();
			generateBothListsOfMovesForPlayer(PlayerToMove);			
			checkIfGameOverAndDeclareWinnerOrTie();			
		}

		private void executeSeriesOfComputerMovesCaptures()
		{
			int validMovesListCount;
			int randomIndex;
			bool isExecutingMoves = true;
			bool isKingBeforeMoving;
			Move move;
			
			validMovesListCount = PlayerToMove.m_ListPlayerCaptureMoves.Count;
			randomIndex = r_RandomInstanceForMoveGeneration.Next(validMovesListCount);
			move = PlayerToMove.m_ListPlayerCaptureMoves[randomIndex];
			isKingBeforeMoving = m_Board.GetDiscFromPositionOrNullIfEmpty(move.Start).DiscType == eDiscType.King; 
			
			while (isExecutingMoves)
			{
				m_Board.ExecuteCapture(move);
				
				if (m_Board.HasReachedEndRow(move.End) && isKingBeforeMoving == false)
				{
					m_Board.PromoteToKing(move.End);
					isExecutingMoves = false; //In the very move where he became king, a checkerPiece should not continue in capturing series.. (according to english checkers rules).
				}

				GenerateNextConsecutiveCaptureMoves(move);
				validMovesListCount = PlayerToMove.m_ListPlayerCaptureMoves.Count;

				if (validMovesListCount == 0)
				{
					isExecutingMoves = false;
				}
				else
				{
					randomIndex = r_RandomInstanceForMoveGeneration.Next(validMovesListCount);
					move = PlayerToMove.m_ListPlayerCaptureMoves[randomIndex];
				}
			}
		}

		private void executeOneNonCaptureMoveForComputer()
		{
			Move move;
			int validMovesListCount;
			int randomIndex;

			if (IsGameRunning)
			{
				validMovesListCount = PlayerToMove.m_ListPlayerRegularMoves.Count;
				randomIndex = r_RandomInstanceForMoveGeneration.Next(validMovesListCount);
				move = PlayerToMove.m_ListPlayerRegularMoves[randomIndex];
				m_Board.ExecuteRegularMove(move);
			}			
		}

		protected virtual void OnGameEnded()
		{
			GameEnded?.Invoke();
		}

		protected virtual void OnComputerMoveExecuted()
		{
			ComputerMoveExecuted?.Invoke();
		}

		private void clearMovesListsForBothPlayers()
		{
			m_Player1.m_ListPlayerCaptureMoves.Clear();
			m_Player1.m_ListPlayerRegularMoves.Clear();
			m_Player2.m_ListPlayerCaptureMoves.Clear();
			m_Player2.m_ListPlayerRegularMoves.Clear();
		}

		private bool isMoveInListCaptureMoves(Player i_Player, Move i_Move)
		{
			bool isInList = false;

			foreach (Move move in PlayerToMove.m_ListPlayerCaptureMoves)
			{
				if (move == i_Move)
				{
					isInList = true;
					break;
				}
			}

			return isInList;
		}

		private bool isMoveInListRegularMoves(Player i_Player, Move i_Move)
		{
			bool isInList = false;

			foreach (Move move in i_Player.m_ListPlayerRegularMoves)
			{
				if (move == i_Move)
				{
					isInList = true;
					break;
				}
			}

			return isInList;
		}

		private void GenerateNextConsecutiveCaptureMoves(Move i_Move)
		{
			Disc disc = m_Board.GetDiscFromPositionOrNullIfEmpty(i_Move.End);
			bool isKing = disc.DiscType == eDiscType.King;

			PlayerToMove.m_ListPlayerCaptureMoves.Clear();
			m_Board.GenerateMovesFromPosition(i_Move.End, PlayerToMove, isKing);
		}

		private void generateBothListsOfMovesForPlayer(Player i_Player) 
		{
			eColor discColor;
			bool isKing = false;
			Disc disc = null;
			Player discOwner = null;
			eColor playerColor = i_Player.PlayerColor;

			i_Player.m_ListPlayerRegularMoves.Clear();
			i_Player.m_ListPlayerCaptureMoves.Clear();

			for (int row = 0; row < BoardSizeInt; row++)
			{
				for (int col = 0; col < BoardSizeInt; col++)
				{
					Position startPos = new Position(row, col);
					disc = m_Board.GetDiscFromPositionOrNullIfEmpty(startPos);

					if (disc != null && disc.DiscColor == playerColor)
					{
						isKing = disc.DiscType == eDiscType.King;
						discColor = disc.DiscColor;
						discOwner = (discColor == eColor.White) ? m_Player1 : m_Player2;
						m_Board.GenerateMovesFromPosition(startPos, discOwner, isKing);
					}
				}
			}
		}

		private void switchTurn()
		{
			PlayerToMove = (PlayerToMove == m_Player1) ? m_Player2 : m_Player1;
			PlayerWhoPlayedLast = (PlayerToMove == m_Player1) ? m_Player2 : m_Player1;
		}

		private void ExecuteMoveAndCheckConsecutiveCaptures(Move i_Move)
		{
			if (i_Move.IsCapture)
			{
				m_Board.ExecuteCapture(i_Move);
				GenerateNextConsecutiveCaptureMoves(i_Move);
				IsInConsecutiveCapturesMode = PlayerToMove.m_ListPlayerCaptureMoves.Count > 0 ? true : false;
			}
			else
			{
				m_Board.ExecuteRegularMove(i_Move);
			}

			if (m_Board.HasReachedEndRow(i_Move.End))
			{
				m_Board.GetDiscFromPositionOrNullIfEmpty(i_Move.End).PromoteToKing();
			}

			if (IsInConsecutiveCapturesMode)
			{
				PlayerWhoPlayedLast = PlayerToMove;
			}
			else
			{
				switchTurn();
				generateBothListsOfMovesForPlayer(PlayerToMove);
			}

			checkIfGameOverAndDeclareWinnerOrTie();
		}

		private bool checkIfHasCapture(Player i_Player)
		{
			return i_Player.m_ListPlayerCaptureMoves.Count > 0;
		}

		private void checkIfGameOverAndDeclareWinnerOrTie()
		{
			Player otherPlayer = null;
			Player loser = null;
			int playerToMoveValidMovesCount;
			int otherPlayerValidMovesCount;

			otherPlayer = (PlayerToMove == m_Player1) ? m_Player2 : m_Player1;
			generateBothListsOfMovesForPlayer(otherPlayer);
			playerToMoveValidMovesCount = PlayerToMove.m_ListPlayerRegularMoves.Count + PlayerToMove.m_ListPlayerCaptureMoves.Count;
			otherPlayerValidMovesCount = otherPlayer.m_ListPlayerRegularMoves.Count + otherPlayer.m_ListPlayerCaptureMoves.Count;
			IsGameRunning = false;

			if (playerToMoveValidMovesCount == 0 && otherPlayerValidMovesCount == 0)
			{
				GameWinner = null;
			}
			else if (playerToMoveValidMovesCount > 0 && otherPlayerValidMovesCount == 0)
			{
				GameWinner = PlayerToMove;
				loser = otherPlayer;
			}
			else if (playerToMoveValidMovesCount == 0 && otherPlayerValidMovesCount > 0)
			{
				GameWinner = otherPlayer;
				loser = PlayerToMove;
			}
			else
			{
				IsGameRunning = true;
			}

			if (!IsGameRunning)
			{
				if (GameWinner != null)
				{
					calculatePoints(loser);
				}

				OnGameEnded();
			}
		}

		private void calculatePoints(Player i_Loser)
		{
			eCellState[,] cellStatesSnapshot = GetBoardStateSnapshot();
			int playerOneRoundTotal = 0;
			int playerTwoRoundTotal = 0;

			if (GameWinner != null)
			{
				GameWinner = (i_Loser == m_Player1) ? m_Player2 : m_Player1;

				for (int row = 0; row < BoardSizeInt; row++)
				{
					for (int col = 0; col < BoardSizeInt; col++)
					{
						switch (cellStatesSnapshot[row, col])
						{
							case eCellState.Empty:
								break;
							case eCellState.WhiteRegular:
								playerOneRoundTotal += 1;
								break;
							case eCellState.BlackRegular:
								playerTwoRoundTotal += 1;
								break;
							case eCellState.WhiteKing:
								playerOneRoundTotal += 4;
								break;
							case eCellState.BlackKing:
								playerTwoRoundTotal += 4;
								break;
							default:
								break;
						}
					}
				}
			}

			GameWinner.PlayerTotalScore += Math.Abs(playerOneRoundTotal - playerTwoRoundTotal);
		}
	}
}
