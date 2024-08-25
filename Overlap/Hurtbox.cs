using Godot;
using System;

public partial class Hurtbox : Area2D
{
    [Signal]
    public delegate void InvincibilityStartedEventHandler();
    [Signal]
    public delegate void InvincibilityEndedEventHandler();
    PackedScene HitEffect = (PackedScene)ResourceLoader.Load("res://Effects/HitEffect.tscn"); 
    CollisionShape2D collisionShape2D;

    private bool invincible;

    public bool Invincible
    {
        get => invincible;// Im not really sure how the get works
        set => SetInvincible(value);//this calls the SetInvincible method but I'm not clear how "value" is set
    }

    Timer timer;

    public override void _Ready()
    {
        timer = GetNode<Timer>(nameof(Timer));
        collisionShape2D = GetNode<CollisionShape2D>(nameof(CollisionShape2D));
    }
    public void StartInvincibility(double duration)
    {
        // this is called from the parent node with a duration given for the timer
        this.Invincible = true;// setting this triggers the getter setter. I think true fills in the "value" argument
        timer.Start(duration);// calls the timer child node to start.
        
    }

    public void OnInvincibilityStarted()
    {
        //turns off monitoring to make the sprite invincible.
        SetDeferred("monitoring", false);//if in quotes make sure "monitoring" is lower case
    }

    public void OnInvincibilityEnded()
    {
        // turns monitoring back on to back the sprite vulnerable
        this.Monitoring = true;
    }

    public void SetInvincible(bool value)
    {
        //stores "value" in variable then emits one signal or another if it is true or false
        invincible = value;
        if(invincible)
        {
            EmitSignal(nameof(InvincibilityStarted));
        }
        else
        {
            EmitSignal(nameof(InvincibilityEnded));
        }
    }
    public void CreateHitEffect()
    {
        // when called by a parent node, instatiate the effect in the position the parent was in
        Node2D effect = (Node2D)HitEffect.Instantiate();
        var main = GetTree().CurrentScene;
        main.AddChild(effect);
        effect.GlobalPosition = GlobalPosition;
    }

    void OnTimerTimeout()
    {
        //when the timer runs out set the bool to false.
        //setting this triggers the getter setter. 
        //I think true fills in the SetInvincible argument "value".
        this.Invincible = false;
    }
}
