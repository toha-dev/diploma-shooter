using DS.Simulation.GameModes;
using UniRx;

namespace DS.Simulation
{
	public enum GameMap
	{
		FreeModeMap = 0,
	}

	public enum ApplicationState
	{
		Launching = 0,
		MainMenu = 1,
		Loading = 2,
		Loaded = 3,
		Running = 4,
	}

	public static class Application
	{
		public static IReadOnlyReactiveProperty<ApplicationState> State => InnerState;
		public static GameModeType GameMode { get; private set; }
		public static GameMap GameMap { get; private set; }

		private static readonly ReactiveProperty<ApplicationState> InnerState = new(ApplicationState.Launching);

		public static void RunWithParameters(GameModeType gameMode, GameMap gameMap)
		{
			GameMode = gameMode;
			GameMap = gameMap;
		}

		public static void UpdateState(ApplicationState state)
		{
			InnerState.Value = state;
		}
	}
}