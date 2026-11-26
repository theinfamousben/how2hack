using System.Collections.Generic;
using Godot;

namespace How2Hack.Scripts;

public class ErrorHelper
{
    public readonly List<Error> errors = 
    [
        new("Camera Clamps Not Set", Errors.CameraClampsNotSet),
        new("Outline Material Not Set", Errors.OutlineMaterialNotSet)
    ];

    public void ThrowOnce(Error e, string caller = "[not set]", bool display = false)
    {
        if (e.thrown) return;
        Throw(e, caller);
        GetError(e.errorEnum).thrown = true;
    }

    public void ThrowOnce(Errors errorEnum, string caller = "[not set]", bool display = false) => ThrowOnce(GetError(errorEnum), caller, display);

    public void Throw(Error e, string caller = "[not set]", bool display = false)
    {
        GD.PrintErr($"{caller}: {e.msg}");
    }

    public void Throw(Errors errorEnum, string caller = "[not set]", bool display = false) => Throw(GetError(errorEnum), caller, display);

    public Error GetError(Errors e) => errors.Find(error => error.errorEnum == e);
}

public class Error
{
    public Error(string msg, Errors errorEnum, bool thrown = false)
    {
        this.msg = msg;
        this.errorEnum = errorEnum;
        this.thrown = thrown;
    }

    public readonly Errors errorEnum;
    public readonly string msg;
    public bool thrown = false;
}
public enum Errors
{
    CameraClampsNotSet,
    OutlineMaterialNotSet
}