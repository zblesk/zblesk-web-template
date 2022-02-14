﻿namespace zblesk_web.Configs;

public class MatrixConfig
{
    public const string ConfigName = "Matrix";

    public bool Enabled { get; set; } = false;
    public string Homeserver { get; set; }
    public string RoomId { get; set; }
    public string AccessToken { get; set; }
}
