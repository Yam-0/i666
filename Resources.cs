using System;
using System.Collections.Generic;
using System.Drawing;

using asparagus;
using static asparagus.Game;

class Resources
{
	public static Bitmap wall0;
	public static Bitmap wall1;
	public static Bitmap wall2;
	public static Bitmap wall3;
	public static Bitmap bullet;

	public static Bitmap pistol;
	public static Bitmap hudPistol;
	public static Bitmap shotgun;
	public static Bitmap hudShotgun;
	public static Bitmap machinegun;
	public static Bitmap hudMachinegun;
	public static Bitmap sniper;
	public static Bitmap hudSniper;

	public static Bitmap heart;
	public static Bitmap ammo;

	public static Bitmap healthPickup;
	public static Bitmap ammoPickup;

	public static Bitmap exit;

	public static Bitmap guard;
	public static Bitmap marine;
	public static Bitmap officer;

	public static Bitmap titan;

	public static int titleWidth = 32;
	public static int titleHeight = 7;

	public static void Load()
	{
		string local = Environment.CurrentDirectory;

		wall0 = new Bitmap(local + "\\sprites\\wall0.png");
		wall1 = new Bitmap(local + "\\sprites\\wall1.png");
		wall2 = new Bitmap(local + "\\sprites\\wall2.png");
		wall3 = new Bitmap(local + "\\sprites\\wall3.png");
		bullet = new Bitmap(local + "\\sprites\\bullet.png");

		pistol = new Bitmap(local + "\\sprites\\pistol.png");
		hudPistol = new Bitmap(local + "\\sprites\\hudPistol.png");
		shotgun = new Bitmap(local + "\\sprites\\shotgun.png");
		hudShotgun = new Bitmap(local + "\\sprites\\hudShotgun.png");
		machinegun = new Bitmap(local + "\\sprites\\machinegun.png");
		hudMachinegun = new Bitmap(local + "\\sprites\\hudMachinegun.png");
		sniper = new Bitmap(local + "\\sprites\\sniper.png");
		hudSniper = new Bitmap(local + "\\sprites\\hudSniper.png");

		heart = new Bitmap(local + "\\sprites\\heart.png");
		ammo = new Bitmap(local + "\\sprites\\ammo.png");

		healthPickup = new Bitmap(local + "\\sprites\\healthPickup.png");
		ammoPickup = new Bitmap(local + "\\sprites\\ammoPickup.png");

		exit = new Bitmap(local + "\\sprites\\exit.png");

		guard = new Bitmap(local + "\\sprites\\guard.png");
		marine = new Bitmap(local + "\\sprites\\marine.png");
		officer = new Bitmap(local + "\\sprites\\officer.png");

		titan = new Bitmap(local + "\\sprites\\titan.png");
	}

	static public Sprite GetSprite(Bitmap _image)
	{
		Sprite wall;

		Bitmap image = _image;

		wall = new Sprite(image.Width, image.Height);

		asparagus.Game.Color[] wColors = new asparagus.Game.Color[wall.width * wall.height];
		string wChars = "";

		for (var i = 0; i < wall.width * wall.height; i++)
		{
			wChars += " ";
		}

		for (var x = 0; x < wall.width; x++)
		{
			for (var y = 0; y < wall.height; y++)
			{
				System.Drawing.Color pixelColor = image.GetPixel(x, y);
				int pixelIndex = Game.Index(x, y, wall.width);
				wColors[pixelIndex] = GetColor(x, y, image);
			}
		}

		wall.Load(wChars, wColors);

		return wall;
	}

	public static Sprite GetSpriteSlice(Sprite sprite, int slices, int index, bool horizontal)
	{
		if (horizontal)
		{
			Sprite[] sprites = new Sprite[slices];
			int sliceWidth = sprite.width / slices;
			for (var i = 0; i < sprites.Length; i++)
			{
				sprites[i] = new Sprite();
				sprites[i].sprite = new Pixel[sprite.height * sliceWidth];
			}

			for (var i = 0; i < sprites.Length; i++)
			{
				sprites[i].width = sliceWidth;
				sprites[i].height = sprite.height;

				for (var x = 0; x < sliceWidth; x++)
				{
					for (var y = 0; y < sprite.height; y++)
					{
						Pixel pixel = sprite.sprite[Index((i * sliceWidth) + x, y, sprite.width)];

						sprites[i].sprite[Index(x, y, sliceWidth)].Attributes = pixel.Attributes;
						sprites[i].sprite[Index(x, y, sliceWidth)].Char.UnicodeChar = pixel.Char.UnicodeChar;
					}
				}
			}

			if (index < 0 || index > sprites.Length) { index = 0; }
			return sprites[index];
		}
		else
		{
			Sprite[] sprites = new Sprite[slices];
			int sliceHeight = sprite.height / slices;
			for (var i = 0; i < sprites.Length; i++)
			{
				sprites[i] = new Sprite();
				sprites[i].sprite = new Pixel[sprite.width * sliceHeight];
			}

			for (var i = 0; i < sprites.Length; i++)
			{
				sprites[i].width = sprite.width;
				sprites[i].height = sliceHeight;

				for (var x = 0; x < sprite.width; x++)
				{
					for (var y = 0; y < sliceHeight; y++)
					{
						Pixel pixel = sprite.sprite[Index(x, (i * sliceHeight) + y, sprite.width)];

						sprites[i].sprite[Index(x, y, sprite.width)].Attributes = pixel.Attributes;
						sprites[i].sprite[Index(x, y, sprite.width)].Char.UnicodeChar = pixel.Char.UnicodeChar;
					}
				}
			}

			if (index < 0 || index > sprites.Length) { index = 0; }
			return sprites[index];
		}
	}

	public static Sprite GetSpriteSliceBoth(Sprite sprite, int slicesX, int slicesY, int x, int y)
	{
		sprite = GetSpriteSlice(sprite, slicesX, x, true);
		sprite = GetSpriteSlice(sprite, slicesY, y, false);

		return sprite;
	}

	public static asparagus.Game.Color GetColor(int x, int y, Bitmap image)
	{
		System.Drawing.Color cBLACK = System.Drawing.Color.FromArgb(255, 0, 0, 0);
		System.Drawing.Color cBLUE = System.Drawing.Color.FromArgb(255, 0, 0, 255);
		System.Drawing.Color cCYAN = System.Drawing.Color.FromArgb(255, 0, 255, 255);
		System.Drawing.Color cDARKBLUE = System.Drawing.Color.FromArgb(255, 0, 0, 100);
		System.Drawing.Color cDARKCYAN = System.Drawing.Color.FromArgb(255, 0, 100, 100);
		System.Drawing.Color cDARKGRAY = System.Drawing.Color.FromArgb(255, 100, 100, 100);
		System.Drawing.Color cDARKGREEN = System.Drawing.Color.FromArgb(255, 0, 100, 0);
		System.Drawing.Color cDARKRED = System.Drawing.Color.FromArgb(255, 100, 0, 0);
		System.Drawing.Color cDARKYELLOW = System.Drawing.Color.FromArgb(255, 100, 100, 0);
		System.Drawing.Color cGRAY = System.Drawing.Color.FromArgb(255, 200, 200, 200);
		System.Drawing.Color cGREEN = System.Drawing.Color.FromArgb(255, 0, 255, 0);
		System.Drawing.Color cMAGENTA = System.Drawing.Color.FromArgb(255, 255, 100, 100);
		System.Drawing.Color cRED = System.Drawing.Color.FromArgb(255, 255, 0, 0);
		System.Drawing.Color cWHITE = System.Drawing.Color.FromArgb(255, 255, 255, 255);
		System.Drawing.Color cYELLOW = System.Drawing.Color.FromArgb(255, 255, 255, 0);
		System.Drawing.Color cTRANSPARENT = System.Drawing.Color.FromArgb(255, 100, 10, 40);

		Game.Color color = new Game.Color(Game.BLACK, Game.BLACK); ;

		System.Drawing.Color pixelColor = image.GetPixel(x, y);

		if (pixelColor == cBLACK)
			color = new Game.Color(Game.BLACK, Game.BLACK);
		if (pixelColor == cBLUE)
			color = new Game.Color(Game.BLUE, Game.BLUE);
		if (pixelColor == cCYAN)
			color = new Game.Color(Game.CYAN, Game.CYAN);
		if (pixelColor == cDARKBLUE)
			color = new Game.Color(Game.DARKBLUE, Game.DARKBLUE);
		if (pixelColor == cDARKCYAN)
			color = new Game.Color(Game.DARKCYAN, Game.DARKCYAN);
		if (pixelColor == cDARKGRAY)
			color = new Game.Color(Game.DARKGRAY, Game.DARKGRAY);
		if (pixelColor == cDARKGREEN)
			color = new Game.Color(Game.DARKGREEN, Game.DARKGREEN);
		if (pixelColor == cDARKRED)
			color = new Game.Color(Game.DARKRED, Game.DARKRED);
		if (pixelColor == cDARKYELLOW)
			color = new Game.Color(Game.DARKYELLOW, Game.DARKYELLOW);
		if (pixelColor == cGRAY)
			color = new Game.Color(Game.GRAY, Game.GRAY);
		if (pixelColor == cGREEN)
			color = new Game.Color(Game.GREEN, Game.GREEN);
		if (pixelColor == cMAGENTA)
			color = new Game.Color(Game.MAGENTA, Game.MAGENTA);
		if (pixelColor == cRED)
			color = new Game.Color(Game.RED, Game.RED);
		if (pixelColor == cWHITE)
			color = new Game.Color(Game.WHITE, Game.WHITE);
		if (pixelColor == cYELLOW)
			color = new Game.Color(Game.YELLOW, Game.YELLOW);
		if (pixelColor == cTRANSPARENT)
			color = new Game.Color(Game.TRANSPARENT, Game.TRANSPARENT);

		return color;
	}

	public static string GetMap(int floor, int width, int height)
	{
		string map = "";

		switch (floor)
		{
			case 0:
				map += "################################";
				map += "#.......#....#........#........#";
				map += "#..............................#";
				map += "#.......##...#........#........#";
				map += "AAA...AAA....#........####..####";
				map += "A.......A.A..####.#####........#";
				map += "A.........A.....#.#............#";
				map += "AAAABAAAAAA....................#";
				map += "#...............#.#............#";
				map += "#...##...#.....................#";
				map += "#...##..###..####.####.........#";
				map += "#........#...#.......###......##";
				map += "#.............................##";
				map += "xx.xxx...#...#.......###..###.##";
				map += "x.......###..####.####....#....#";
				map += "x....x...#.....#...#......#....#";
				map += "xxxxxx...#.....#...#....####..##";
				map += "#........#.....#...#.....###..##";
				map += "#......#####..##...##...####..##";
				map += "#..###.##.##............#......#";
				map += "#..#....................#......#";
				map += "#..#########..#######...#......#";
				map += "#...##.....#............#.###.##";
				map += "#..####.................#.#....#";
				map += "#..............................#";
				map += "#..####....#....#############.##";
				map += "#.....#....#....#..............#";
				map += "#.....#....#....#....#...#....##";
				map += "#.###.######....##..###.###..###";
				map += "#..........#...................#";
				map += "#..............................#";
				map += "################################";
				break;
			case 4:
				map += "####AAAA########################";
				map += "#..........#####################";
				map += "#.x......#.#####################";
				map += "#..........#####################";
				map += "A...B..B...A####################";
				map += "A..........A####################";
				map += "A..........A####################";
				map += "A...B..B...A####################";
				map += "#..........#####################";
				map += "#.#......#.#####################";
				map += "#..........#####################";
				map += "####AAAA########################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				map += "################################";
				break;

			default:
				map = GenerateMap(width, height, floor);
				break;
		}

		return map;
	}

	public static string GenerateMap(int width, int height, int floor)
	{
		char[] map = new char[width * height];

		//Fill empty map
		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				map[Index(x, y, width)] = '.';
			}
		}

		//Draw edge
		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				bool edge = false;
				edge = (y == 0 || y == width - 1) ? true : edge;
				edge = (x == 0 || x == width - 1) ? true : edge;

				if (edge)
					map[Index(x, y, width)] = '#';
			}
		}

		Stack<Vector2> coords = new Stack<Vector2>();
		dir[] dirTiles = new dir[16];
		for (var i = 0; i < dirTiles.Length; i++)
			dirTiles[i] = new dir(false, false, false, false);
		bool[] visited = new bool[16];

		coords.Push(new Vector2(0, 0));
		Vector2 cursor = coords.Peek();
		visited[0] = true;
		int counter = 1;

		while (true)
		{
			if (counter >= 16)
				break;

			dir walkable = new dir(false, false, false, false);
			if (cursor.X != 0)
				if (!visited[Index(cursor.X - 1, cursor.Y, 4)])
					walkable.left = true;
			if (cursor.X != 3)
				if (!visited[Index(cursor.X + 1, cursor.Y, 4)])
					walkable.right = true;
			if (cursor.Y != 0)
				if (!visited[Index(cursor.X, cursor.Y - 1, 4)])
					walkable.up = true;
			if (cursor.Y != 3)
				if (!visited[Index(cursor.X, cursor.Y + 1, 4)])
					walkable.down = true;

			if (!walkable.up && !walkable.right && !walkable.down && !walkable.left)
			{
				cursor = coords.Pop();
				continue;
			}

			string dir = "";
			string[] dirs = new string[4] { "up", "right", "down", "left" };

			while (true)
			{
				Random random = new Random();
				int moveIndex = random.Next(5);

				if (moveIndex == 0 && walkable.up)
					dir = dirs[moveIndex];
				if (moveIndex == 1 && walkable.right)
					dir = dirs[moveIndex];
				if (moveIndex == 2 && walkable.down)
					dir = dirs[moveIndex];
				if (moveIndex == 3 && walkable.left)
					dir = dirs[moveIndex];

				if (dir != "")
					break;
			}

			if (dir == dirs[0])
			{
				dirTiles[Index(cursor.X, cursor.Y, 4)].up = true;
				dirTiles[Index(cursor.X, cursor.Y - 1, 4)].down = true;
			}
			if (dir == dirs[1])
			{
				dirTiles[Index(cursor.X, cursor.Y, 4)].right = true;
				dirTiles[Index(cursor.X + 1, cursor.Y, 4)].left = true;
			}
			if (dir == dirs[2])
			{
				dirTiles[Index(cursor.X, cursor.Y, 4)].down = true;
				dirTiles[Index(cursor.X, cursor.Y + 1, 4)].up = true;
			}
			if (dir == dirs[3])
			{
				dirTiles[Index(cursor.X, cursor.Y, 4)].left = true;
				dirTiles[Index(cursor.X - 1, cursor.Y, 4)].right = true;
			}

			int newX = cursor.X;
			int newY = cursor.Y;

			newY = (dir == dirs[0]) ? newY - 1 : newY;
			newX = (dir == dirs[1]) ? newX + 1 : newX;
			newY = (dir == dirs[2]) ? newY + 1 : newY;
			newX = (dir == dirs[3]) ? newX - 1 : newX;

			cursor = new Vector2(newX, newY);
			visited[Index(cursor.X, cursor.Y, 4)] = true;
			coords.Push(cursor);
			counter++;
		}

		TileType[] mapTiles = new TileType[16];

		for (var i = 0; i < dirTiles.Length; i++)
		{
			TileType tileType = TileType.X1;
			dir tdir = dirTiles[i];

			if (tdir.up && tdir.right && tdir.down && tdir.left) { tileType = TileType.X1; }

			if (!tdir.up && tdir.right && tdir.down && !tdir.left) { tileType = TileType.L1; }
			if (!tdir.up && !tdir.right && tdir.down && tdir.left) { tileType = TileType.L2; }
			if (tdir.up && !tdir.right && !tdir.down && tdir.left) { tileType = TileType.L3; }
			if (tdir.up && tdir.right && !tdir.down && !tdir.left) { tileType = TileType.L4; }

			if (!tdir.up && tdir.right && tdir.down && tdir.left) { tileType = TileType.T1; }
			if (tdir.up && !tdir.right && tdir.down && tdir.left) { tileType = TileType.T2; }
			if (tdir.up && tdir.right && !tdir.down && tdir.left) { tileType = TileType.T3; }
			if (tdir.up && tdir.right && tdir.down && !tdir.left) { tileType = TileType.T4; }

			if (tdir.up && !tdir.right && tdir.down && !tdir.left) { tileType = TileType.I1; }
			if (!tdir.up && tdir.right && !tdir.down && tdir.left) { tileType = TileType.I2; }

			if (!tdir.up && !tdir.right && tdir.down && !tdir.left) { tileType = TileType.U1; }
			if (!tdir.up && !tdir.right && !tdir.down && tdir.left) { tileType = TileType.U2; }
			if (tdir.up && !tdir.right && !tdir.down && !tdir.left) { tileType = TileType.U3; }
			if (!tdir.up && tdir.right && !tdir.down && !tdir.left) { tileType = TileType.U4; }

			mapTiles[i] = tileType;
		}

		string[] tiles = new string[16];
		for (var i = 0; i < 16; i++)
		{
			tiles[i] = GetTile(mapTiles[i]);
			char wall;
			switch (floor)
			{
				case 1:
				default:
					wall = '#';
					break;
				case 2:
					wall = 'x';
					break;
				case 3:
					wall = 'A';
					break;
			}

			string tile = "";
			for (var j = 0; j < tiles[i].Length; j++)
			{
				if (tiles[i][j] != '.')
				{
					tile += wall;
				}
				else
				{
					tile += '.';
				}
			}
			tiles[i] = tile;
		}

		for (var mapx = 0; mapx < 4; mapx++)
		{
			for (var mapy = 0; mapy < 4; mapy++)
			{
				for (var x = 0; x < 8; x++)
				{
					for (var y = 0; y < 8; y++)
					{
						char tileChar = tiles[Index(mapx, mapy, 4)][Index(x, y, 8)];
						if (tileChar != '.')
							map[Index(mapx * 8 + (7 - x), mapy * 8 + y, 32)] = tileChar;
					}
				}
			}
		}

		return new string(map);
	}

	private static string GetTile(TileType tileType)
	{
		Random random = new Random();

		string tile = "";

		string[] tilesL = new string[]
		{
			("########")+
			("########")+
			("##......")+
			("##......")+
			("##......")+
			("##......")+
			("##....##")+
			("##....##"),

			("########")+
			("#.....##")+
			("#.###...")+
			("#.###...")+
			("#.###...")+
			("#.......")+
			("##....##")+
			("##....##"),

			("########")+
			("########")+
			("##......")+
			("##.##...")+
			("##.##...")+
			("##......")+
			("##....##")+
			("##....##"),

			("########")+
			("#......#")+
			("#.####.#")+
			("#.#.....")+
			("#.#.....")+
			("#.#..###")+
			("#....###")+
			("###..###"),

			("########")+
			("#......#")+
			("#...#...")+
			("#...#...")+
			("#.###...")+
			("#......#")+
			("#......#")+
			("##...###"),

			("########")+
			("#......#")+
			("#.#.#..#")+
			("#.......")+
			("#.#.#...")+
			("#.......")+
			("#.....##")+
			("###...##"),
		};
		string[] tilesT = new string[]
		{
			("########")+
			("########")+
			("........")+
			("..####..")+
			("..####..")+
			("........")+
			("##....##")+
			("##....##"),

			("########")+
			("#......#")+
			("###..###")+
			("........")+
			("........")+
			("#..##..#")+
			("#......#")+
			("###..###"),

			("########")+
			("#......#")+
			("###..###")+
			("........")+
			("........")+
			("..####..")+
			("#......#")+
			("##....##"),

			("########")+
			("########")+
			("########")+
			("........")+
			("........")+
			("###..###")+
			("###..###")+
			("###..###"),

			("########")+
			("#......#")+
			("###..###")+
			("........")+
			("........")+
			("###..###")+
			("###..###")+
			("###..###"),

			("########")+
			("#......#")+
			("#..##..#")+
			("........")+
			("...##...")+
			("...##...")+
			("#......#")+
			("#......#"),
		};
		string[] tilesI = new string[]
		{
			("##....##")+
			("#..##..#")+
			("#......#")+
			("#..##..#")+
			("#..##..#")+
			("#......#")+
			("#..##..#")+
			("##....##"),

			("##....##")+
			("##....##")+
			("##....##")+
			("##....##")+
			("##....##")+
			("##....##")+
			("##....##")+
			("##....##"),

			("##....##")+
			("#......#")+
			("#.#..#.#")+
			("#......#")+
			("#......#")+
			("#.#..#.#")+
			("#......#")+
			("##....##"),

			("###..###")+
			("#......#")+
			("#.####.#")+
			("#.####.#")+
			("#.####.#")+
			("#.####.#")+
			("#......#")+
			("###..###"),

			("###..###")+
			("#......#")+
			("#.#..#.#")+
			("#.#..#.#")+
			("#.#..#.#")+
			("#.#..#.#")+
			("#......#")+
			("###..###"),

			("###..###")+
			("#......#")+
			("#.####.#")+
			("#......#")+
			("#......#")+
			("#.####.#")+
			("#......#")+
			("###..###"),
		};
		string[] tilesX = new string[]
		{
			("##....##")+
			("#......#")+
			("........")+
			("........")+
			("........")+
			("........")+
			("#......#")+
			("##....##"),

			("##....##")+
			("#......#")+
			("..####..")+
			("........")+
			("........")+
			("..####..")+
			("#......#")+
			("##....##"),

			("##....##")+
			("#......#")+
			("..#..#..")+
			("........")+
			("........")+
			("..#..#..")+
			("#......#")+
			("##....##"),
		};
		string[] tilesU = new string[]
		{
			("########")+
			("#.####.#")+
			("#.####.#")+
			("#......#")+
			("##....##")+
			("##....##")+
			("##....##")+
			("##....##"),

			("########")+
			("#......#")+
			("#......#")+
			("###..###")+
			("##....##")+
			("##....##")+
			("###..###")+
			("###..###"),

			("########")+
			("#......#")+
			("#.####.#")+
			("#.####.#")+
			("#.####.#")+
			("#.####.#")+
			("#......#")+
			("###..###"),

			("########")+
			("#.####.#")+
			("#......#")+
			("#.#..#.#")+
			("#.#..#.#")+
			("#.#..#.#")+
			("#......#")+
			("###..###"),

			("########")+
			("###..###")+
			("###..###")+
			("#......#")+
			("#.#..#.#")+
			("#.####.#")+
			("#......#")+
			("###..###"),

			("########")+
			("#......#")+
			("#.#..#.#")+
			("#.####.#")+
			("#......#")+
			("#......#")+
			("###..###")+
			("###..###"),
		};


		if (tileType == TileType.L1)
			tile = RotateTile(tilesL[random.Next(tilesL.Length)], 3);
		if (tileType == TileType.L2)
			tile = RotateTile(tilesL[random.Next(tilesL.Length)], 4);
		if (tileType == TileType.L3)
			tile = RotateTile(tilesL[random.Next(tilesL.Length)], 5);
		if (tileType == TileType.L4)
			tile = RotateTile(tilesL[random.Next(tilesL.Length)], 6);

		if (tileType == TileType.T1)
			tile = RotateTile(tilesT[random.Next(tilesT.Length)], 0);
		if (tileType == TileType.T2)
			tile = RotateTile(tilesT[random.Next(tilesT.Length)], 1);
		if (tileType == TileType.T3)
			tile = RotateTile(tilesT[random.Next(tilesT.Length)], 2);
		if (tileType == TileType.T4)
			tile = RotateTile(tilesT[random.Next(tilesT.Length)], 3);

		if (tileType == TileType.I1)
			tile = RotateTile(tilesI[random.Next(tilesI.Length)], 0);
		if (tileType == TileType.I2)
			tile = RotateTile(tilesI[random.Next(tilesI.Length)], 1);
		if (tileType == TileType.I3)
			tile = RotateTile(tilesI[random.Next(tilesI.Length)], 2);
		if (tileType == TileType.I4)
			tile = RotateTile(tilesI[random.Next(tilesI.Length)], 3);

		if (tileType == TileType.X1)
			tile = RotateTile(tilesX[random.Next(tilesX.Length)], 0);
		if (tileType == TileType.X2)
			tile = RotateTile(tilesX[random.Next(tilesX.Length)], 1);
		if (tileType == TileType.X3)
			tile = RotateTile(tilesX[random.Next(tilesX.Length)], 2);
		if (tileType == TileType.X4)
			tile = RotateTile(tilesX[random.Next(tilesX.Length)], 3);

		if (tileType == TileType.U1)
			tile = RotateTile(tilesU[random.Next(tilesU.Length)], 0);
		if (tileType == TileType.U2)
			tile = RotateTile(tilesU[random.Next(tilesU.Length)], 1);
		if (tileType == TileType.U3)
			tile = RotateTile(tilesU[random.Next(tilesU.Length)], 2);
		if (tileType == TileType.U4)
			tile = RotateTile(tilesU[random.Next(tilesU.Length)], 3);

		return tile;
	}

	public static string RotateTile(string tile, int turns)
	{
		char[] rotatedTile = new char[tile.Length];
		for (var i = 0; i < tile.Length; i++)
		{
			rotatedTile[i] = tile[i];
		}

		turns = (int)Overflowf(turns, 0, 4);

		for (var i = 0; i < turns; i++)
		{
			tile = new string(rotatedTile);

			for (var x = 0; x < 8; x++)
			{
				for (var y = 0; y < 8; y++)
				{
					rotatedTile[Index(x, y, 8)] = tile[Index(8 - y - 1, x, 8)];
				}
			}
		}

		return new string(rotatedTile);
	}

	public class dir
	{
		public bool up = false;
		public bool right = false;
		public bool down = false;
		public bool left = false;

		public dir(bool _up, bool _right, bool _down, bool _left)
		{
			up = _up;
			right = _right;
			down = _down;
			left = _left;
		}
	}

	public enum TileType
	{
		L1, L2, L3, L4,
		T1, T2, T3, T4,
		I1, I2, I3, I4,
		X1, X2, X3, X4,
		U1, U2, U3, U4
	}

	public static Vector2 PopulateMap(int floor, string map, int width, int height, List<asparagus.Game.Object> objects, List<Enemy> enemies, Difficulty difficulty)
	{
		if (floor == 0)
		{
			WorldObjects.PlaceObject(2, 3, objects, WorldObjects.Pistol());
			WorldObjects.PlaceObject(3, 4, objects, WorldObjects.Shotgun());
			WorldObjects.PlaceObject(1, 3, objects, WorldObjects.Machinegun());
			WorldObjects.PlaceObject(3, 3, objects, WorldObjects.Sniper());

			return new Vector2(5, 5);
		}

		int px = 0;
		int py = 0;

		bool spawnNotSet = true;

		int closePistols = 1;
		int pistols = 3;
		int shotguns = 2;
		int machineguns = 2;
		int snipers = 1;
		int health = 6;
		int ammo = 6;

		int exits = 2;

		int guards = 8;
		int marines = 0;
		int officers = 0;

		if (floor == 2) { marines = 4; }
		if (floor == 3) { officers = 2; }

		if (floor == 4)
		{
			pistols = 2;
			shotguns = 1;
			machineguns = 1;
			snipers = 1;
			health = 4;
			ammo = 4;

			exits = 0;

			guards = 0;
			marines = 0;
			officers = 0;

			Enemy enemy = new Enemy();
			enemy = Enemies.Titan(difficulty);
			WorldObjects.PlaceEnemy(5.5f, 5.5f, objects, enemies, enemy);

			px = 10;
			py = 1;
			spawnNotSet = true;
		}

		Random random = new Random(floor);

		bool remaining = true;
		while (remaining)
		{
			if (!(
				closePistols > 0 ||
				pistols > 0 ||
				shotguns > 0 ||
				machineguns > 0 ||
				snipers > 0 ||
				spawnNotSet ||
				health > 0 ||
				ammo > 0 ||
				exits > 0 ||
				guards > 0 ||
				marines > 0 ||
				officers > 0
			))
			{
				remaining = false;
			}

			int x = random.Next(0, width);
			int y = random.Next(0, height);

			if (map[Index(x, y, width)] == '.')
			{
				bool intersect = false;
				foreach (var Object in objects)
				{
					if (x == (int)Math.Round(Object.x) && y == (int)Math.Round(Object.y))
						intersect = true;
					if (x == px && y == py)
						intersect = true;
				}
				if (intersect)
					continue;

				if (spawnNotSet)
				{
					px = x;
					py = y;
					spawnNotSet = false;
					continue;
				}

				if (closePistols > 0)
				{
					float dist = MathF.Sqrt((x - px) * (x - px) + (y - py) * (y - py));
					if (dist < 2.0f)
					{
						WorldObjects.PlaceObject(x, y, objects, WorldObjects.Pistol());
						closePistols--;
						continue;
					}
					continue;
				}
				if (pistols > 0)
				{
					WorldObjects.PlaceObject(x, y, objects, WorldObjects.Pistol());
					pistols--;
					continue;
				}
				if (shotguns > 0)
				{
					WorldObjects.PlaceObject(x, y, objects, WorldObjects.Shotgun());
					shotguns--;
					continue;
				}
				if (machineguns > 0)
				{
					WorldObjects.PlaceObject(x, y, objects, WorldObjects.Machinegun());
					machineguns--;
					continue;
				}
				if (snipers > 0)
				{
					WorldObjects.PlaceObject(x, y, objects, WorldObjects.Sniper());
					snipers--;
					continue;
				}
				if (health > 0)
				{
					WorldObjects.PlaceObject(x, y, objects, WorldObjects.HealthPickup());
					health--;
					continue;
				}
				if (ammo > 0)
				{
					WorldObjects.PlaceObject(x, y, objects, WorldObjects.AmmoPickup());
					ammo--;
					continue;
				}
				if (exits > 0)
				{
					float dist = MathF.Sqrt((x - px) * (x - px) + (y - py) * (y - py));
					if (dist > 16.0f)
					{
						WorldObjects.PlaceObject(x, y, objects, WorldObjects.Exit());
						exits--;
						continue;
					}
				}
				if (guards > 0)
				{
					float dist = MathF.Sqrt((x - px) * (x - px) + (y - py) * (y - py));
					if (dist > 4.0f)
					{
						Enemy enemy = new Enemy();
						enemy = Enemies.Guard(difficulty);
						WorldObjects.PlaceEnemy(x, y, objects, enemies, enemy);
						guards--;
						continue;
					}
				}
				if (marines > 0)
				{
					float dist = MathF.Sqrt((x - px) * (x - px) + (y - py) * (y - py));
					if (dist > 4.0f)
					{
						Enemy enemy = new Enemy();
						enemy = Enemies.Marine(difficulty);
						WorldObjects.PlaceEnemy(x, y, objects, enemies, enemy);
						marines--;
						continue;
					}
				}
				if (officers > 0)
				{
					float dist = MathF.Sqrt((x - px) * (x - px) + (y - py) * (y - py));
					if (dist > 4.0f)
					{
						Enemy enemy = new Enemy();
						enemy = Enemies.Officer(difficulty);
						WorldObjects.PlaceEnemy(x, y, objects, enemies, enemy);
						officers--;
						continue;
					}
				}
			}
		}

		return new Vector2(px, py);
	}

	public static Pixel[] GetTitle()
	{
		string titleChars = "";
		titleChars += "  _____     __      __      __  ";
		titleChars += " |_   _|   / /     / /     / /  ";
		titleChars += "   | |    / /_    / /_    / /_  ";
		titleChars += "   | |   | '_ \\  | '_ \\  | '_ \\ ";
		titleChars += "  _| |_  | (_) | | (_) | | (_) |";
		titleChars += " |_____|  \\___/   \\___/   \\___/ ";
		titleChars += "                                ";

		asparagus.Game.Color[] titleColors = new asparagus.Game.Color[titleWidth * titleHeight];
		for (var i = 0; i < titleColors.Length; i++)
		{
			titleColors[i] = new Game.Color(RED, BLACK);
		}

		Pixel[] pixels = new Pixel[titleWidth * titleHeight];
		for (var i = 0; i < pixels.Length; i++)
		{
			Pixel pixel = new Pixel();
			pixel.Char.UnicodeChar = titleChars[i];
			pixel.Attributes = (short)ColorInt(titleColors[i].foreground, titleColors[i].background);
			pixels[i] = pixel;
		}

		return pixels;
	}

}
public struct Sprite
{
	public int width { get; set; }
	public int height { get; set; }

	public Pixel[] sprite { get; set; }

	public Sprite(int _width, int _height)
	{
		width = _width;
		height = _height;
		sprite = new Pixel[_width * _height];
	}

	public void Load(string chars, asparagus.Game.Color[] colors)
	{
		for (var i = 0; i < width * height; i++)
		{
			sprite[i].Char.UnicodeChar = chars[i];
			sprite[i].Attributes = (short)ColorInt(colors[i].foreground, colors[i].background);
		}
	}

	public Pixel Sample(float nx, float ny)
	{
		nx = Overflowf(nx, 0, 1);
		ny = Overflowf(ny, 0, 1);

		int sx = (int)Math.Round(Mapf(nx, 0, 1, 0, width - 1));
		int sy = (int)Math.Round(Mapf(ny, 0, 1, 0, height - 1));

		Pixel pixel = new Pixel();
		int si = Index(sx, sy, width);

		pixel.Char.UnicodeChar = sprite[si].Char.UnicodeChar;
		pixel.Attributes = sprite[si].Attributes; ;

		return pixel;
	}
}

public enum Difficulty
{
	Easy,
	Normal,
	Hard
}