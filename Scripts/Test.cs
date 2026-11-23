using Godot;

namespace How2Hack.Scripts;

public partial class Test : Node2D
{
	private ScriptManager scriptManager;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scriptManager = GetNode<ScriptManager>("/root/ScriptManager");
		scriptManager.RegisterRule("test_file");
		GD.Print(CheckTarget(56));
		GD.Print(CheckTarget(67));
	}

	bool CheckTarget(int i)
	{
		return (bool)scriptManager.CallRule("test_file", "return_true", [i], false);
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame. 
	public override void _Process(double delta)
	{
	}
}
