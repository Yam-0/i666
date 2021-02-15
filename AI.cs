using System;
using System.Collections.Generic;
using asparagus;

public class Enemy
{
	public float health;
	public float maxHealth;
	public float speed;
	public float enemyRadius;
	public float reactionTime;
	public float viewRange;
	public float firerate;
	public string type = "Enemy";
	public string name = "Enemy";

	public float ex;
	public float ey;
	public float vx;
	public float vy;

	public asparagus.Game.Object enemy;
	public State state;
	public int frame;
	public float shootTimer;
	public float animationTimer = 0;
	public bool animationSwitch;

	public bool found = false;
	public bool foundOnce = false;
	public bool alive = true;
	public bool sleep = false;

	const float pi = 3.14159f;
	public Sprite[] waiting;
	public Sprite[] walking;
	public Sprite[] attacking;
	public Sprite[] dying;
	public asparagus.Game.Object corpse;

	public bool Update(float deltaTime, List<asparagus.Game.Object> objects, float px, float py, string map, Weapon currentWeapon, AudioManager audioManager)
	{
		if (sleep) { return false; }
		for (int i = objects.Count - 1; i >= 0; i--)
		{
			if (objects[i].name == "bullet")
			{
				float dist = MathF.Sqrt((ex - objects[i].x) * (ex - objects[i].x) + (ey - objects[i].y) * (ey - objects[i].y));
				if (dist <= enemyRadius)
				{
					health -= currentWeapon.damage;
					if (health > 0)
					{
						audioManager.PlaySound("HitWall.wav");
					}
					objects.Remove(objects[i]);
				}
			}
		}

		if (animationTimer > 0) { animationTimer -= deltaTime; }

		if (alive)
		{
			if (!foundOnce)
			{
				state = State.waiting;
			}
			if (enemy.dist < viewRange)
			{
				foundOnce = true;
				state = State.attacking;
			}
			else
			{
				state = State.searching;
			}
			if (health <= 0)
			{
				state = State.dying;
				animationTimer = 0.2f;
				alive = false;
				audioManager.PlaySound("Death.wav");
			}
		}

		shootTimer = (shootTimer > 0) ? shootTimer -= deltaTime : shootTimer;
		if (state == State.attacking && shootTimer <= 0)
		{
			if (name == "Titan")
			{
				float etpa = MathF.Atan2(py - ey, px - ex) * (180 / pi);
				float x2 = MathF.Cos((etpa + 90) * (pi / 180)) / 2.3f;
				float y2 = MathF.Sin((etpa + 90) * (pi / 180)) / 2.3f;

				float gx1 = ex + x2;
				float gy1 = ey + y2;
				float gx2 = ex - x2;
				float gy2 = ey - y2;

				float gtpa1 = MathF.Atan2(py - gy1, px - gx1) * (180 / pi);
				float gtpa2 = MathF.Atan2(py - gy2, px - gx2) * (180 / pi);

				float bdx1 = MathF.Cos((gtpa1) * (pi / 180)) * 4;
				float bdy1 = MathF.Sin((gtpa1) * (pi / 180)) * 4;
				float bdx2 = MathF.Cos((gtpa2) * (pi / 180)) * 4;
				float bdy2 = MathF.Sin((gtpa2) * (pi / 180)) * 4;

				objects.Add(new asparagus.Game.Object(ex + x2, ey + y2, 0.45f, bdx1, bdy1, 2, 8, Resources.GetSprite(Resources.bullet), "enemyBullet"));
				objects.Add(new asparagus.Game.Object(ex - x2, ey - y2, 0.45f, bdx2, bdy2, 2, 8, Resources.GetSprite(Resources.bullet), "enemyBullet"));
				shootTimer = 1.0f / firerate;
			}
			else
			{
				float etpa = MathF.Atan2(py - ey, px - ex) * 180 / pi;
				float y1 = MathF.Sin((etpa) * (pi / 180)) * 4;
				float x1 = MathF.Cos((etpa) * (pi / 180)) * 4;
				objects.Add(new asparagus.Game.Object(ex, ey, 0.40f, x1, y1, 1, 4, Resources.GetSprite(Resources.bullet), "enemyBullet"));
				shootTimer = 1.0f / firerate;
			}
		}

		vx = 0;
		vy = 0;

		switch (state)
		{
			case State.waiting:
				enemy.sprite = waiting[0];
				break;

			case State.walking:
			case State.searching:
				if (animationSwitch)
				{
					enemy.sprite = walking[0];
				}
				else
				{
					enemy.sprite = walking[1];
				}
				if (animationTimer <= 0)
				{
					animationSwitch = !animationSwitch;
					animationTimer = 0.4f;
				}
				break;

			case State.attacking:
				enemy.sprite = attacking[0];
				break;

			case State.dying:
				enemy.sprite = dying[0];
				if (animationTimer < 0)
				{
					sleep = true;
					Die(objects);
					if (name == "Titan")
					{
						return true;
					}
				}
				break;
		}

		ex += vx * deltaTime * speed;
		ey += vy * deltaTime * speed;
		int mx = (int)Math.Round(ex);
		int my = (int)Math.Round(ey);

		for (var x = mx - 1; x < mx + 2; x++)
		{
			for (var y = my - 1; y < my + 2; y++)
			{
				if (map[asparagus.Game.Index(x, y, 32)] != '.')
				{
					float idx = MathF.Abs(ex - x);
					float idy = MathF.Abs(ey - y);

					if (idx < 0.5f + enemyRadius && idy < 0.5f + enemyRadius)
					{
						if (idx > idy)
						{
							if (ex >= x) { ex += 0.5f - idx + enemyRadius; }
							if (ex < x) { ex -= 0.5f - idx + enemyRadius; }
						}
						else
						{
							if (ey >= y) { ey += 0.5f - idy + enemyRadius; }
							if (ey < y) { ey -= 0.5f - idy + enemyRadius; }
						}
					}
				}
			}
		}

		enemy.x = ex;
		enemy.y = ey;

		return false;
	}

	public void Die(List<asparagus.Game.Object> objects)
	{
		objects.Remove(enemy);
		WorldObjects.PlaceObject(ex, ey, objects, corpse);
	}
}

public class Enemies
{
	public static Enemy Guard(Difficulty difficulty)
	{
		Enemy guard = new Enemy();

		switch (difficulty)
		{
			case Difficulty.Easy:
				guard.maxHealth = 40.0f;
				guard.speed = 0.8f;
				guard.firerate = 0.8f;
				guard.viewRange = 3.0f;
				guard.reactionTime = 0.7f;
				break;
			case Difficulty.Normal:
				guard.maxHealth = 60.0f;
				guard.speed = 1.0f;
				guard.firerate = 1.0f;
				guard.viewRange = 4.0f;
				guard.reactionTime = 0.5f;
				break;
			case Difficulty.Hard:
				guard.maxHealth = 80.0f;
				guard.speed = 1.4f;
				guard.firerate = 1.5f;
				guard.viewRange = 7.0f;
				guard.reactionTime = 0.2f;
				break;
			default:
				break;
		}

		guard.enemyRadius = 0.2f;
		guard.type = "Enemy";
		guard.name = "Guard";
		guard.health = guard.maxHealth;
		guard.enemy = WorldObjects.Guard();

		guard.waiting = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.guard), 4, 2, 0, 0),
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.guard), 4, 2, 1, 0)
		};
		guard.walking = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.guard), 4, 2, 0, 1),
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.guard), 4, 2, 1, 1)
		};
		guard.attacking = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.guard), 4, 2, 2, 1),
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.guard), 4, 2, 3, 1)
		};
		guard.dying = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.guard), 4, 2, 2, 0)
		};
		guard.corpse = WorldObjects.GuardCorpse();

		return guard;
	}
	public static Enemy Marine(Difficulty difficulty)
	{
		Enemy marine = new Enemy();

		switch (difficulty)
		{
			case Difficulty.Easy:
				marine.maxHealth = 60.0f;
				marine.speed = 1.0f;
				marine.firerate = 1.5f;
				marine.viewRange = 4.0f;
				marine.reactionTime = 0.6f;
				break;
			case Difficulty.Normal:
				marine.maxHealth = 80.0f;
				marine.speed = 1.2f;
				marine.firerate = 2.0f;
				marine.viewRange = 5.0f;
				marine.reactionTime = 0.4f;
				break;
			case Difficulty.Hard:
				marine.maxHealth = 110.0f;
				marine.speed = 1.7f;
				marine.firerate = 3.0f;
				marine.viewRange = 5.0f;
				marine.reactionTime = 0.3f;
				break;
			default:
				break;
		}

		marine.enemyRadius = 0.2f;
		marine.type = "Enemy";
		marine.name = "Marine";
		marine.health = marine.maxHealth;
		marine.enemy = WorldObjects.Guard();

		marine.waiting = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.marine), 4, 2, 0, 0),
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.marine), 4, 2, 1, 0)
		};
		marine.walking = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.marine), 4, 2, 0, 1),
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.marine), 4, 2, 1, 1)
		};
		marine.attacking = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.marine), 4, 2, 2, 1),
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.marine), 4, 2, 3, 1)
		};
		marine.dying = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.marine), 4, 2, 2, 0)
		};
		marine.corpse = WorldObjects.MarineCorpse();

		return marine;
	}
	public static Enemy Officer(Difficulty difficulty)
	{
		Enemy officer = new Enemy();

		switch (difficulty)
		{
			case Difficulty.Easy:
				officer.maxHealth = 80.0f;
				officer.speed = 1.2f;
				officer.firerate = 2.0f;
				officer.viewRange = 4.0f;
				officer.reactionTime = 0.5f;
				break;
			case Difficulty.Normal:
				officer.maxHealth = 120.0f;
				officer.speed = 1.4f;
				officer.firerate = 4.0f;
				officer.viewRange = 6.0f;
				officer.reactionTime = 0.2f;
				break;
			case Difficulty.Hard:
				officer.maxHealth = 160.0f;
				officer.speed = 2.0f;
				officer.firerate = 6.0f;
				officer.viewRange = 8.0f;
				officer.reactionTime = 0.1f;
				break;
			default:
				break;
		}

		officer.enemyRadius = 0.2f;
		officer.type = "Enemy";
		officer.name = "Officer";
		officer.health = officer.maxHealth;
		officer.enemy = WorldObjects.Guard();

		officer.waiting = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.officer), 4, 2, 0, 0),
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.officer), 4, 2, 1, 0)
		};
		officer.walking = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.officer), 4, 2, 0, 1),
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.officer), 4, 2, 1, 1)
		};
		officer.attacking = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.officer), 4, 2, 2, 1),
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.officer), 4, 2, 3, 1)
		};
		officer.dying = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.officer), 4, 2, 2, 0)
		};
		officer.corpse = WorldObjects.OfficerCorpse();

		return officer;
	}
	public static Enemy Titan(Difficulty difficulty)
	{
		Enemy titan = new Enemy();

		switch (difficulty)
		{
			case Difficulty.Easy:
				titan.maxHealth = 500.0f;
				titan.speed = 0.2f;
				titan.firerate = 2.0f;
				titan.viewRange = 16.0f;
				titan.reactionTime = 0.5f;
				break;
			case Difficulty.Normal:
				titan.maxHealth = 750.0f;
				titan.speed = 0.4f;
				titan.firerate = 4.0f;
				titan.viewRange = 16.0f;
				titan.reactionTime = 0.2f;
				break;
			case Difficulty.Hard:
				titan.maxHealth = 1250.0f;
				titan.speed = 0.6f;
				titan.firerate = 6.0f;
				titan.viewRange = 16.0f;
				titan.reactionTime = 0.1f;
				break;
			default:
				break;
		}

		titan.enemyRadius = 0.5f;
		titan.type = "Enemy";
		titan.name = "Titan";
		titan.health = titan.maxHealth;
		titan.enemy = WorldObjects.Titan();

		titan.waiting = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.titan), 4, 2, 0, 0),
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.titan), 4, 2, 1, 0)
		};
		titan.walking = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.titan), 4, 2, 0, 1),
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.titan), 4, 2, 1, 1)
		};
		titan.attacking = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.titan), 4, 2, 2, 1),
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.titan), 4, 2, 3, 1)
		};
		titan.dying = new Sprite[]
		{
			Resources.GetSpriteSliceBoth(Resources.GetSprite(Resources.titan), 4, 2, 2, 0)
		};
		titan.corpse = WorldObjects.TitanCorpse();

		return titan;
	}
}

public enum State
{
	waiting,
	walking,
	attacking,
	searching,
	dying
}