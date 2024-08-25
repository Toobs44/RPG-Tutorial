using Godot;
using System;

public partial class SoftCollision : Area2D
{
    public bool IsColliding()
    {
        var areas = GetOverlappingAreas();
        return areas.Count > 0;
    }

    public Vector2 GetPushVector()
    {
        var areas = GetOverlappingAreas();
        Vector2 pushVector = Vector2.Zero;
        if(IsColliding())
        {
            Area2D area = areas[0];
            pushVector = area.GlobalPosition.DirectionTo(GlobalPosition);
            // how can I normalize this?
            //pushVector = pushVector.Normalize();
        }
        return pushVector;
    }
}
