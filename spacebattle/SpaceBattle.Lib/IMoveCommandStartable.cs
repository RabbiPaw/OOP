﻿namespace SpaceBattle.Lib;

public interface IMoveCommandStartable
{
    IUObject Target { get; }
    Dictionary<string, object> Properties { get; }
}
