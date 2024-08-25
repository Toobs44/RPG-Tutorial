using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

public partial class Player : CharacterBody2D
{
	public const int MAXSPEED = 80;
	public const int ACCELRATION = 500;
	public const int FRICTION = 500;
	public const int ROLLSPEED = 125;
	
	public AnimationPlayer animationPlayer = null;
	public AnimationTree animationTree = null;
	PackedScene PlayerHurtSound = (PackedScene)ResourceLoader.Load("res://Player/player_hurt_sound.tscn");
	public AnimationNodeStateMachinePlayback animationState;// if I dont use this class it auto selects variant and that doesnt work.
	public Vector2 velocity = Vector2.Zero;
	public Vector2 rollVector = Vector2.Down;  
	public CollisionShape2D hitBoxCollision;
	public SwordHitbox swordHitbox;
	public Hurtbox hurtbox;
	Hitbox batHitBox;
	AnimationPlayer blinkAnimationPlayer;
	// Reference to the Stats singleton
	/*!!!!! I thought I had to do this but it can be accessed directly from anywhere ... right?*/
	Stats playerStats; 

	//should the playerStats here be accessing the global singleton or just from the playerStats node?
	// is there a diffence?

	//public Enum state;
	// What exactly is an enum?
	enum gameState{
		MOVE,
		ROLL,
		ATTACK
	}

	private gameState state = gameState.MOVE;//the state machines default position

    public override void _Ready()
    {
		//playerStats = Stats.Instance;
		playerStats = GetNode<Stats>("/root/PlayerStats");
		GD.Print($"Players Health: {playerStats.Health}");
		playerStats.Connect("NoHealth", new Callable(this, nameof(OnNoHealth)));
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		animationTree.Active = true;
		animationState = animationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>(); // I can either add the .As() or use a cast Im not sure which is better.
		hitBoxCollision = GetNode<CollisionShape2D>("HitboxPivot/SwordHitbox/CollisionShape2D");
		swordHitbox = GetNode<SwordHitbox>("HitboxPivot/SwordHitbox");
		hurtbox = GetNode<Hurtbox>("HurtBox");
		//batHitBox = GetNode<Hitbox>("HitBox"); I need this to connect to the bats hit box
		blinkAnimationPlayer = GetNode<AnimationPlayer>("BlinkAnimationPlayer");
    }

    public override void _PhysicsProcess(double delta)
	{
		//switch between the different state in the statemachine.
		switch (state)
		{
			case gameState.MOVE:
				MoveState(delta);
				break;
			case gameState.ROLL:
				RollState(delta);
				break;
			case gameState.ATTACK:
				AttackState(delta);
				break;
		}
		
	}

	public void OnHurtBoxAreaEntered(Area2D area)
	{
		// this makes sure the hitbox node being accessed is vague enough to be used by any emeny not just the bats
		Hitbox batHitbox = area as Hitbox;
		//Later the players health should be changed by the specific monsters damage not hard coded like this.
		//playerStats.Health -= 1;
		playerStats.Health -= batHitbox.damage;
		//GD.PrintS($"Players health: {playerStats.Health}");
		hurtbox.StartInvincibility(1.0);
		hurtbox.CreateHitEffect();
		AudioStreamPlayer playerHurtSound = (AudioStreamPlayer)PlayerHurtSound.Instantiate();//should this be casted as a audio stream player?
		GetTree().CurrentScene.AddChild(playerHurtSound);
	}

	void OnHurtBoxInvincibilityStarted()
	{
		blinkAnimationPlayer.Play("Start");
	}

	void OnHurtBoxInvincibilityEnded()
	{
		blinkAnimationPlayer.Play("Stop");
	}

	private void OnNoHealth()
	{
		QueueFree();
	}

	public void MoveState(double delta)
	{
		Vector2 inputVector = Vector2.Zero;
		
		//GetActionStrength gets a number between -1 to 1. 
		//Useful for analog sticks and joysticks.
		inputVector.X = Input.GetActionStrength("right") - Input.GetActionStrength("left");
		inputVector.Y = Input.GetActionStrength("down") - Input.GetActionStrength("up");
		inputVector = inputVector.Normalized();

		if (inputVector != Vector2.Zero)
		{
			rollVector = inputVector;
			animationTree.Set("parameters/Idle/blend_position", inputVector);
			animationTree.Set("parameters/Run/blend_position", inputVector);
			animationTree.Set("parameters/Attack/blend_position", inputVector);
			animationTree.Set("parameters/Roll/blend_position", inputVector);
			animationState.Travel("Run");//Travel is not recogized unless the animationState is a animationNodeStateMachinePlayback()
			velocity = velocity.MoveToward(inputVector * MAXSPEED, ACCELRATION * (float)delta);
		}
		else 
		{
			animationState.Travel("Idle");
			velocity = velocity.MoveToward(Vector2.Zero, FRICTION * (float)delta);
		}
		
		//call the move function to start moving.
		move();

		//if the attack key is pressed switch to that game state
		if (Input.IsActionJustPressed("attack"))//Travel is not recogize unless the animationState is a animationNodeStateMachinePlayback()
		{
			swordHitbox.knockbackVector = rollVector;
			state = gameState.ATTACK;
		}

		//if the roll key is pressed switch to that game state
		if(Input.IsActionJustPressed("roll"))
		{
			state = gameState.ROLL;
			/*************/
			/*debug*/
			//playerStats.MaxHealth += 1;
			/*************/
			
		}
	}
	
	public void RollState(double delta)
	{
		velocity = rollVector * ROLLSPEED;
		animationState.Travel("Roll");
		move();//calling this function allows the player to move while rolling.
	}

	public void AttackState(double delta)
	{
		Velocity = Vector2.Zero;
		animationState.Travel("Attack");
	}

	public void move()
	{
		Velocity = velocity;
		MoveAndSlide();
	}

	//used to transistion back to the move state after certain animations finish
	public void BackToMoveState()
	{
		velocity /= 2;//setting the velocity to zero also works but might feel a little choppy
		state = gameState.MOVE;
	}	
}
