using Godot;
using System;

public partial class PlayerHurtSound : AudioStreamPlayer
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Connect the signal manually.
		Connect("finished", new Callable(this, "QueueFree"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
