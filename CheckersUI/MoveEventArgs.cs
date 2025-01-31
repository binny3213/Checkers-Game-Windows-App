using System;

using CheckersGameLogic;

namespace CheckersUI
{
	public class MoveEventArgs : EventArgs
	{
        public Position StartPosition { get; }
        public Position DestinationPosition { get; }

        public MoveEventArgs(Position start, Position destination)
        {
            StartPosition = start;
            DestinationPosition = destination;
        }
    }
}
