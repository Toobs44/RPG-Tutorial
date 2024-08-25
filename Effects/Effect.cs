using Godot;
using System;

//should I replace GrassEffect or Node2D with AnimatedSprite2D to make this more modular? 

public partial class Effect : AnimatedSprite2D
{
	AnimatedSprite2D animatedSprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//!!!Can I use this in the ttrpg character sheet?!!!
		// if this was connecting to a different node I would need to add that node reference before Play and Connect
		// But I am connecting to the node the script is in so I dont need to do that.
		Play("Animate");
		Connect("animation_finished", new Callable(this, nameof(OnAnimationFinished)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnAnimationFinished()
	{
		QueueFree();
	}
}
