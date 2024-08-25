using Godot;
using System;

public partial class SwordHitbox : Hitbox
{
    public Vector2 knockbackVector = Vector2.Zero;

    public override void _Ready()
    {
        //GD.Print("SwordHitbox ready, knockbackVector: " + knockbackVector);
    }
}
