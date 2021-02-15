using System;
using System.Collections.Generic;
using asparagus;
using static asparagus.Game;

class Weapons : i666_Vertex.I666
{
	public static Weapon pistol;
	public static Weapon shotgun;
	public static Weapon machinegun;
	public static Weapon sniper;

	private const float pi = 3.14159f;

	public static void Load()
	{
		Sprite hudSprite = new Sprite(2, 2);
		hudSprite.Load("    ", new Color[]{
			new Color(WHITE,BLACK),
			new Color(BLACK,BLACK),
			new Color(WHITE,BLACK),
			new Color(BLACK,BLACK)
		});

		Sprite groundSprite = new Sprite(2, 2);
		groundSprite.Load("    ", new Color[]{
			new Color(WHITE,BLACK),
			new Color(BLACK,BLACK),
			new Color(WHITE,BLACK),
			new Color(BLACK,BLACK),
		});

		Sprite bulletSprite = Resources.GetSprite(Resources.bullet);

		asparagus.Game.Object bullet = new asparagus.Game.Object(0, 0, 0, 0, 0, 1, 4, bulletSprite, "bullet");

		Weapon _pistol = new Weapon("Pistol", 15.0f, 0.35f, 4.0f, Resources.GetSprite(Resources.pistol), Resources.GetSprite(Resources.hudPistol), bullet, false);
		Weapon _shotgun = new Weapon("Shotgun", 15.0f, 0.45f, 6.0f, Resources.GetSprite(Resources.shotgun), Resources.GetSprite(Resources.hudShotgun), bullet, false);
		Weapon _machinegun = new Weapon("Machinegun", 10.0f, 0.10f, 8.0f, Resources.GetSprite(Resources.machinegun), Resources.GetSprite(Resources.hudMachinegun), bullet, true);
		Weapon _sniper = new Weapon("Sniper", 60.0f, 1.0f, 12.0f, Resources.GetSprite(Resources.sniper), Resources.GetSprite(Resources.hudSniper), bullet, false);

		pistol = _pistol;
		shotgun = _shotgun;
		machinegun = _machinegun;
		sniper = _sniper;
	}

	public static bool Fire(Weapon weapon, float x, float y, float pa, List<asparagus.Game.Object> objects)
	{
		float bulletX;
		float bulletY;

		bool fired = true;

		switch (weapon.name)
		{
			case "Pistol":
				bulletX = MathF.Sin((pa + 90) * (pi / 180)) * weapon.speed;
				bulletY = MathF.Cos((pa + 90) * (pi / 180)) * weapon.speed;
				objects.Add(new Object(x, y, 0.40f, bulletX, bulletY, 1, 4, weapon.bullet.sprite, "bullet"));
				break;
			case "Machinegun":
				bulletX = MathF.Sin((pa + 90) * (pi / 180)) * weapon.speed;
				bulletY = MathF.Cos((pa + 90) * (pi / 180)) * weapon.speed;
				objects.Add(new Object(x, y, 0.40f, bulletX, bulletY, 1, 4, weapon.bullet.sprite, "bullet"));

				break;
			case "Shotgun":
				for (var i = 0; i < 5; i++)
				{
					bulletX = MathF.Sin((pa + 90 + (i * 5) - 10) * (pi / 180)) * weapon.speed;
					bulletY = MathF.Cos((pa + 90 + (i * 5) - 10) * (pi / 180)) * weapon.speed;
					objects.Add(new Object(x, y, 0.40f, bulletX, bulletY, 1, 4, weapon.bullet.sprite, "bullet"));
				}
				break;
			case "Sniper":
				bulletX = MathF.Sin((pa + 90) * (pi / 180)) * weapon.speed;
				bulletY = MathF.Cos((pa + 90) * (pi / 180)) * weapon.speed;
				objects.Add(new Object(x, y, 0.40f, bulletX, bulletY, 1, 4, weapon.bullet.sprite, "bullet"));
				break;
			default:
				fired = false;
				break;
		}

		return fired;
	}
}
public struct Weapon
{
	public string name { get; }
	public float damage { get; }
	public float cooldown { get; }
	public float speed { get; }

	public Sprite hudSprite { get; }
	public Sprite handSprite { get; }
	public asparagus.Game.Object bullet { get; }

	public bool hold { get; }

	public Weapon(
			string _name,
			float _damage,
			float _cooldown,
			float _speed,
			Sprite _hudSprite,
			Sprite _handSprite,
			asparagus.Game.Object _bullet,
			bool _hold)
	{
		name = _name;
		damage = _damage;
		cooldown = _cooldown;
		speed = _speed;
		hudSprite = _hudSprite;
		handSprite = _handSprite;
		bullet = _bullet;
		hold = _hold;
	}
}

class WorldObjects : i666_Vertex.I666
{
	public static Object Pistol()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 6, 16, Resources.GetSprite(Resources.pistol), "pistol");
	}
	public static Object Shotgun()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 16, 16, Resources.GetSprite(Resources.shotgun), "shotgun");
	}
	public static Object Machinegun()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 12, 16, Resources.GetSprite(Resources.machinegun), "machinegun");
	}
	public static Object Sniper()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 18, 16, Resources.GetSprite(Resources.sniper), "sniper");
	}
	public static Object HealthPickup()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 8, 16, Resources.GetSprite(Resources.healthPickup), "healthPickup");
	}
	public static Object AmmoPickup()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 8, 16, Resources.GetSprite(Resources.ammoPickup), "ammoPickup");
	}
	public static Object Exit()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 8, 16, Resources.GetSprite(Resources.exit), "exit");
	}
	public static Object Guard()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 7, 60, Resources.GetSprite(Resources.guard), "guard");
	}
	public static Object Marine()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 7, 60, Resources.GetSprite(Resources.guard), "marine");
	}
	public static Object Officer()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 7, 60, Resources.GetSprite(Resources.guard), "officer");
	}
	public static Object GuardCorpse()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 8, 60, Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.guard), 4, 2, 3, 0), "corpse");
	}
	public static Object MarineCorpse()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 8, 60, Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.marine), 4, 2, 3, 0), "corpse");
	}
	public static Object OfficerCorpse()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 8, 60, Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.officer), 4, 2, 3, 0), "corpse");
	}
	public static Object Titan()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 30, 100, Resources.GetSprite(Resources.titan), "titan");
	}
	public static Object TitanCorpse()
	{
		return new asparagus.Game.Object(0, 0, 0, 0, 0, 30, 100, Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.titan), 4, 2, 3, 0), "corpse");
	}

	public static void PlaceObject(float x, float y, List<Object> objects, Object _object)
	{
		Game.Object sObject = new Game.Object(0, 0, 0, 0, 0, _object.width, _object.height, _object.sprite, _object.name);
		sObject = _object;
		_object.x = x;
		_object.y = y;
		objects.Add(sObject);
	}
	public static void PlaceEnemy(float x, float y, List<Object> objects, List<Enemy> enemies, Enemy _enemy)
	{
		PlaceObject(x, y, objects, _enemy.enemy);
		Enemy enemy = new Enemy();
		enemy = _enemy;
		enemy.ex = x;
		enemy.ey = y;
		enemies.Add(enemy);
	}
}