using Godot;
using System.Collections.Generic;

public partial class ScriptManager : Node
{
	[Signal]
	public delegate void ScriptReloadedEventHandler(string scriptName, bool success, string message);

	private static readonly string[] BannedTokens =
	{
		"OS", "FileAccess", "DirAccess", "ProjectSettings",
		"Engine", "ClassDB", "load(", "preload(", "while "
	};

	private string _scriptsDir;
	private readonly Dictionary<string, GodotObject> _instances = new();
	private readonly Dictionary<string, ulong> _mtimes = new();
	private double _pollTimer;

	public override void _Ready()
	{
		string baseDir = OS.HasFeature("editor")
			? ProjectSettings.GlobalizePath("res://")
			: OS.GetExecutablePath().GetBaseDir();

		_scriptsDir = baseDir.PathJoin("scripts");
		DirAccess.MakeDirRecursiveAbsolute(_scriptsDir);
	}

	public void RegisterRule(string scriptName)
	{
		if (!_instances.ContainsKey(scriptName))
			_instances[scriptName] = null;

		string target = _scriptsDir.PathJoin($"{scriptName}.gd");
		if (!FileAccess.FileExists(target))
			ResetRule(scriptName);
		else
			ReloadFromDisk(scriptName, target);
	}
	public void ResetRule(string scriptName)
	{
		string template = $"res://default_scripts/{scriptName}.gd.txt";
		string target = _scriptsDir.PathJoin($"{scriptName}.gd");

		if (!FileAccess.FileExists(template))
		{
			GD.PushError($"Template not found: {template}");
			return;
		}

		string defaultCode = FileAccess.GetFileAsString(template);

		using (var file = FileAccess.Open(target, FileAccess.ModeFlags.Write))
		{
			file.StoreString(defaultCode);
		} // Flushes and closes the file on disk here

		ReloadFromDisk(scriptName, target);
	}

	// old from ai
	// public void ResetRule(string scriptName)
	// {
	// 	GD.Print("resetting");
	// 	string template = $"res://default_scripts/{scriptName}.gd.txt";
	// 	string target = _scriptsDir.PathJoin($"{scriptName}.gd");
	//
	// 	using var file = FileAccess.Open(target, FileAccess.ModeFlags.Write);
	// 	file.StoreString(FileAccess.GetFileAsString(template));
	// 	ReloadFromDisk(scriptName, target);
	// }

	public Variant CallRule(string scriptName, string method, Godot.Collections.Array args = null, Variant fallback = default)
	{
		if (_instances.TryGetValue(scriptName, out GodotObject inst) && inst != null && inst.HasMethod(method))
		{
			return inst.Callv(method, args ?? new Godot.Collections.Array());
		}

		GD.Print("fallback");
		return fallback;
	}

	public override void _Process(double delta)
	{
		_pollTimer += delta;
		if (_pollTimer < 0.5)
			return;
		_pollTimer = 0.0;

		foreach (string scriptName in _instances.Keys)
		{
			string path = _scriptsDir.PathJoin($"{scriptName}.gd");
			ulong mtime = FileAccess.GetModifiedTime(path);
			if (!_mtimes.TryGetValue(scriptName, out ulong lastMtime) || mtime != lastMtime)
			{
				ReloadFromDisk(scriptName, path);
			}
		}
	}

	private void ReloadFromDisk(string scriptName, string path)
	{
		_mtimes[scriptName] = FileAccess.GetModifiedTime(path);
		string code = FileAccess.GetFileAsString(path);

		foreach (string token in BannedTokens)
		{
			if (code.Contains(token))
			{
				EmitSignal(SignalName.ScriptReloaded, scriptName, false, $"Blocked keyword: {token}");
				return;
			}
		}

		var dyn = new GDScript();
		dyn.SourceCode = code;

		if (dyn.Reload() != Error.Ok)
		{
			EmitSignal(SignalName.ScriptReloaded, scriptName, false, $"Syntax error in {scriptName}.gd");
			return;
		}

		_instances[scriptName] = dyn.New().AsGodotObject();
		EmitSignal(SignalName.ScriptReloaded, scriptName, true, $"Reloaded {scriptName}.gd");
	}
}
