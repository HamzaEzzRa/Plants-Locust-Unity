using System;

public class DebugCommandBase
{
    private string commandId, commandDesc, commandFormat;

    public string CommandId => commandId;
    public string CommandDesc => commandDesc;
    public string CommandFormat => commandFormat;

    public DebugCommandBase(string id, string desc, string format)
    {
        commandId = id;
        commandDesc = desc;
        commandFormat = format;
    }
}

public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(string id, string desc, string format, Action command) : base (id, desc, format)
    {
        this.command = command;
    }

    public void Invoke()
    {
        command.Invoke();
    }
}

public class DebugCommand<T> : DebugCommandBase
{
    private Action<T> command;

    public DebugCommand(string id, string desc, string format, Action<T> command) : base (id, desc, format)
    {
        this.command = command;
    }

    public void Invoke(T value)
    {
        command.Invoke(value);
    }
}

public class DebugCommand<T1, T2> : DebugCommandBase
{
    private Action<T1, T2> command;

    public DebugCommand(string id, string desc, string format, Action<T1, T2> command) : base (id, desc, format)
    {
        this.command = command;
    }

    public void Invoke(T1 val1, T2 val2)
    {
        command.Invoke(val1, val2);
    }
}
