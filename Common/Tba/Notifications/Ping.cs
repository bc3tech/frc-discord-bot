﻿namespace Common.Tba.Notifications;
#nullable disable
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
public readonly record struct Ping(string title, string desc);
