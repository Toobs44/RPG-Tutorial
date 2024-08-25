using Godot;
using System;

public partial class PlayerDetectionZone : Area2D
{
    //variable for the players location.
    public Node2D player;

    public void OnBodyEntered(Node2D body)
    {
        player = body;//detects player location
    }

    public void OnBodyExited(Node2D body)
    {
        player = null;// forgets player location
    }

    public bool CanSeePlayer()
    {
        return player != null;
    }
}
