namespace DS.Simulation.GameModes
{
	// Mode for testing purpose
	public class FreeGameMode : IGameMode
	{
		public GameModeType GameMode => GameModeType.Free;

		public FreeGameMode(IPlayerController playerController)
		{
			playerController.SpawnPlayer();
		}
	}
}