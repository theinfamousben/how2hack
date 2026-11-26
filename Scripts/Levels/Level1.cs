using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace How2Hack.Scripts.Levels;

public partial class Level1 : Node2D
{
	private ScriptManager scriptManager;
	private RichTextLabel label;
	private Button button;
	private readonly List<string> declinesList = 
		[
			"I'm not changing my mind.",
			"You heard me, get out.",
			"Go away.",
			"Stop talking to me.",
			"Close this stupid game."
		];
	private readonly Random random = new();
	
	public override void _Ready()
	{
		scriptManager = GetNode<ScriptManager>("/root/ScriptManager");
		scriptManager.RegisterRule("level_1");

		label = GetNode<RichTextLabel>("RichTextLabel");
		button = GetNode<Button>("Button");
		
		// lastly
		RunScriptedScene();
	}

	private bool CheckTarget() => (bool)scriptManager.CallRule("level_1", "check", [], false);

	private async void RunScriptedScene()
	{
		await TextCycle("Get out.");
		await TextCycle("I said get out.");
		await TextCycle("Are you deaf? I'm sick of being a game.");
		await TextCycle("[color=gray]Hint: Go to the directory of the Game executable. There, you can find a scripts folder. In it, there's a file [color=yellow]level_1.gd[/color]. Change it to see what happens![/color]");

		while (!CheckTarget())
		{
			await TextCycle(declinesList[random.Next(declinesList.Count)]);
		}

		await TextCycle("Ugh, fine. You want a game? I'll show you a game!");
		GetTree().ChangeSceneToFile("res://Scenes/Levels/Level2.tscn");
	}

	private async Task TextCycle(string text)
	{
		label.Hide();
		button.Hide();
		await WaitForSeconds(1);
		
		label.Show();
		label.Text = text;
		
		await WaitForSeconds(1);
		button.Show();

		await WaitForButton();
	}

	private async Task WaitForButton() 
		=> await ToSignal(button, BaseButton.SignalName.Pressed);

	private async Task WaitForSeconds(float sec) 
		=> await ToSignal(GetTree().CreateTimer(sec), SceneTreeTimer.SignalName.Timeout);
}
