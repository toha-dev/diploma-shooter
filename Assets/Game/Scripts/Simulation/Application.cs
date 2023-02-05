using DS.Simulation.GameModes;

namespace DS.Simulation
{
	public enum GameMap
	{
		FreeModeMap = 0,
	}

	public static class Application
	{
		public static GameModeType GameMode { get; private set; }
		public static GameMap GameMap { get; private set; }

		public static void SetGameParameters(GameModeType gameMode, GameMap gameMap)
		{
			GameMode = gameMode;
			GameMap = gameMap;
		}
	}
}