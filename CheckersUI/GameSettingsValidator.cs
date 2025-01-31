namespace CheckersUI
{
	public static class GameSettingsValidator
	{
		internal static bool ValidatePlayerNameLength(string i_PlayerName)
		{
			return !string.IsNullOrWhiteSpace(i_PlayerName) && i_PlayerName.Trim().Length <= 10;
		}

		internal static bool ValidatePlayersNames(string i_PlayerOneName, string i_PlayerTwoName)
		{
			bool isValidNames = true;

			isValidNames = isValidNames && ValidatePlayerNameLength(i_PlayerOneName);
			isValidNames = isValidNames && ValidatePlayerNameLength(i_PlayerTwoName);

			return isValidNames;
		}

		internal static bool ValidateGameSizeSelected(int i_BoardSize)
		{
			return (i_BoardSize == 6 || i_BoardSize == 8 || i_BoardSize == 10);
		}
	}
}
