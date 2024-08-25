using Godot;
using System;

public partial class camera_2d : Camera2D
{
	Marker2D topLeft;
	Marker2D BottomRight;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		topLeft = GetNode<Marker2D>("Limits/TopLeft");
		BottomRight = GetNode<Marker2D>("Limits/BottomRight");

		var limitTop = topLeft.Position.Y;
		var limitLeft = topLeft.Position.X;
		var limitBottom = BottomRight.Position.Y;
		var limitRight = BottomRight.Position.X;
		LimitBottom = (int)limitBottom;
		LimitLeft = (int)limitLeft;
		LimitRight = (int)limitRight;
		LimitTop = (int)limitTop;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
