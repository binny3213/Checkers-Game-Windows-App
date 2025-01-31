using CheckersGameLogic;

namespace CheckersUI
{
	public class ApplicationFormManager
	{
		private bool m_SettingsValid = false;
		private FormGameSettings formSettings;
		
		public void FormSettingsCreateAndRun()
		{			
			if (ensureValidSettings())
			{
				Game game = new Game((eBoardSize)formSettings.BoardSizeSelected, formSettings.TextBoxPlayerOneName, formSettings.TextBoxPlayerTwoName, formSettings.GameType);
				GameUI gameUI = new GameUI(game);

				gameUI.ShowDialog();
			}
			else if (formSettings.ClosedByButtonDone)
			{
				FormSettingsCreateAndRun();
			}
		}

		private bool ensureValidSettings()
		{		
			if (!m_SettingsValid)
			{
				formSettings = new FormGameSettings();
				formSettings.ShowDialog();

				if (formSettings.ClosedByButtonDone && 
					GameSettingsValidator.ValidateGameSizeSelected(formSettings.BoardSizeSelected) &&
					GameSettingsValidator.ValidatePlayersNames(formSettings.TextBoxPlayerOneName, formSettings.TextBoxPlayerTwoName))
				{
					m_SettingsValid = true;
				}
			}

			return m_SettingsValid;
		}
	}
}
