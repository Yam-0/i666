using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using asparagus;

namespace i666_Vertex
{
	class I666 : asparagus.Game
	{
		string map;
		bool[] visionMap;

		Sprite wall0;
		Sprite wall1;
		Sprite wall2;
		Sprite wall3;

		Sprite healthIcon;
		Sprite ammoIcon;

		Pixel[] title;

		Stopwatch stopWatch;
		TimeSpan shootTimer1;
		TimeSpan shootTimer2;

		AudioManager audioManager;

		float iVertical;
		float iHorizintal;
		float iSprint;
		float iRot;
		bool iDown;
		bool iUp;
		bool iRight;
		bool iLeft;
		bool iBack;
		bool iConfirm;
		bool iFire;

		bool mMasterVolume;
		int iMasterVolume = 75;
		bool mMusicVolume;
		int iMusicVolume = 75;
		bool mOtherVolume;
		int iOtherVolume = 75;

		float px;
		float py;
		//float pz = 1.0f;
		float pa = 0.0f;
		int pHealth;
		int pAmmo;
		int floor;

		Difficulty vDifficulty;
		Difficulty _currentDifficulty = Difficulty.Normal;
		Difficulty currentDifficulty = Difficulty.Normal;
		bool mDifficulty;
		bool godmode = false;
		bool mGodmode;
		bool vGodmode;
		bool revealMap = false;
		bool mRevealMap;
		bool vRevealMap;

		Weapon currentWeapon;

		int mx;
		int my;

		int sMapWidth = 32;
		int sMapHeight = 32;

		float pWalkSpeed = 1.0f;
		float pSprintMult = 2.0f;
		float pRotSpeed = 120.0f;
		float pPlayerRadius = 0.2f;

		float[] depthBuffer;
		float iFov = 90.0f;
		bool mFov;
		float cFov = 90.0f;
		float cStepSize = 0.025f;
		bool mMaxRenderDistance;
		float cMaxRenderDistance = 10.0f;
		float cDepth = 80.0f;
		const float pi = 3.14159f;

		int sceneIndex;
		int sceneIndexBuffer;
		int lastSceneIndex;
		bool sceneStart;

		int menuIndex;
		int subMenuIndex;
		bool showSubMenu;

		float timer;
		float endTimer;
		bool end;

		List<Enemy> enemies;
		float hat = 0.0f;

		public override void Setup()
		{
			Loading();
			sceneIndex = 0;
			lastSceneIndex = -1;

			menuIndex = 0;
			subMenuIndex = 0;
			showSubMenu = false;

			Resources.Load();
			Weapons.Load();

			wall0 = Resources.GetSprite(Resources.wall0);
			wall1 = Resources.GetSprite(Resources.wall1);
			wall2 = Resources.GetSprite(Resources.wall2);
			wall3 = Resources.GetSprite(Resources.wall3);

			healthIcon = Resources.GetSprite(Resources.heart);
			ammoIcon = Resources.GetSprite(Resources.ammo);

			stopWatch = new Stopwatch();
			stopWatch.Start();
			shootTimer1 = stopWatch.Elapsed;
			shootTimer2 = stopWatch.Elapsed;

			audioManager = new AudioManager();
			audioManager.menu = true;
			Thread thread = new Thread(audioManager.PlayBGM);
			thread.Start();

			title = Resources.GetTitle();

			depthBuffer = new float[sWidth];
		}

		public void Reset()
		{
			Loading();
			floor = 1;
			pHealth = 75;
			pAmmo = 80;
			currentWeapon = new Weapon();
			vGodmode = godmode;
			vRevealMap = revealMap;
			vDifficulty = currentDifficulty;

			map = Resources.GetMap(floor, sMapWidth, sMapHeight);
			visionMap = new bool[sMapWidth * sMapHeight];

			for (var i = 0; i < visionMap.Length; i++)
			{
				visionMap[i] = revealMap;
			}

			enemies = new List<Enemy>();
			objects = new List<Object>();
			Vector2 sp = Resources.PopulateMap(floor, map, sMapWidth, sMapHeight, objects, enemies, vDifficulty);

			px = sp.X;
			py = sp.Y;
			pa = 0;

			timer = 0;
			endTimer = 3.0f;
			end = false;
		}

		public void Continue()
		{
			Loading();
			floor++;
			map = Resources.GetMap(floor, sMapWidth, sMapHeight);
			visionMap = new bool[sMapWidth * sMapHeight];

			enemies = new List<Enemy>();
			objects = new List<Object>();
			Vector2 sp = Resources.PopulateMap(floor, map, sMapWidth, sMapHeight, objects, enemies, vDifficulty);

			px = sp.X;
			py = sp.Y;
			pa = 0;
		}

		public void Loading()
		{
			for (var x = 0; x < sWidth; x++)
			{
				for (var y = 0; y < sHeight; y++)
				{
					Draw(x, y, ' ', ColorInt(BLACK, BLACK));
				}
			}
			string loading = "LOADING";
			for (var x = 0; x < loading.Length; x++)
			{
				int xOffset = sWidth / 2 - loading.Length / 2;
				int yOffset = sHeight / 2;
				Draw(x + xOffset, yOffset, loading[x], ColorInt(WHITE, BLACK));
			}
			DrawBuffer();
		}

		public override void Update(float deltaTime)
		{
			if (Console.WindowHeight > 5 && Console.WindowWidth > 5) { Console.SetCursorPosition(0, 0); }

			sceneStart = (lastSceneIndex != sceneIndex) ? true : false;
			lastSceneIndex = sceneIndex;

			switch (sceneIndex)
			{
				case 0:
					MainMenuScene();
					break;
				case 1:
					OptionsScene();
					break;
				case 2:
					CreditsScene();
					break;
				case 3:
					GameScene(deltaTime);
					break;
				case 4:
					PauseScene();
					break;
				case 5:
					DefeatScene();
					break;
				case 6:
					VictoryScene();
					break;
				default:
					MainMenuScene();
					break;
			}
		}

		public override void OnWindowChange()
		{
			int size = 14;
			if (sceneIndex == 3) { size = 8; }

			UpdateSize(new Vector2(Console.WindowWidth, Console.WindowHeight), size);
			depthBuffer = new float[sWidth * sHeight];
		}

		void MainMenuScene()
		{
			if (sceneStart)
			{
				menuIndex = 0;
				audioManager.menu = true;
				UpdateSize(new Vector2(80, 30), 14);
			}

			int topOffset = (int)sHeight / 10;
			int leftOffset = (int)((sWidth - Resources.titleWidth) / 2);

			for (var x = 0; x < sWidth; x++)
			{
				for (var y = 0; y < sHeight; y++)
				{
					Draw(x, y, ' ', ColorInt(BLACK, BLACK));
				}
			}

			for (var x = 0; x < Resources.titleWidth; x++)
			{
				for (var y = 0; y < Resources.titleHeight; y++)
				{
					short color = title[Index(x, y, Resources.titleWidth)].Attributes;
					char titleChar = title[Index(x, y, Resources.titleWidth)].Char.UnicodeChar;
					Draw(x + leftOffset, y + topOffset, titleChar, color);
				}
			}

			if (GetKeyCode((int)ConsoleKey.UpArrow) && menuIndex > 0 && !iUp)
			{
				iUp = true;
				menuIndex--;
				audioManager.PlaySound("Select.wav");
			}
			if (!GetKeyCode((int)ConsoleKey.UpArrow))
			{
				iUp = false;
			}
			if (GetKeyCode((int)ConsoleKey.DownArrow) && menuIndex < 3 && !iDown)
			{
				iDown = true;
				menuIndex++;
				audioManager.PlaySound("Select.wav");
			}
			if (!GetKeyCode((int)ConsoleKey.DownArrow))
			{
				iDown = false;
			}

			string[] options = new string[]{
				"START", "OPTIONS", "CREDITS", "QUIT"
			};

			for (var i = 0; i < options.Length; i++)
			{
				for (var x = 0; x < options[i].Length; x++)
				{
					int color = ColorInt(WHITE, BLACK);

					if (i == menuIndex)
					{
						color = ColorInt(BLACK, WHITE);
					}

					Draw(sWidth / 2 - options[i].Length / 2 + x, topOffset + Resources.titleHeight + (i * 3), options[i][x], color);
				}
			}

			if (GetKeyCode((int)ConsoleKey.Enter) && !iConfirm)
			{
				audioManager.PlaySound("Select.wav");

				switch (menuIndex)
				{
					case 0:
						Reset();
						sceneIndex = 3;
						break;
					case 1:
						sceneIndexBuffer = 0;
						sceneIndex = 1;
						break;
					case 2:
						sceneIndex = 2;
						break;
					case 3:
						Environment.Exit(0);
						break;
					default:
						break;
				}

				iConfirm = true;
			}

			if (!GetKeyCode((int)ConsoleKey.Enter))
			{
				iConfirm = false;
			}
		}

		void OptionsScene()
		{
			if (sceneStart)
			{
				audioManager.menu = true;
				menuIndex = 0;
				subMenuIndex = 0;
				mMasterVolume = false;
				UpdateSize(new Vector2(80, 30), 14);
			}

			int topOffset = (int)sHeight / 10;
			int leftOffset = (int)(sWidth / 10);

			for (var x = 0; x < sWidth; x++)
			{
				for (var y = 0; y < sHeight; y++)
				{
					Draw(x, y, ' ', ColorInt(BLACK, BLACK));
				}
			}

			for (var x = 0; x < Resources.titleWidth; x++)
			{
				for (var y = 0; y < Resources.titleHeight; y++)
				{
					short color = title[Index(x, y, Resources.titleWidth)].Attributes;
					char titleChar = title[Index(x, y, Resources.titleWidth)].Char.UnicodeChar;
					Draw(x + leftOffset, y + topOffset, titleChar, color);
				}
			}

			string optionTitle = "OPTIONS";
			for (var x = 0; x < optionTitle.Length; x++)
			{
				Draw(x + leftOffset + 1, topOffset + Resources.titleHeight, optionTitle[x], ColorInt(WHITE, BLACK));
			}
			string line = "--------------------";
			for (var x = 0; x < line.Length; x++)
			{
				Draw(x + leftOffset + 1, topOffset + Resources.titleHeight + 1, line[x], ColorInt(WHITE, BLACK));
			}

			string[] options = new string[]{
				"DISPLAY", "GAME", "AUDIO", "BACK"
			};

			string[][] categories = new string[][]{
				new string[]{"FOV", "RENDER DISTANCE"},
				new string[]{"DIFFICULTY", "REVEAL MAP", "GOD MODE"},
				new string[]{"MASTER", "MUSIC", "OTHER"}
			};
			if (mFov)
			{
				if (GetKeyCode((int)ConsoleKey.LeftArrow) && !iLeft)
					cFov -= 5;

				if (GetKeyCode((int)ConsoleKey.RightArrow) && !iRight)
					cFov += 5;


				if (cFov > 110) { cFov = 110; }
				if (cFov < 30) { cFov = 30; }

				string fov = "|" + cFov.ToString() + "|";
				for (var x = 0; x < fov.Length; x++)
				{
					Draw(x + leftOffset + 32, topOffset + Resources.titleHeight + 3, fov[x], ColorInt(BLACK, WHITE));
				}

				if (GetKeyCode((int)ConsoleKey.Enter) && !iConfirm)
				{
					mFov = false;
				}

				iLeft = true;
				iRight = true;
				iConfirm = true;
			}
			if (mMaxRenderDistance)
			{
				if (GetKeyCode((int)ConsoleKey.LeftArrow) && !iLeft)
					cMaxRenderDistance -= 1;

				if (GetKeyCode((int)ConsoleKey.RightArrow) && !iRight)
					cMaxRenderDistance += 1;


				if (cMaxRenderDistance > 32) { cMaxRenderDistance = 32; }
				if (cMaxRenderDistance < 4) { cMaxRenderDistance = 4; }

				string renderDistance = "|" + cMaxRenderDistance.ToString() + "|";
				for (var x = 0; x < renderDistance.Length; x++)
				{
					Draw(x + leftOffset + 32, topOffset + Resources.titleHeight + 5, renderDistance[x], ColorInt(BLACK, WHITE));
				}

				if (GetKeyCode((int)ConsoleKey.Enter) && !iConfirm)
				{
					mMaxRenderDistance = false;
				}

				iLeft = true;
				iRight = true;
				iConfirm = true;
			}
			if (mDifficulty)
			{
				if (GetKeyCode((int)ConsoleKey.LeftArrow) && !iLeft)
				{
					switch (_currentDifficulty)
					{
						case Difficulty.Easy:
							break;
						case Difficulty.Normal:
							_currentDifficulty = Difficulty.Easy;
							break;
						case Difficulty.Hard:
							_currentDifficulty = Difficulty.Normal;
							break;
						default:
							break;
					}
				}

				if (GetKeyCode((int)ConsoleKey.RightArrow) && !iRight)
				{
					switch (_currentDifficulty)
					{
						case Difficulty.Easy:
							_currentDifficulty = Difficulty.Normal;
							break;
						case Difficulty.Normal:
							_currentDifficulty = Difficulty.Hard;
							break;
						case Difficulty.Hard:
							break;
						default:
							break;
					}
				}

				string state = "";
				switch (_currentDifficulty)
				{
					case Difficulty.Easy:
						state = "EASY";
						break;
					case Difficulty.Normal:
						state = "NORMAL";
						break;
					case Difficulty.Hard:
						state = "HARD";
						break;
					default:
						break;
				}

				string sDifficulty = "|" + state + "|";
				for (var x = 0; x < sDifficulty.Length; x++)
				{
					Draw(x + leftOffset + 26, topOffset + Resources.titleHeight + 3, sDifficulty[x], ColorInt(BLACK, WHITE));
				}

				if (GetKeyCode((int)ConsoleKey.Enter) && !iConfirm)
				{
					currentDifficulty = _currentDifficulty;
					audioManager.PlaySound("Select.wav");
					mDifficulty = false;
				}

				iLeft = true;
				iRight = true;
				iConfirm = true;
			}
			if (mRevealMap)
			{
				if (GetKeyCode((int)ConsoleKey.LeftArrow) && !iLeft)
					revealMap = !revealMap;

				if (GetKeyCode((int)ConsoleKey.RightArrow) && !iRight)
					revealMap = !revealMap;

				string state = (revealMap) ? "ON" : "OFF";
				string sRevealMap = "|" + state + "|";
				for (var x = 0; x < sRevealMap.Length; x++)
				{
					Draw(x + leftOffset + 26, topOffset + Resources.titleHeight + 5, sRevealMap[x], ColorInt(BLACK, WHITE));
				}

				if (GetKeyCode((int)ConsoleKey.Enter) && !iConfirm)
				{
					audioManager.PlaySound("Select.wav");
					mRevealMap = false;
				}

				iLeft = true;
				iRight = true;
				iConfirm = true;
			}
			if (mGodmode)
			{
				if (GetKeyCode((int)ConsoleKey.LeftArrow) && !iLeft)
					godmode = !godmode;

				if (GetKeyCode((int)ConsoleKey.RightArrow) && !iRight)
					godmode = !godmode;

				string state = (godmode) ? "ON" : "OFF";
				string sGodmode = "|" + state + "|";
				for (var x = 0; x < sGodmode.Length; x++)
				{
					Draw(x + leftOffset + 26, topOffset + Resources.titleHeight + 7, sGodmode[x], ColorInt(BLACK, WHITE));
				}

				if (GetKeyCode((int)ConsoleKey.Enter) && !iConfirm)
				{
					audioManager.PlaySound("Select.wav");
					mGodmode = false;
				}

				iLeft = true;
				iRight = true;
				iConfirm = true;
			}
			if (mMasterVolume)
			{
				if (GetKeyCode((int)ConsoleKey.LeftArrow) && !iLeft)
					iMasterVolume -= 5;

				if (GetKeyCode((int)ConsoleKey.RightArrow) && !iRight)
					iMasterVolume += 5;


				if (iMasterVolume > 100) { iMasterVolume = 100; }
				if (iMasterVolume < 0) { iMasterVolume = 0; }

				string masterVolume = "|" + iMasterVolume.ToString() + "|";
				for (var x = 0; x < masterVolume.Length; x++)
				{
					Draw(x + leftOffset + 24, topOffset + Resources.titleHeight + 3, masterVolume[x], ColorInt(BLACK, WHITE));
				}

				if (GetKeyCode((int)ConsoleKey.Enter) && !iConfirm)
				{
					audioManager.PlaySound("Select.wav");
					audioManager._master = iMasterVolume;
					mMasterVolume = false;
				}

				audioManager.SetVolume();
				iLeft = true;
				iRight = true;
				iConfirm = true;
			}
			if (mMusicVolume)
			{
				if (GetKeyCode((int)ConsoleKey.LeftArrow) && !iLeft)
					iMusicVolume -= 5;

				if (GetKeyCode((int)ConsoleKey.RightArrow) && !iRight)
					iMusicVolume += 5;


				if (iMusicVolume > 100) { iMusicVolume = 100; }
				if (iMusicVolume < 0) { iMusicVolume = 0; }

				string musicVolume = "|" + iMusicVolume.ToString() + "|";
				for (var x = 0; x < musicVolume.Length; x++)
				{
					Draw(x + leftOffset + 24, topOffset + Resources.titleHeight + 5, musicVolume[x], ColorInt(BLACK, WHITE));
				}

				if (GetKeyCode((int)ConsoleKey.Enter) && !iConfirm)
				{
					audioManager.PlaySound("Select.wav");
					audioManager._music = iMusicVolume;
					mMusicVolume = false;
				}

				audioManager.SetVolume();
				iLeft = true;
				iRight = true;
				iConfirm = true;
			}
			if (mOtherVolume)
			{
				if (GetKeyCode((int)ConsoleKey.LeftArrow) && !iLeft)
					iOtherVolume -= 5;

				if (GetKeyCode((int)ConsoleKey.RightArrow) && !iRight)
					iOtherVolume += 5;

				if (iOtherVolume > 100) { iOtherVolume = 100; }
				if (iOtherVolume < 0) { iOtherVolume = 0; }

				string otherVolume = "|" + iOtherVolume.ToString() + "|";
				for (var x = 0; x < otherVolume.Length; x++)
				{
					Draw(x + leftOffset + 24, topOffset + Resources.titleHeight + 7, otherVolume[x], ColorInt(BLACK, WHITE));
				}

				if (GetKeyCode((int)ConsoleKey.Enter) && !iConfirm)
				{
					audioManager.PlaySound("Select.wav");
					audioManager._other = iOtherVolume;
					mOtherVolume = false;
				}

				audioManager.SetVolume();
				iLeft = true;
				iRight = true;
				iConfirm = true;
			}

			bool moveable = true;
			if (mFov ||
				mMaxRenderDistance ||
				mGodmode ||
				mRevealMap ||
				mDifficulty ||
				mMasterVolume ||
				mMusicVolume ||
				mOtherVolume)
			{
				moveable = false;
			}

			if (GetKeyCode((int)ConsoleKey.UpArrow) && !iUp && moveable)
			{
				if (!showSubMenu && menuIndex > 0)
				{
					menuIndex--;
					audioManager.PlaySound("Select.wav");
				}
				else
				{
					if (showSubMenu && subMenuIndex > 0)
					{
						subMenuIndex--;
						audioManager.PlaySound("Select.wav");
					}
				}
				iUp = true;
			}
			if (GetKeyCode((int)ConsoleKey.DownArrow) && !iDown && moveable)
			{
				if (!showSubMenu && menuIndex < options.Length - 1)
				{
					menuIndex++;
					audioManager.PlaySound("Select.wav");
				}
				else
				{
					if (showSubMenu && subMenuIndex < categories[menuIndex].Length - 1)
					{
						subMenuIndex++;
						audioManager.PlaySound("Select.wav");
					}
				}
				iDown = true;
			}

			for (var i = 0; i < options.Length; i++)
			{
				for (var x = 0; x < options[i].Length; x++)
				{
					int color = ColorInt(WHITE, BLACK);

					if (i == menuIndex)
					{
						color = ColorInt(BLACK, WHITE);
					}

					Draw(x + leftOffset + 1, topOffset + Resources.titleHeight + (i * 2) + 3, options[i][x], color);
				}
			}

			if (showSubMenu)
			{
				for (var i = 0; i < categories[menuIndex].Length; i++)
				{
					for (var x = 0; x < categories[menuIndex][i].Length; x++)
					{
						int color = ColorInt(WHITE, BLACK);

						if (i == subMenuIndex)
						{
							color = ColorInt(BLACK, WHITE);
						}

						Draw(x + leftOffset + 14, topOffset + Resources.titleHeight + (i * 2) + 3, categories[menuIndex][i][x], color);
					}
				}
			}

			if (GetKeyCode((int)ConsoleKey.Enter) && !iConfirm)
			{
				audioManager.PlaySound("Select.wav");

				switch (menuIndex)
				{
					case 0:
						if (showSubMenu)
						{
							switch (subMenuIndex)
							{
								case 0:
									mFov = true;
									break;
								case 1:
									mMaxRenderDistance = true;
									break;
								default:
									break;
							}
						}
						else
						{
							subMenuIndex = 0;
							showSubMenu = true;
						}
						break;
					case 1:
						if (showSubMenu)
						{
							switch (subMenuIndex)
							{
								case 0:
									_currentDifficulty = currentDifficulty;
									mDifficulty = true;
									break;
								case 1:
									mRevealMap = true;
									break;
								case 2:
									mGodmode = true;
									break;
								default:
									break;
							}
						}
						else
						{
							subMenuIndex = 0;
							showSubMenu = true;
						}
						break;
					case 2:
						if (showSubMenu)
						{
							switch (subMenuIndex)
							{
								case 0:
									mMasterVolume = true;
									break;
								case 1:
									mMusicVolume = true;
									break;
								case 2:
									mOtherVolume = true;
									break;
								default:
									break;
							}
						}
						else
						{
							subMenuIndex = 0;
							showSubMenu = true;
						}
						break;
					case 3:
						sceneIndex = sceneIndexBuffer;
						break;
					default:
						break;
				}

				iConfirm = true;
			}
			if (GetKeyCode((int)ConsoleKey.Escape) && !iBack)
			{
				iBack = true;
				if (showSubMenu) { showSubMenu = false; } else { sceneIndex = sceneIndexBuffer; }
				mDifficulty = false;
				mRevealMap = false;
				mGodmode = false;
				mMasterVolume = false;
				mMusicVolume = false;
				mOtherVolume = false;
				audioManager.PlaySound("Select.wav");
			}

			if (!GetKeyCode((int)ConsoleKey.Enter))
			{
				iConfirm = false;
			}
			if (!GetKeyCode((int)ConsoleKey.Escape))
			{
				iBack = false;
			}
			if (!GetKeyCode((int)ConsoleKey.UpArrow))
			{
				iUp = false;
			}
			if (!GetKeyCode((int)ConsoleKey.DownArrow))
			{
				iDown = false;
			}
			if (!GetKeyCode((int)ConsoleKey.LeftArrow))
			{
				iLeft = false;
			}
			if (!GetKeyCode((int)ConsoleKey.RightArrow))
			{
				iRight = false;
			}
		}

		void CreditsScene()
		{
			if (sceneStart)
			{
				audioManager.menu = true;
				menuIndex = 0;
				UpdateSize(new Vector2(80, 30), 14);
			}

			for (var x = 0; x < sWidth; x++)
			{
				for (var y = 0; y < sHeight; y++)
				{
					Draw(x, y, ' ', ColorInt(BLACK, BLACK));
				}
			}

			int topOffset = (int)sHeight / 10;
			int leftOffset = (int)((sWidth - Resources.titleWidth) / 2);

			for (var x = 0; x < Resources.titleWidth; x++)
			{
				for (var y = 0; y < Resources.titleHeight; y++)
				{
					short color = title[Index(x, y, Resources.titleWidth)].Attributes;
					char titleChar = title[Index(x, y, Resources.titleWidth)].Char.UnicodeChar;
					Draw(x + leftOffset, y + topOffset, titleChar, color);
				}
			}

			string[] lines = new string[]{
				"I666, a game made by Tage Akerstrom 2021.",
				"Made with The Asparagus Engine",
				"Made for the PRRPRR01 course | NTI Stockholm",
				"-- ESC TO GO BACK --",
			};

			for (var i = 0; i < lines.Length; i++)
			{
				for (var x = 0; x < lines[i].Length; x++)
				{
					int color = ColorInt(WHITE, BLACK);
					Draw(sWidth / 2 - lines[i].Length / 2 + x, topOffset + Resources.titleHeight + (i * 3), lines[i][x], color);
				}
			}

			if (GetKeyCode((int)ConsoleKey.Escape))
			{
				sceneIndex = 0;
				audioManager.PlaySound("Select.wav");
			}
		}

		void GameScene(float deltaTime)
		{
			if (sceneStart)
			{
				audioManager.menu = false;
				UpdateSize(new Vector2(320, 100), 8);
			}

			if (GetKeyCode((int)ConsoleKey.Escape) && !iBack)
			{
				iBack = true;
				sceneIndexBuffer = sceneIndex;
				sceneIndex = 4;
				return;
			}
			if (!GetKeyCode((int)ConsoleKey.Escape))
			{
				iBack = false;
			}

			vRevealMap = revealMap ? true : vRevealMap;
			vGodmode = godmode ? true : vGodmode;

			for (var i = 0; i < visionMap.Length; i++)
			{
				visionMap[i] = revealMap ? true : visionMap[i];
			}

			//Clear inputs
			iVertical = 0;
			iHorizintal = 0;
			iSprint = 0;
			iRot = 0;

			if (GetKey(ConsoleKey.Backspace))
				Reset();

			if (GetKeyCode((int)ConsoleKey.W))
				iVertical += 1;

			if (GetKeyCode((int)ConsoleKey.S))
				iVertical -= 1;

			if (GetKeyCode((int)ConsoleKey.A))
				iHorizintal -= 1;

			if (GetKeyCode((int)ConsoleKey.D))
				iHorizintal += 1;

			if (GetKeyCode((int)ConsoleKey.LeftArrow))
				iRot -= 1;

			if (GetKeyCode((int)ConsoleKey.RightArrow))
				iRot += 1;

			if (GetKeyCode(0x10))
				iSprint = 1;

			if (currentWeapon.hold) { iFire = false; }
			if (GetKeyCode((int)ConsoleKey.Spacebar) && !iFire && pAmmo > 0)
			{
				//Delta time
				shootTimer2 = stopWatch.Elapsed;
				float elapsedTime = (float)(shootTimer2.TotalSeconds - shootTimer1.TotalSeconds);

				if (elapsedTime > currentWeapon.cooldown)
				{
					shootTimer1 = shootTimer2;
					float fElapsedTime = elapsedTime;

					bool fired = Weapons.Fire(currentWeapon, px, py, pa, objects);
					if (fired)
					{
						if (currentWeapon.name == "Pistol")
							audioManager.PlaySound("Pistol.wav");
						if (currentWeapon.name == "Shotgun")
							audioManager.PlaySound("Sniper.wav");
						if (currentWeapon.name == "Machinegun")
							audioManager.PlaySound("Machinegun.wav");
						if (currentWeapon.name == "Sniper")
							audioManager.PlaySound("Sniper.wav");

						pAmmo--;
						hat = 0.2f;
					}

					iFire = true;
				}
			}
			if (!GetKeyCode((int)ConsoleKey.Spacebar))
			{
				iFire = false;
			}

			UpdateObjects(deltaTime);

			List<Object> remove = new List<Object>();
			List<Object> exits = new List<Object>();
			remove.Clear();
			exits.Clear();

			objects.Sort((x, y) => y.dist.CompareTo(x.dist));
			for (int i = objects.Count - 1; i >= 0; i--)
			{
				if (objects[i].dist > cMaxRenderDistance) { continue; }
				bool removeObject = false;

				if (objects[i].name == "bullet" || objects[i].name == "enemyBullet")
				{
					if (objects[i].timeAlvie > 4.0f)
					{
						removeObject = true;
					}
				}
				if (objects[i].name == "enemyBullet")
				{
					if (objects[i].dist < pPlayerRadius)
					{
						if (!godmode)
							pHealth -= 10;
						removeObject = true;
					}
				}
				if (objects[i].x > sMapWidth || objects[i].x < 0 || objects[i].y > sMapHeight || objects[i].y < 0)
				{
					removeObject = true;
				}
				try
				{
					if (map[Index((int)Math.Round(objects[i].x), (int)Math.Round(objects[i].y), sMapWidth)] != '.')
					{
						removeObject = true;
						audioManager.PlaySound("HitWall.wav");
					}
				}
				catch (System.Exception ex)
				{
					removeObject = true;
					Console.Write(ex.ToString());
				}
				if (objects[i].name == WorldObjects.Exit().name)
				{
					exits.Add(objects[i]);
				}

				float pickupDistance = objects[i].Distance(px, py);

				if (pickupDistance <= pPlayerRadius)
				{
					if (objects[i].name == WorldObjects.Pistol().name)
					{
						pAmmo += 10;
						removeObject = true;
						audioManager.PlaySound("Pickup.wav");
						currentWeapon = Weapons.pistol;
					}
					if (objects[i].name == WorldObjects.Shotgun().name)
					{
						pAmmo += 5;
						removeObject = true;
						audioManager.PlaySound("Pickup.wav");
						currentWeapon = Weapons.shotgun;
					}
					if (objects[i].name == WorldObjects.Machinegun().name)
					{
						pAmmo += 15;
						removeObject = true;
						audioManager.PlaySound("Pickup.wav");
						currentWeapon = Weapons.machinegun;
					}
					if (objects[i].name == WorldObjects.Sniper().name)
					{
						pAmmo += 3;
						removeObject = true;
						audioManager.PlaySound("Pickup.wav");
						currentWeapon = Weapons.sniper;
					}
					if (objects[i].name == WorldObjects.HealthPickup().name && pHealth < 100)
					{
						pHealth += 25;
						removeObject = true;
						audioManager.PlaySound("Pickup.wav");
					}
					if (objects[i].name == WorldObjects.AmmoPickup().name && pAmmo < 99)
					{
						pAmmo += 35;
						removeObject = true;
						audioManager.PlaySound("Pickup.wav");
					}
					if (objects[i].name == WorldObjects.Exit().name)
					{
						Continue();
						audioManager.PlaySound("Pickup.wav");
						return;
					}
				}

				if (removeObject)
				{
					objects.Remove(objects[i]);
				}
			}

			foreach (var Enemy in enemies)
			{
				if (Enemy.Update(deltaTime, objects, px, py, map, currentWeapon, audioManager))
				{
					end = true;
				}
			}
			if (end) { endTimer -= deltaTime; }
			if (endTimer <= 0) { sceneIndex = 6; }

			pa = pa + iRot * pRotSpeed * deltaTime;
			float speed = iSprint == 1 ? pWalkSpeed * pSprintMult : pWalkSpeed;

			float x1 = MathF.Sin((pa + 90) * (pi / 180)) * speed;
			float y1 = MathF.Cos((pa + 90) * (pi / 180)) * speed;

			float x2 = MathF.Sin((pa) * (pi / 180)) * -speed;
			float y2 = MathF.Cos((pa) * (pi / 180)) * -speed;

			float vx = (x1 * iVertical) + (x2 * iHorizintal);
			float vy = (y1 * iVertical) + (y2 * iHorizintal);

			px = px + (vx * deltaTime);
			py = py + (vy * deltaTime);

			mx = (int)Math.Round(px);
			my = (int)Math.Round(py);

			for (var x = mx - 1; x < mx + 2; x++)
			{
				for (var y = my - 1; y < my + 2; y++)
				{
					if (map[Index(x, y, sMapWidth)] != '.')
					{
						float idx = MathF.Abs(px - x);
						float idy = MathF.Abs(py - y);

						if (idx < 0.5f + pPlayerRadius && idy < 0.5f + pPlayerRadius)
						{
							if (idx > idy)
							{
								if (px >= x) { px += 0.5f - idx + pPlayerRadius; }
								if (px < x) { px -= 0.5f - idx + pPlayerRadius; }
							}
							else
							{
								if (py >= y) { py += 0.5f - idy + pPlayerRadius; }
								if (py < y) { py -= 0.5f - idy + pPlayerRadius; }
							}
						}
					}
				}
			}

			//Draw ceiling and floor
			for (int x = 0; x < sWidth; x++)
			{
				for (int y = 0; y < sHeight; y++)
				{
					int groundColor = ColorInt(BLACK, DARKGRAY);

					if (y > sHeight / 2)
					{
						groundColor = ColorInt(BLACK, GRAY);
					}

					Draw(x, y, ' ', groundColor);
				}
			}

			//Multithread ray casting
			/*
			List<Thread> threads = new List<Thread>();
			int threadCount = 8;
			for (var i = 0; i < threadCount; i++)
			{
				int start = (i) * (sWidth / threadCount);
				int stop = (i + 1) * (sWidth / threadCount);

				threads.Add(new Thread(delegate ()
				{
					CastRays(start, stop);
				}));
				threads[i].Start();
			}

			foreach (var Thread in threads)
			{
				Thread.Join();
			}
			*/

			CastRays(0, sWidth);

			//Update HUD
			for (int y = 0; y < sMapHeight; y++)
				for (int x = 0; x < sMapWidth; x++)
				{
					char mi = map[Index(x, y, sMapWidth)];

					for (var i = 0; i < exits.Count; i++)
					{
						int ex = (int)Math.Round(exits[i].x);
						int ey = (int)Math.Round(exits[i].y);

						if (x == ex && y == ey)
							mi = 'e';
					}

					if (x == mx && y == my)
					{
						mi = 'p';
					}

					bool edge = false;
					edge = (y == 0 || y == sMapHeight - 1) ? true : edge;
					edge = (x == 0 || x == sMapWidth - 1) ? true : edge;

					int mapColor;
					if (visionMap[Index(x, y, sMapWidth)])
					{
						mapColor = ColorInt(BLACK, BLACK);
					}
					else
					{
						mapColor = ColorInt(BLACK, DARKGRAY);
					}

					if (edge)
						mapColor = ColorInt(BLACK, GRAY);
					if (mi != '.' && visionMap[Index(x, y, sMapWidth)])
						mapColor = ColorInt(BLACK, WHITE);
					if (mi == 'p')
						mapColor = ColorInt(BLACK, BLUE);
					if (mi == 'e')
						mapColor = ColorInt(BLACK, GREEN);

					Draw((2 * (sMapWidth - x)) + 3, y + 3, ' ', mapColor);
					Draw((2 * (sMapWidth - x)) + 1 + 3, y + 3, ' ', mapColor);
				}

			for (var x = 0; x < currentWeapon.hudSprite.width; x++)
			{
				for (var y = 0; y < currentWeapon.hudSprite.height; y++)
				{
					int xOffset = sWidth - (currentWeapon.hudSprite.width * 2) - 5;
					int yOffset = 5;

					int color = currentWeapon.hudSprite.sprite[Index(x, y, currentWeapon.hudSprite.width)].Attributes;

					for (var i = 0; i < 2; i++)
					{
						Draw(2 * x + i + xOffset, y + yOffset, ' ', color);
					}
				}
			}
			for (var x = 0; x < currentWeapon.handSprite.width / 2; x++)
			{
				for (var y = 0; y < currentWeapon.handSprite.height; y++)
				{
					Sprite sprite = currentWeapon.handSprite;
					int xOffset = (sWidth / 2) - (sprite.width / 2);
					int yOffset = sHeight - sprite.height;
					int xIndex = x;
					if (hat > 0.0f)
					{
						int frameOffset = (int)(sprite.width / 2);
						xIndex = frameOffset + x;
					}

					int color = sprite.sprite[Index(xIndex, y, sprite.width)].Attributes;

					Draw(xOffset + (2 * x), yOffset + y, ' ', color);
					Draw(xOffset + (2 * x + 1), yOffset + y, ' ', color);
				}
			}
			hat = hat > 0 ? hat - deltaTime : 0;

			if (pAmmo > 99) { pAmmo = 99; }
			if (pAmmo < 0) { pAmmo = 0; }

			string ammoNumberString = pAmmo.ToString();

			int ammoD1;
			int ammoD2;

			if (ammoNumberString.Length < 2)
			{
				ammoD1 = 0;
				ammoD2 = (int)Char.GetNumericValue(ammoNumberString[0]);
			}
			else
			{
				ammoD1 = (int)Char.GetNumericValue(ammoNumberString[0]);
				ammoD2 = (int)Char.GetNumericValue(ammoNumberString[1]);
			}

			Pixel[] ammoCounter1 = Counter.Counter1(ammoD1, new Color(RED, WHITE));
			Pixel[] ammoCounter2 = Counter.Counter1(ammoD2, new Color(RED, WHITE));

			if (pHealth > 100) { pHealth = 100; }
			if (pHealth < 0) { pHealth = 0; }
			if (pHealth == 0) { sceneIndex = 5; }

			string healthNumberString = pHealth.ToString();

			int healthD1;
			int healthD2;
			int healthD3;

			switch (healthNumberString.Length)
			{
				case 1:
					healthD1 = 0;
					healthD2 = 0;
					healthD3 = (int)Char.GetNumericValue(healthNumberString[0]);
					break;
				case 2:
					healthD1 = 0;
					healthD2 = (int)Char.GetNumericValue(healthNumberString[0]);
					healthD3 = (int)Char.GetNumericValue(healthNumberString[1]);
					break;
				case 3:
					healthD1 = (int)Char.GetNumericValue(healthNumberString[0]);
					healthD2 = (int)Char.GetNumericValue(healthNumberString[1]);
					healthD3 = (int)Char.GetNumericValue(healthNumberString[2]);
					break;
				default:
					healthD1 = 0;
					healthD2 = 0;
					healthD3 = 0;
					break;
			}

			Pixel[] healthCounter1 = Counter.Counter1(healthD1, new Color(RED, RED));
			Pixel[] healthCounter2 = Counter.Counter1(healthD2, new Color(RED, RED));
			Pixel[] healthCounter3 = Counter.Counter1(healthD3, new Color(RED, RED));

			for (var x = 0; x < 3; x++)
			{
				for (var y = 0; y < 5; y++)
				{
					int xOffset1 = sWidth - 20 + (2 * x);
					int xOffset2 = sWidth - 20 + (2 * x) + 1;

					Draw(xOffset1 - 8, y + 20, ' ', ammoCounter1[Index(x, y, 3)].Attributes);
					Draw(xOffset2 - 8, y + 20, ' ', ammoCounter1[Index(x, y, 3)].Attributes);
					Draw(xOffset1, y + 20, ' ', ammoCounter2[Index(x, y, 3)].Attributes);
					Draw(xOffset2, y + 20, ' ', ammoCounter2[Index(x, y, 3)].Attributes);

					Draw(xOffset1 - 16, y + 28, ' ', healthCounter1[Index(x, y, 3)].Attributes);
					Draw(xOffset2 - 16, y + 28, ' ', healthCounter1[Index(x, y, 3)].Attributes);
					Draw(xOffset1 - 8, y + 28, ' ', healthCounter2[Index(x, y, 3)].Attributes);
					Draw(xOffset2 - 8, y + 28, ' ', healthCounter2[Index(x, y, 3)].Attributes);
					Draw(xOffset1, y + 28, ' ', healthCounter3[Index(x, y, 3)].Attributes);
					Draw(xOffset2, y + 28, ' ', healthCounter3[Index(x, y, 3)].Attributes);
				}
			}

			for (var x = 0; x < ammoIcon.width; x++)
			{
				for (var y = 0; y < ammoIcon.height; y++)
				{
					Pixel pixel = ammoIcon.sprite[Index(x, y, ammoIcon.width)];
					Draw(sWidth - 12 + (2 * x), y + 20, ' ', pixel.Attributes);
					Draw(sWidth - 12 + (2 * x) + 1, y + 20, ' ', pixel.Attributes);
				}
			}

			for (var x = 0; x < healthIcon.width; x++)
			{
				for (var y = 0; y < healthIcon.height; y++)
				{
					Pixel pixel = healthIcon.sprite[Index(x, y, healthIcon.width)];
					Draw(sWidth - 12 + (2 * x), y + 28, ' ', pixel.Attributes);
					Draw(sWidth - 12 + (2 * x) + 1, y + 28, ' ', pixel.Attributes);
				}
			}

			//Timer for score
			timer += deltaTime * 1000;

			//Draw crosshair
			Draw(sWidth / 2 + 1, sHeight / 2, ' ', ColorInt(BLACK, BLACK));
			Draw(sWidth / 2 + 2, sHeight / 2, ' ', ColorInt(BLACK, BLACK));
			Draw(sWidth / 2, sHeight / 2 + 1, ' ', ColorInt(BLACK, BLACK));
			Draw(sWidth / 2, sHeight / 2, ' ', ColorInt(BLACK, WHITE));
			Draw(sWidth / 2 - 1, sHeight / 2, ' ', ColorInt(BLACK, BLACK));
			Draw(sWidth / 2 - 2, sHeight / 2, ' ', ColorInt(BLACK, BLACK));
			Draw(sWidth / 2, sHeight / 2 - 1, ' ', ColorInt(BLACK, BLACK));

			debugParams = (new[] {
				new Debug("FPS", 1.0f/deltaTime, 0, false),
				new Debug("DELTA", deltaTime, 0, false),
				new Debug("POS", px, py, true),
				new Debug("ANGLE", pa, 0, false),
				new Debug("MAPPOS", mx, my, true)
			});
		}

		void CastRays(int start, int stop)
		{
			for (var x = start; x < stop; x++)
			{
				float startAngle = 90 + pa - cFov / 2;
				float stepAngle = (cFov / sWidth);
				float rayAngle = Overflowf(startAngle + x * stepAngle, 0, 360);

				float rx = px;
				float ry = py;

				float sdx = MathF.Sin(rayAngle * pi / 180) * cStepSize;
				float sdy = MathF.Cos(rayAngle * pi / 180) * cStepSize;

				float distance = 0;
				char hitchar = '.';

				bool hit = false;
				while (!hit)
				{
					if (distance >= cMaxRenderDistance)
					{
						hit = true;
						break;
					}

					int rmpx = (int)Math.Round(rx);
					int rmpy = (int)Math.Round(ry);
					visionMap[Index(rmpx, rmpy, sMapWidth)] = true;

					if (map[Index(rmpx, rmpy, sMapWidth)] != '.')
					{
						hitchar = map[Index(rmpx, rmpy, sMapWidth)];
						hit = true;
						break;
					}

					rx += sdx;
					ry += sdy;
					distance += cStepSize;
				}

				depthBuffer[x] = distance;
				float va = Overflowf(pa, 0, 360);

				float dx = distance * MathF.Cos(va * (pi / 180));
				float dy = distance * MathF.Sin(va * (pi / 180));

				float nDist = dx * MathF.Cos(va * pi / 180) + dy * MathF.Sin(va * pi / 180);
				float height = Even((int)Math.Round(cDepth / nDist));

				int pixel = ' ';
				int color = ColorInt(BLACK, BLACK);

				//Draw height
				for (int y = 0; y < height; y++)
				{
					int center = sHeight / 2;
					int top = (int)Math.Round(center - height / 2);

					if (hitchar != '.')
					{
						Sprite sprite = wall0;

						if (hitchar == '#')
							sprite = wall0;
						if (hitchar == 'x')
							sprite = wall1;
						if (hitchar == 'A')
							sprite = wall2;
						if (hitchar == 'B')
							sprite = wall3;

						float nxx = ((int)Math.Round(rx) - 0.5f) - rx;
						float nxy = ((int)Math.Round(ry) - 0.5f) - ry;
						float ny = Mapf(y, 0, height, 0, 1);

						Pixel sPixel = sprite.Sample(nxx + nxy, ny);

						pixel = sPixel.Char.UnicodeChar;
						color = sPixel.Attributes;
					}

					Draw(x, top + y, (char)pixel, color);
				}

				//Draw objects
				foreach (var Object in objects)
				{
					float dist = Object.Distance(px, py);
					if (dist > cMaxRenderDistance) { continue; }
					if (dist < pPlayerRadius) { continue; }

					float ptoa;
					float oa;
					float ra;

					if (px < Object.x)
					{
						ra = Overflowf(MathF.Atan2(-sdy, -sdx) * (180 / pi), 0, 360);
						ptoa = Overflowf(MathF.Atan2(py - Object.y, px - Object.x) * (180 / pi), 0, 360);
						oa = Overflowf(Object.width / dist, 0, 360);
					}
					else
					{
						ra = MathF.Atan2(-sdy, -sdx) * (180 / pi);
						ptoa = MathF.Atan2(py - Object.y, px - Object.x) * (180 / pi);
						oa = Object.width / dist;
					}

					int objectHeight = (int)Math.Round(Object.height / dist);
					int oDist = (int)Math.Round(cDepth / dist);

					if (ptoa > ra - oa && ptoa < ra + oa)
					{
						for (var oy = 0; oy < objectHeight; oy++)
						{
							if (dist > depthBuffer[x]) { continue; }

							float sx = (float)Mapf(ptoa, ra - oa, ra + oa, 0, 1);
							float sy = (float)Mapf(oy, 0, objectHeight, 0, 1);

							Pixel spritePixel = Object.sprite.Sample(sx, sy);

							int ceiling = (sHeight / 2) - (oDist / 2);
							int ctt = (oDist - objectHeight);
							int yp = ceiling + ctt + oy - (int)(oDist * Object.z);
							Draw(x, yp, spritePixel.Char.UnicodeChar, spritePixel.Attributes);
						}
					}
				}
			}
		}

		void PauseScene()
		{
			if (sceneStart)
			{
				audioManager.menu = true;
				menuIndex = 0;
				UpdateSize(new Vector2(80, 30), 14);
			}

			int topOffset = (int)sHeight / 10;
			int leftOffset = (int)((sWidth - Resources.titleWidth) / 2);

			for (var x = 0; x < sWidth; x++)
			{
				for (var y = 0; y < sHeight; y++)
				{
					Draw(x, y, ' ', ColorInt(BLACK, BLACK));
				}
			}

			for (var x = 0; x < Resources.titleWidth; x++)
			{
				for (var y = 0; y < Resources.titleHeight; y++)
				{
					short color = title[Index(x, y, Resources.titleWidth)].Attributes;
					char titleChar = title[Index(x, y, Resources.titleWidth)].Char.UnicodeChar;
					Draw(x + leftOffset, y + topOffset, titleChar, color);
				}
			}

			if (GetKeyCode((int)ConsoleKey.UpArrow) && menuIndex > 0 && !iUp)
			{
				iUp = true;
				audioManager.PlaySound("Select.wav");
				menuIndex--;
			}
			if (!GetKeyCode((int)ConsoleKey.UpArrow))
			{
				iUp = false;
			}
			if (GetKeyCode((int)ConsoleKey.DownArrow) && menuIndex < 2 && !iDown)
			{
				iDown = true;
				audioManager.PlaySound("Select.wav");
				menuIndex++;
			}
			if (!GetKeyCode((int)ConsoleKey.DownArrow))
			{
				iDown = false;
			}

			string[] options = new string[]{
				"RESUME", "OPTIONS", "MAIN MENU"
			};

			for (var i = 0; i < options.Length; i++)
			{
				for (var x = 0; x < options[i].Length; x++)
				{
					int color = ColorInt(WHITE, BLACK);

					if (i == menuIndex)
					{
						color = ColorInt(BLACK, WHITE);
					}

					Draw(sWidth / 2 - options[i].Length / 2 + x, topOffset + Resources.titleHeight + (i * 3), options[i][x], color);
				}
			}

			if (GetKeyCode((int)ConsoleKey.Enter) && !iConfirm)
			{
				audioManager.PlaySound("Select.wav");

				switch (menuIndex)
				{
					case 0:
						sceneIndex = 3;
						break;
					case 1:
						sceneIndexBuffer = 4;
						sceneIndex = 1;
						break;
					case 2:
						sceneIndex = 0;
						break;
					default:
						break;
				}

				iConfirm = true;
			}

			if (GetKeyCode((int)ConsoleKey.Escape) && !iBack)
			{
				iBack = true;
				if (showSubMenu) { showSubMenu = false; } else { sceneIndex = 3; }
				mDifficulty = false;
				mRevealMap = false;
				mGodmode = false;
				mMasterVolume = false;
				mMusicVolume = false;
				mOtherVolume = false;
				audioManager.PlaySound("Select.wav");
			}
			if (!GetKeyCode((int)ConsoleKey.Enter))
			{
				iConfirm = false;
			}
			if (!GetKeyCode((int)ConsoleKey.Escape))
			{
				iBack = false;
			}
		}

		void DefeatScene()
		{
			if (sceneStart)
			{
				audioManager.menu = true;
				menuIndex = 0;
				UpdateSize(new Vector2(80, 30), 14);
			}

			int topOffset = (int)sHeight / 10;
			int leftOffset = (int)((sWidth - Resources.titleWidth) / 2);

			for (var x = 0; x < sWidth; x++)
			{
				for (var y = 0; y < sHeight; y++)
				{
					Draw(x, y, ' ', ColorInt(BLACK, BLACK));
				}
			}

			for (var x = 0; x < Resources.titleWidth; x++)
			{
				for (var y = 0; y < Resources.titleHeight; y++)
				{
					short color = title[Index(x, y, Resources.titleWidth)].Attributes;
					char titleChar = title[Index(x, y, Resources.titleWidth)].Char.UnicodeChar;
					Draw(x + leftOffset, y + topOffset, titleChar, color);
				}
			}

			if (GetKeyCode((int)ConsoleKey.UpArrow) && menuIndex > 0 && !iUp)
			{
				iUp = true;
				menuIndex--;
			}
			if (!GetKeyCode((int)ConsoleKey.UpArrow))
			{
				iUp = false;
			}
			if (GetKeyCode((int)ConsoleKey.DownArrow) && menuIndex < 2 && !iDown)
			{
				iDown = true;
				menuIndex++;
			}
			if (!GetKeyCode((int)ConsoleKey.DownArrow))
			{
				iDown = false;
			}

			string message = "YOU DIED!";
			for (var x = 0; x < message.Length; x++)
			{
				Draw(x + (sWidth / 2) - (message.Length / 2), topOffset + Resources.titleHeight, message[x], ColorInt(RED, BLACK));
			}

			string[] options = new string[]{
				"RETRY", "OPTIONS", "MAIN MENU"
			};

			for (var i = 0; i < options.Length; i++)
			{
				for (var x = 0; x < options[i].Length; x++)
				{
					int color = ColorInt(WHITE, BLACK);

					if (i == menuIndex)
					{
						color = ColorInt(BLACK, WHITE);
					}

					Draw(sWidth / 2 - options[i].Length / 2 + x, topOffset + Resources.titleHeight + (i * 3) + 3, options[i][x], color);
				}
			}

			if (GetKeyCode((int)ConsoleKey.Enter) && !iConfirm)
			{
				switch (menuIndex)
				{
					case 0:
						Reset();
						sceneIndex = 3;
						break;
					case 1:
						sceneIndexBuffer = 5;
						sceneIndex = 1;
						break;
					case 2:
						sceneIndex = 0;
						break;
					default:
						break;
				}

				iConfirm = true;
			}

			if (!GetKeyCode((int)ConsoleKey.Enter))
			{
				iConfirm = false;
			}
		}

		void VictoryScene()
		{
			if (sceneStart)
			{
				audioManager.menu = true;
				menuIndex = 0;
				UpdateSize(new Vector2(80, 30), 14);
			}

			for (var x = 0; x < sWidth; x++)
			{
				for (var y = 0; y < sHeight; y++)
				{
					Draw(x, y, ' ', ColorInt(BLACK, BLACK));
				}
			}

			int topOffset = (int)sHeight / 10;
			int leftOffset = (int)((sWidth - Resources.titleWidth) / 2);

			for (var x = 0; x < Resources.titleWidth; x++)
			{
				for (var y = 0; y < Resources.titleHeight; y++)
				{
					char titleChar = title[Index(x, y, Resources.titleWidth)].Char.UnicodeChar;
					Draw(x + leftOffset, y + topOffset, titleChar, ColorInt(GREEN, BLACK));
				}
			}

			string gametime = "";
			if (!(timer < 0 || timer > int.MaxValue))
			{
				TimeSpan t = TimeSpan.FromMilliseconds(timer);
				if (t.TotalHours >= 1)
				{
					gametime = string.Format("{0:D1}h:{1:D1}m:{2:D2}s:{3:D3}ms",
										t.Hours,
										t.Minutes,
										t.Seconds,
										t.Milliseconds);
				}
				else
				{
					gametime = string.Format("{0:D1}m:{1:D2}s:{2:D3}ms",
										t.Minutes,
										t.Seconds,
										t.Milliseconds);
				}
			}
			else
			{
				gametime = "Invalid game time";
			}

			string[] lines = new string[]{
				"Well done! You won.",
				"Your time: "+gametime,
				"Difficulty: "+vDifficulty.ToString(),
				"Godmode: "+(vGodmode?"ON":"OFF") + ", Reveal map: "+(vRevealMap?"ON":"OFF"),
				"Thank you for playing my game",
				"-- ESC TO GO BACK --",
			};

			for (var i = 0; i < lines.Length; i++)
			{
				for (var x = 0; x < lines[i].Length; x++)
				{
					int color = ColorInt(GREEN, BLACK);
					Draw(sWidth / 2 - lines[i].Length / 2 + x, topOffset + Resources.titleHeight + (i * 3), lines[i][x], color);
				}
			}

			if (GetKeyCode((int)ConsoleKey.Escape)) { sceneIndex = 0; }
		}
	}

	class Program
	{
		static void Main()
		{
			Console.Title = "I666";
			I666 game = new I666();
			asparagus.Vector2 consoleSize = new Vector2(500, 150);
			game.Init(consoleSize, 5);
			game.Start();
			game.Stop();
		}
	}
}
