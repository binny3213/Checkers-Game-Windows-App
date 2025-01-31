
namespace CheckersGameLogic
{
	public struct Move
	{
		public Position Start { get; }
		public Position End { get; }
		public bool IsCapture { get; }

		public Move(Position i_StartPos, Position i_EndPos, bool i_IsCapture)
		{
			Start = i_StartPos;
			End = i_EndPos;
			IsCapture = i_IsCapture;
		}

		public static bool operator ==(Move i_Param1, Move i_Param2)
		{
			return i_Param1.Equals(i_Param2); 
		}

		public static bool operator !=(Move i_Param1, Move i_Param2)
		{
			return !(i_Param1 == i_Param2);
		}
	}
}
