using Godot;

namespace How2Hack.Scripts;

public partial class TitleScreen : Control
{
	private Button startGameButton;
	private Button quitGameButon;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		startGameButton = GetNode<Button>("StartGameButton");
		quitGameButon = GetNode<Button>("StartGameButton");
	}

	public void OnStartGame()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Disclaimer.tscn");
	}

	public void OnQuitGame()
	{
		GetTree().Quit();
	}
}
