using Godot;
using GodotPlugins.Game;
using System;

public partial class Grass : Node2D
{
	// this preloads the scene once to be used later
	PackedScene grassEffectScene = (PackedScene)ResourceLoader.Load("res://Effects/GrassEffect.tscn");


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{}
	public void CreateGrassEffect()
	{
		Node2D grassEffect = (Node2D)grassEffectScene.Instantiate();
			Node world = GetTree().CurrentScene;
			GetParent().AddChild(grassEffect);
			grassEffect.GlobalPosition = GlobalPosition;
	}

	public void OnHurtBoxAreaEntered(Area2D area)
	{
		CreateGrassEffect();
		QueueFree();
	}
}
