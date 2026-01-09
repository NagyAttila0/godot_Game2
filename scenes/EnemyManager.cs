// using Godot;
// using System;

// public partial class EnemyManager : Node
// {
//     private int _enemyCount = 0;
//     private const int MaxHealth = 15; // 15 HP a maximum

//     public override void _Ready()
//     {
//         // Ez a sor megkeresi az összes "enemies" címkével ellátott node-ot
//         var enemies = GetTree().GetNodesInGroup("enemies");
//         _enemyCount = enemies.Count;
//         GD.Print($"Manager: {_enemyCount} ellenség betöltve.");
//     }

//     public void OnEnemyDied()
//     {
//         _enemyCount--;
//         GD.Print($"Szörny meghalt! Még {_enemyCount} van hátra.");

//         // Gyógyítás +1 HP minden ölésnél
//         var player = GetTree().CurrentScene.FindChild("player", true, false) as Player;
//         if (player != null && player.Health < MaxHealth)
//         {
//             player.Health += 1;
//             FrissitHPBar(player);
//         }

//         // Kulcs megadása a legvégén
//         if (_enemyCount <= 0)
//         {
//             if (player != null)
//             {
//                 player.HasKey = true;
//                 player.Health = MaxHealth; // Full gyógyítás a végén
//                 FrissitHPBar(player);
//                 GD.Print("MINDENKI HALOTT! MEGKAPTAD A KULCSOT!");
//             }
//         }
//     }

//     private void FrissitHPBar(Player p)
//     {
//         // A korábban kitapasztalt útvonalat használjuk
//         var hpBar = p.GetNodeOrNull<ProgressBar>("CanvasLayer/HealthBar");
//         if (hpBar != null) hpBar.Value = p.Health;
//     }
// }