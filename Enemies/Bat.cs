using Godot;
using System;

public partial class Bat : CharacterBody2D
{
	[Export]
	int ACCELRATION = 300;
	[Export]
	int MAXSPEED = 50;
	[Export]
	int FRICTION = 200;
	[Export]
	int WANDERBUFFER = 4;

	public Vector2 velocity = Vector2.Zero;
	Stats stats;//global variable for the stats node.
	// Is this really a globel variable?

	// a preloaded scene for the death effect animation.
	PackedScene EnemyDeathEffect = (PackedScene)ResourceLoader.Load("res://Effects/EnemyDeathEffect.tscn");
	PlayerDetectionZone playerDetectionZone;//global variable for playerDetectionZone
	AnimatedSprite2D sprite;//globel variable for the bat sprite
	AnimationPlayer animationPlayer;
	Hurtbox hurtbox;
	SoftCollision softcollions;
	Random random;// varible for storing the random method
	WanderContoller wanderController;// variable for the WanderController node.
	batState[] stateArray; // = { batState.IDLE, batState.WANDER};// this is needed to store the IDLE and WANDER states for the PickRandomState Method
	Vector2 direction;
	public enum batState
	{
		IDLE,
		WANDER,
		CHASE
	}

	batState state = batState.IDLE;// Start the state machine in IDLE.


    public override void _Ready()
    {
		// this connects to the stats node
        stats = GetNode<Stats>("Stats");
		// this calls down to the stats node to find the remaining health
		GD.Print($"Bats health: {stats.Health}");
		playerDetectionZone = GetNode<PlayerDetectionZone>(nameof(PlayerDetectionZone));
		sprite = GetNode<AnimatedSprite2D>(nameof(AnimatedSprite2D));
		hurtbox = GetNode<Hurtbox>("HurtBox");
		softcollions = GetNode<SoftCollision>(nameof(SoftCollision));
		random = new Random();
		animationPlayer = GetNode<AnimationPlayer>(nameof(AnimationPlayer));
		wanderController = GetNode<WanderContoller>(nameof(WanderContoller));
		
		// Initialize the state array with the IDLE and WANDER states
		stateArray = new batState[] {batState.IDLE, batState.WANDER};
    }

    public override void _PhysicsProcess(double delta)
	{
		velocity = velocity.MoveToward(Vector2.Zero, 200 * (float)delta);
		Velocity = velocity;
		MoveAndSlide();

		switch (state)
		{
			case batState.IDLE:
			velocity = velocity.MoveToward(Vector2.Zero, FRICTION * (float)delta);
			SeekPlayer();
			CheckNewState();
			break;

			case batState.WANDER:
			SeekPlayer();
			CheckNewState();
			MoveToPoint(wanderController.targetPosition, delta);
			if(GlobalPosition.DistanceTo(wanderController.targetPosition) <= WANDERBUFFER)
			{
				state = PickRandomState(stateArray);
				GD.Print($" Picked state {state}");
			}
			Velocity = velocity;
			MoveAndSlide();
			break;

			case batState.CHASE:
			Node2D player = playerDetectionZone.player;
			if(playerDetectionZone.CanSeePlayer())
			{
				MoveToPoint(player.GlobalPosition, delta);
			}
			if(softcollions.IsColliding())
			{
				velocity += softcollions.GetPushVector() * (float)delta * 400;
				Velocity = velocity;
			}
			Velocity = velocity;
			MoveAndSlide();
			CheckNewState();
			break;
		}
	}

	void MoveToPoint(Vector2 point, double delta)
	{
			direction = GlobalPosition.DirectionTo(point);
			// should I add .Normalized? otherwise the bats are increadably fast.
			velocity = velocity.MoveToward(direction * MAXSPEED, ACCELRATION * (float)delta);
			// flip the position for the bat in the direction it is moving
			sprite.FlipH = velocity.X < 0;
	}

	public void SeekPlayer()
	{
		// do I need a parameter in CanSeePlayer?
		if (playerDetectionZone.CanSeePlayer())
		{
			state = batState.CHASE;
			GD.Print("Player Sighted");
		}
	}


	public void OnHurtBoxAreaEntered(Area2D area)
	{
		// I store the swordHitbox into a node
		SwordHitbox swordHitbox = GetNode<SwordHitbox>("../Player/HitboxPivot/SwordHitbox");
		
		velocity = swordHitbox.knockbackVector * 120;
		
		//GD.Print("checking health");
		// I call down to the stats node and change the "health" based on the damage created by sword hitbox.
		stats.Health -= swordHitbox.damage;
		//GD.Print($"bat health: {stats.Health}");
		hurtbox.CreateHitEffect();
		hurtbox.StartInvincibility(0.4);
		
	}

	public void OnStatsNoHealth()
	{
		QueueFree();
		Node2D enemyDeathEffect = (Node2D)EnemyDeathEffect.Instantiate();
		GetParent().AddChild(enemyDeathEffect);
		enemyDeathEffect.GlobalPosition = GlobalPosition;
	}

    //I wonder If I could find a way to combine PickRandomState and CheckNewState...
	batState PickRandomState(batState[] stateList)
	{
		
		int index = random.Next(stateList.Length);
		return stateList[index];
	}
	void CheckNewState()
	{
		if(wanderController.GetTimeLeft() == 0)
			{
				state = PickRandomState(stateArray);
				GD.Print($"NewState {state}");
				wanderController.StartWanderTimer(random.Next(1,3));
			}
	}

	void OnHurtBoxInvincibilityStarted()
	{
		animationPlayer.Play("Start");
	}

	void OnHurtBoxInvincibilityEnded()
	{
		animationPlayer.Play("Stop");
	}
}
