using Godot;
using System;

public partial class JoyStick : TouchScreenButton
{
	public Sprite2D knob;
	public Vector2 stickCenter;
	private float maxDistance;
	bool touched = false; // do I use this?
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		knob = GetNode<Sprite2D>("Knob");
		var joyStick = this.Shape as CircleShape2D;
		stickCenter = TextureNormal.GetSize()/2;
		maxDistance = joyStick.Radius;
		SetProcess(false);
	}

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventScreenTouch touchEvent)
		{
			if(touchEvent.Pressed)
			{
				SetProcess(true);
			}

			else if(!touchEvent.Pressed)
			{
				SetProcess(false);
				knob.Position = stickCenter;
			}
		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		knob.GlobalPosition = GetGlobalMousePosition();
		knob.Position = stickCenter + (knob.Position - stickCenter).LimitLength(maxDistance);
	}

	public Vector2 GetJoystickDir()
	{
		var dir = knob.Position - stickCenter;
		return dir.Normalized();
	}
}
