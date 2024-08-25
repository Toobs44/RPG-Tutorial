using Godot;
using System;

public partial class WanderContoller : Node2D
{
	[Export]
	int wonderRange = 32;
	Vector2 startPosition;
	public Vector2 targetPosition;
	Random randomRange = new Random();
	Timer timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		startPosition = GlobalPosition;
		targetPosition = GlobalPosition;
		timer = GetNode<Timer>("Timer");
		UpdateTargetPosition();
	}

	public void UpdateTargetPosition()
	{
		Vector2 targetVector = new Vector2(randomRange.Next(-wonderRange, wonderRange), randomRange.Next(-wonderRange, wonderRange));
		targetPosition = startPosition + targetVector;
		//do I need to add GlobalPostion = targetPostion;?
		GlobalPosition = targetPosition;
		//GD.Print("Position Updated");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public double GetTimeLeft()
	{
		return timer.TimeLeft;//why do I care how much time is left?
	}

	public void StartWanderTimer(int duration)
	{
		timer.Start(duration);
	}

	void OnTimerTimeout()
	{
		UpdateTargetPosition();
		//GD.Print("Updating Target Position");
	}
}
