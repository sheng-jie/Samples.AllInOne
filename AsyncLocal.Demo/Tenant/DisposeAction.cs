﻿namespace AsyncLocal.Demo.Tenant;

public class DisposeAction : IDisposable
{
    private readonly Action _action;

    public DisposeAction(Action action)
    {
        _action = action;
    }

    public void Dispose() => this._action();
}