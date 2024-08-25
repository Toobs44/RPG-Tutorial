using Godot;
using System;

public partial class Stats : Node
{
	[Export]
	public int MaxHealth 
	{
		get => maxHealth; 
		set => SetMaxHealth(value);
		} 
	int health;
	int maxHealth;
	[Signal]
	public delegate void NoHealthEventHandler();// I think all custom signals need EventHandler added to it
	//if I want to create a singleton using this script should I create the static instance here?
	//creating a singlton for info to be stored in a variable in the ready method.
	[Signal]
	public delegate int HealthChangedEventHandler(int value);
	[Signal]
	public delegate int MaxHealthChangedEventHandler(int value);
	public static Node Instance {get; private set;}//Singleton instance
	
	//why do I need to create this Health variable to store and set again later
	//is this a variable or a new function?
	public int Health 
	{
		get => health;// Im not sure what => is doing
		set 
		{ 
			health = Mathf.Clamp(value, 0, maxHealth);
			// when the health changes emit signal up with the new health value.
			EmitSignal(nameof(HealthChanged), health);
			
			// when health reaches zero signal up to the parent node
			if(health == 0)
			{
				// instead of nameof(NoHealth) I could just do "NoHealth" but if I spell it wrong I wont get an error.
				EmitSignal(nameof(NoHealth));
			} 
		}
	}

	public void SetMaxHealth(int value)
	{
		maxHealth = value;

		// set the health to which ever of the two values are lower
		this.Health = Mathf.Min(health, MaxHealth);
		EmitSignal(nameof(MaxHealthChanged), maxHealth);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	
		// 	Instance = this;// Setup the singleton to be accessed from C# code without "GetNode()"
		// However I am reusing this code for the bats and the player. 
		// because of that instancing from the other scripts will prevent bugs

		Health = MaxHealth;//sets health to max health which is intially chosen in the editor.
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
