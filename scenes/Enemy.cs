

using Godot;
using System;
using System.Threading.Tasks;

public partial class Enemy : CharacterBody2D
{
	[Export] public int Health = 10;
	[Export] public int Damage = 1;
	[Export] public float Speed = 60f;

	private AnimatedSprite2D _sprite;
	private ProgressBar _hpBar;
	private Node2D _player;
	private bool _isDead = false;
	private bool _isAttacking = false;
	private bool _playerInRange = false;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_hpBar = GetNodeOrNull<ProgressBar>("EnemyHPBar");
		
		if (_hpBar != null)
		{
			_hpBar.MaxValue = Health;
			_hpBar.Value = Health;
		}

		var detectionArea = GetNodeOrNull<Area2D>("DetectionArea"); 
		if (detectionArea != null)
		{
			detectionArea.BodyEntered += (body) => { 
				if (body.Name.ToString().ToLower() == "player") _playerInRange = true; 
			};
			detectionArea.BodyExited += (body) => { 
				if (body.Name.ToString().ToLower() == "player") _playerInRange = false; 
			};
		}

		_player = GetTree().Root.FindChild("player", true, false) as Node2D;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isDead || _isAttacking || _player == null) return;

		float dist = GlobalPosition.DistanceTo(_player.GlobalPosition);
		
		if (_playerInRange)
		{
			Vector2 dir = (_player.GlobalPosition - GlobalPosition).Normalized();
			
			// Ha 35 pixelnél messzebb van, sétál (walk)
			if (dist > 35) 
			{
				Velocity = dir * Speed;
				_sprite?.Play("walk"); // Pontos név a listádból
				if (_sprite != null) _sprite.FlipH = dir.X < 0;
				MoveAndSlide();
			}
			else // Ha közel ér, támad (attack)
			{
				Velocity = Vector2.Zero;
				StartAttack();
			}
		}
		else
		{
			_sprite?.Play("idle"); // Pontos név a listádból
		}
	}

	private async void StartAttack()
{
	if (_isAttacking || _isDead) return;
	_isAttacking = true;
	
	_sprite?.Play("attack"); // Az Enemy listájából: "attack"
	await ToSignal(_sprite, "animation_finished");
	
	// Itt a javítás:
	if (!_isDead && _playerInRange && _player != null)
	{
		// Közvetlenül hívjuk meg a TakeDamage-et a playeren
		if (_player.HasMethod("TakeDamage"))
		{
			_player.Call("TakeDamage", Damage);
			GD.Print("Player megsebezve! Új HP: " + _player.Get("Health"));
		}
	}
	_isAttacking = false;
}

	public void TakeDamage(int amount)
	{
		if (_isDead) return;
		Health -= amount;
		if (_hpBar != null) _hpBar.Value = Health;
		if (Health <= 0) Die();
	}

	private async void Die()
	{
		if (_isDead) return;
		_isDead = true;
		_sprite?.Play("death"); // Pontos név a listádból
		if (_sprite != null) await ToSignal(_sprite, "animation_finished");
		QueueFree();
	}
}
