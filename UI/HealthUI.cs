using Godot;
using System;

public partial class HealthUI : Control
{
	[Export]
	int hearts = 4;
	[Export]
	int maxHearts = 4;

	TextureRect heartUIFull;
	Vector2 textureSizeFull;
	Vector2 textureSizeEmpty;
	TextureRect heartUIEmpty;
	
	Stats playerStats; // a reference to the Stats singleton.

	public int MaxHearts
	{
		get => maxHearts;// gets the private variable maxHearts.
		private set => SetMaxHearts(value);
	}
	public int Hearts
	{
		get => hearts;// gets the private variable hearts.
		private set => SetHearts(value);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// this is already instanced in the stats script and will crash the game if used here.
		// how do I use this singleton then?
		//I do not need to access the Player stats this way sense it should bet accessable from any scene.
		playerStats = GetNode<Stats>("/root/PlayerStats");
		//playerStats = Stats.Instance;// Access to the Stats singleton.
		
		heartUIEmpty = GetNode<TextureRect>("HeartUIEmpty");
		textureSizeEmpty = heartUIEmpty.Size;
		heartUIFull = GetNode<TextureRect>("HeartUIFull");
		textureSizeFull = heartUIFull.Size;
		this.MaxHearts = playerStats.MaxHealth;// sets max hearts the same number of max health in Stats.
		this.Hearts = playerStats.Health;// set hearts the same number of max health in Stats. 
		/*Sense this is in the ready function couldnt we just connect it to public Health variable?
		That way the maxHealth and health variable can stay private?*/
		// listen for HealthChanged signal then call the SetHearts method.
		playerStats.Connect("HealthChanged", new Callable(this, nameof(SetHearts)));
		playerStats.Connect("MaxHealthChanged", new Callable(this, nameof(SetMaxHearts)));
	}

	void SetHearts(int value)
	{
		// this ensures the hearts are never less than 0 and more than maxHeart varible.
		hearts = Mathf.Clamp(value, 0, maxHearts);
		// Update the UI to show the current number of hearts
		if(heartUIFull != null)
		{
			textureSizeFull.X = hearts * 15;
		}
		//Update the size property to see the changes
		heartUIFull.Size = textureSizeFull;
	}
	
	void SetMaxHearts(int value)
	{
		//Mathf.Max() takes the higher of the to values.
		//This ensure the maxHearts value is never lower than 1.
		// I think this should be MaxHearts and not maxHearts but not sure.
		// DO NOT use MaxHearts, it will create a loop and crash the game!
		maxHearts = Mathf.Max(value, 1);
		// Set the max amount of heart the player can hold
		if(heartUIEmpty != null)
		{
			textureSizeEmpty.X = maxHearts * 15;
		}
		// Update the size property for textureRect at the end.
		heartUIEmpty.Size = textureSizeEmpty;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
