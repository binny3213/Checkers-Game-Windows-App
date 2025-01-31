
namespace CheckersGameLogic
{
	public struct Position
	{
		public int Row { get; set; }
		public int Col { get; set; }
		public Position(int i_Row, int i_Col)
		{
			Row = i_Row;
			Col = i_Col;
		}

		public static bool operator ==(Position i_Pos1, Position i_Pos2)
		{
			return i_Pos1.Row == i_Pos2.Row && i_Pos1.Col == i_Pos2.Col;
		}

		// Even though they're not used, according to what we learned if we override == we should/must override != and Equals.
		public static bool operator != (Position i_Pos1, Position i_Pos2) 
		{
			return !(i_Pos1 == i_Pos2);
		}

		public override bool Equals(object i_Obj)
		{
			bool isEquals = false;

			if (i_Obj == null)
			{
				isEquals = false;
			}

			if (i_Obj is Position otherPosition)
			{
				isEquals = (this == otherPosition);
			}

			return isEquals;
		}
	}
}

