using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 80f; 
	[Export] public int Health = 5;
	[Export] public int Damage = 2;

	private AnimatedSprite2D _animatedSprite;
	private bool _isDead = false;
	private bool _isAttacking = false;
	public bool HasKey = false; 
	private string _currentDir = "front"; // Alapértelmezett irány

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (_animatedSprite != null)
		{
			_animatedSprite.AnimationFinished += OnAnimationFinished;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isDead) return;

		HandleAttackInput();
		HandleMovement(delta);
	}

	private void HandleAttackInput()
	{
		if (Input.IsMouseButtonPressed(MouseButton.Left) && !_isAttacking)
		{
			_isAttacking = true;
			// Automatikusan pl. "side_attack" vagy "front_attack"
			_animatedSprite?.Play(_currentDir + "_attack");
			CheckHit();
		}
	}

	private void HandleMovement(double delta)
	{
		// A legbiztosabb mozgás, ami nem dob Key hibát
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Velocity = direction * Speed;

		if (direction != Vector2.Zero)
		{
			UpdateDirection(direction);
			if (!_isAttacking) _animatedSprite?.Play(_currentDir + "_walk");
		}
		else
		{
			if (!_isAttacking) _animatedSprite?.Play(_currentDir + "_idle");
		}

		MoveAndSlide();
	}

	// Irány meghatározása az animációkhoz (front, back, side)
	private void UpdateDirection(Vector2 dir)
	{
		if (Mathf.Abs(dir.X) > Mathf.Abs(dir.Y))
		{
			_currentDir = "side";
			if (_animatedSprite != null) _animatedSprite.FlipH = dir.X < 0;
		}
		else
		{
			_currentDir = dir.Y > 0 ? "front" : "back";
			if (_animatedSprite != null) _animatedSprite.FlipH = false;
		}
	}

	public void PickupKey() { HasKey = true; GD.Print("Kulcs felvéve!"); }

	public void TakeDamage(int amount)
	{
		if (_isDead) return;
		Health -= amount;
		GD.Print("Játékos megütve! Maradék HP: " + Health);
		var hpBar = GetNodeOrNull<ProgressBar>("HealthBar");
		if (hpBar != null)
		{ 
			hpBar.Value = Health;
		}
		if (Health <= 0) 
		{
			Die();
		}
	}

	private void Die() // Kivettem az 'async' szót, mert így nem fog várakozni
{
	if (_isDead) return;
	_isDead = true;
	
	GD.Print("Játékos meghalt!");

	if (_animatedSprite != null)
	{
		_animatedSprite.Play("death"); // Ez elindítja a halált
	}

	// AZONNAL hozza fel a menüt, ne várjon az animációra
	var menu = GetTree().CurrentScene.FindChild("GameOverScreen", true, false) as Control;
	if (menu != null)
	{
		menu.Visible = true;
		GetTree().Paused = true; 
	}
}

	public void CheckHit()
	{
		var area = GetNodeOrNull<Area2D>("AttackArea");
		if (area != null)
		{
			foreach (var body in area.GetOverlappingBodies())
			{
				if (body != this && body.HasMethod("TakeDamage"))
					body.Call("TakeDamage", Damage);
			}
		}
	}

	private void OnAnimationFinished()
	{
		if (_animatedSprite != null && _animatedSprite.Animation.ToString().Contains("attack"))
		{
			_isAttacking = false;
		}
	}
}
