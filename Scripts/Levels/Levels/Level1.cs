using Godot;

namespace How2Hack.Scripts.Levels.Levels;

public partial class Level1 : Node2D
{
	private ScriptManager scriptManager;
	
	public override void _Ready()
	{
		scriptManager = GetNode<ScriptManager>("/root/ScriptManager");
		scriptManager.RegisterRule("level_1");
	}

	private bool CheckTarget() => (bool)scriptManager.CallRule("level_1", "check", [], false);

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}