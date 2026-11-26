using Godot;

namespace How2Hack.Scripts;

public partial class Disclaimer : Node2D
{
	private void OnContinue() => GetTree().ChangeSceneToFile("res://Scenes/Levels/Level1.tscn");
}
