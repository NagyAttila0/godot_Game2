using Godot;
using System;

public partial class GameOverScreen : Control
{
	public override void _Ready()
	{
		// Figyelj a nevekre! Pontosan az legyen a gomb neve, ami a fán (RestartButton, QuitButton)
		GetNode<Button>("RestartButton").Pressed += OnRestartButtonPressed;
		GetNode<Button>("QuitButton").Pressed += OnQuitButtonPressed;
	}

	private void OnRestartButtonPressed()
	{
		GetTree().Paused = false; // Fontos: vedd le a szünetet!
		GetTree().ReloadCurrentScene();
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
