using Godot;
using System;

public partial class Door : Area2D
{

	private bool playerNearby = false;

	private Player playerRef;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node body){
		if(body is Player player){
			playerNearby = true;
			playerRef = player;
		}   
	}

	private void OnBodyExited(Node body){
		if(body is Player player){
			playerNearby = false;
			playerRef = null;
		}   
	}

	public override async void _Process(double delta)
{
	if (playerNearby && Input.IsActionJustPressed("ui_accept"))
	{
		if (playerRef != null && playerRef.HasKey)
		{
			GD.Print("Ajtó kinyitva, nyertél!");
			
			var winScreen = GetTree().CurrentScene.FindChild("WinScreen", true, false) as Control;

			if (winScreen != null)
			{
				winScreen.Visible = true;
				GetTree().Paused = true;

				// Vár 15 másodpercet (akkor is, ha a játék szünetel)
				await ToSignal(GetTree().CreateTimer(15.0, true), "timeout");

				GD.Print("Idő letelt, kilépés...");
				GetTree().Quit();
			}
			else
			{
				GD.Print("HIBA: Nem találom a WinScreen-t!");
			}

			QueueFree(); 
		}
		else
		{
			GD.Print("Nincs kulcs nálad!");
		}
	}
}


}
