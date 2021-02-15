using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace asparagus
{
	public class Game
	{
		private Pixel[] screen;

		private ScreenSize screenSize;
		private SafeFileHandle handle;
		private Vector2 consoleSize;

		public int sWidth, sHeight;
		public int cWidth, cHeight;
		public bool initialized;

		public const ConsoleColor BLACK = ConsoleColor.Black;
		public const ConsoleColor BLUE = ConsoleColor.Blue;
		public const ConsoleColor CYAN = ConsoleColor.Cyan;
		public const ConsoleColor DARKBLUE = ConsoleColor.DarkBlue;
		public const ConsoleColor DARKCYAN = ConsoleColor.DarkCyan;
		public const ConsoleColor DARKGRAY = ConsoleColor.DarkGray;
		public const ConsoleColor DARKGREEN = ConsoleColor.DarkGreen;
		public const ConsoleColor DARKRED = ConsoleColor.DarkRed;
		public const ConsoleColor DARKYELLOW = ConsoleColor.DarkYellow;
		public const ConsoleColor GRAY = ConsoleColor.Gray;
		public const ConsoleColor GREEN = ConsoleColor.Green;
		public const ConsoleColor MAGENTA = ConsoleColor.Magenta;
		public const ConsoleColor RED = ConsoleColor.Red;
		public const ConsoleColor WHITE = ConsoleColor.White;
		public const ConsoleColor YELLOW = ConsoleColor.Yellow;
		public const ConsoleColor TRANSPARENT = ConsoleColor.DarkMagenta;

		public Debug[] debugParams;
		public List<Object> objects;

		public Game()
		{
			Console.WriteLine("---------- USING THE ASPARAGUS ENGINE V2.0 ----------");
			initialized = false;
		}

		~Game()
		{
			Console.WriteLine("---------- STOPPING THE ASPARAGUS ENGINE V2.0 ----------");
			initialized = false;
		}

		public void Init(Vector2 _consoleSize, int fontSize)
		{
			SetFontSize(fontSize);
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			//Save console settings
			consoleSize = _consoleSize;

			//Apply scale for init
			Console.SetWindowSize(consoleSize.X, consoleSize.Y);
			sWidth = consoleSize.X;
			sHeight = consoleSize.Y;

			objects = new List<Object>();

			//Setup screen buffer
			screen = new Pixel[sWidth * sHeight];

			screenSize = new ScreenSize() { Left = 0, Top = 0, Right = (ushort)sWidth, Bottom = (ushort)sHeight };
			handle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

			initialized = true;
			return;
		}

		public void Start()
		{
			Setup();
			if (!initialized) { return; }

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			TimeSpan tp1 = stopWatch.Elapsed;
			TimeSpan tp2 = stopWatch.Elapsed;

			if (handle.IsInvalid) { return; }

			bool bRunning = true;
			while (bRunning)
			{
				//Delta time
				tp2 = stopWatch.Elapsed;
				float elapsedTime = (float)(tp2.TotalSeconds - tp1.TotalSeconds);
				tp1 = tp2;
				float fElapsedTime = elapsedTime;

				//Reset debug params
				debugParams = null;

				//Callback override
				Update(fElapsedTime);

				if (Console.WindowHeight != consoleSize.Y || Console.WindowWidth != consoleSize.X)
				{
					OnWindowChange();
				}

				DrawDebug();

				DrawBuffer();
			}

			stopWatch.Stop();
		}

		public void DrawBuffer()
		{
			//Display frame
			WriteConsoleOutput(handle, screen,
				new Coord() { x = (ushort)sWidth, y = (ushort)sHeight },
				new Coord() { x = 0, y = 0 },
				ref screenSize);
		}

		public void Stop()
		{
			Console.WriteLine("Closing asparagus");
			return;
		}

		public virtual void Setup() { }
		public virtual void Update(float deltaTime) { }
		public virtual void OnWindowChange() { }

		public bool GetKey(ConsoleKey key)
		{
			short s = GetAsyncKeyState((int)key);
			return (s & 0x8000) > 0; //&& ConsoleFocused();
		}
		public bool GetKeyCode(Int32 key)
		{
			short s = GetAsyncKeyState((int)key);
			return (s & 0x8000) > 0; //&& ConsoleFocused();
		}

		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern SafeFileHandle CreateFile(
			string fileName,
			[MarshalAs(UnmanagedType.U4)] uint fileAccess,
			[MarshalAs(UnmanagedType.U4)] uint fileShare,
			IntPtr securityAttributes,
			[MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
			[MarshalAs(UnmanagedType.U4)] int flags,
			IntPtr template);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool WriteConsoleOutput(
			SafeFileHandle hConsoleOutput,
			Pixel[] lpBuffer,
			Coord dwBufferSize,
			Coord dwBufferCoord,
			ref ScreenSize lpWriteRegion);

		[StructLayout(LayoutKind.Sequential)]
		public struct Coord
		{
			public ushort x;
			public ushort y;

			public Coord(ushort _x, ushort _y)
			{
				x = _x;
				y = _y;
			}
		};

		[StructLayout(LayoutKind.Explicit)]
		public struct CharUnion
		{
			[FieldOffset(0)] public char UnicodeChar;
			[FieldOffset(0)] public byte AsciiChar;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct Pixel
		{
			[FieldOffset(0)] public CharUnion Char;
			[FieldOffset(2)] public short Attributes;
			[FieldOffset(0)] public int X;
			[FieldOffset(0)] public int Y;

			public Pixel(CharUnion _Char, short _Attributes, int _x, int _y)
			{
				Char = _Char;
				Attributes = _Attributes;
				X = _x;
				Y = _y;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ScreenSize
		{
			public ushort Left;
			public ushort Top;
			public ushort Right;
			public ushort Bottom;
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern short GetAsyncKeyState(Int32 vKey);

		public void Draw(int x, int y, char pixel, int color)
		{
			if (y < 0 || y > sHeight - 1) { return; }
			if (x < 0 || x > sWidth - 1) { return; }

			if (color == ColorInt(TRANSPARENT, TRANSPARENT)) { return; }

			//byte pixelByte = (byte)pixel;
			screen[y * sWidth + x].Char.UnicodeChar = pixel;
			screen[y * sWidth + x].Attributes = (short)color;
		}
		public void DrawPixel(Pixel pixel)
		{
			//byte pixelByte = (byte)pixel;
			screen[pixel.Y * sWidth + pixel.X].Char.UnicodeChar = pixel.Char.UnicodeChar;
			screen[pixel.Y * sWidth + pixel.X].Attributes = pixel.Attributes;
		}
		public void DrawHud(int x, int y, char pixel, int color)
		{
			//byte pixelByte = (byte)pixel;
			screen[y * sWidth + x].Char.UnicodeChar = pixel;
			screen[y * sWidth + x].Attributes = (short)color;
		}
		public void SetScreen(Pixel[] _screen)
		{
			screen = _screen;
		}
		public void SetFontSize(int size)
		{
			ConsoleHelper.SetCurrentFont("Consolas", (short)size);
		}
		public void UpdateSize(Vector2 _consoleSize, int fontSize)
		{
			SetFontSize(fontSize);
			if (_consoleSize.X <= 0 || _consoleSize.Y <= 0) { return; }
			if (_consoleSize.X > Console.LargestWindowWidth || _consoleSize.Y > Console.LargestWindowHeight) { return; }

			consoleSize = _consoleSize;
			sWidth = consoleSize.X;
			sHeight = consoleSize.Y;
			screen = new Pixel[sWidth * sHeight];
			screenSize = new ScreenSize() { Left = 0, Top = 0, Right = (ushort)sWidth, Bottom = (ushort)sHeight };
			handle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

			try
			{
				Console.SetWindowSize(consoleSize.X, consoleSize.Y);
			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			return;
		}

		public static int ColorInt(ConsoleColor foreground, ConsoleColor background)
		{
			int color = (int)foreground + ((int)background * 16);
			return color;
		}

		public struct Color
		{
			public ConsoleColor foreground { get; }
			public ConsoleColor background { get; }

			public Color(ConsoleColor _foreground, ConsoleColor _background)
			{
				foreground = _foreground;
				background = _background;
			}
		}

		public class Object
		{
			public float x;
			public float y;
			public float z;
			public int width;
			public int height;
			public float vx;
			public float vy;
			public string name;

			public float dist;

			public Sprite sprite;

			public float timeAlvie;

			public Object(float _x, float _y, float _z, float _vx, float _vy, int _width, int _height, Sprite _sprite, string _name)
			{
				x = _x;
				y = _y;
				z = _z;
				vx = _vx;
				vy = _vy;
				width = _width;
				height = _height;
				sprite = _sprite;
				name = _name;

				timeAlvie = 0.0f;
			}

			public float Distance(float _x, float _y)
			{
				float dx = x - _x;
				float dy = y - _y;
				dist = MathF.Sqrt(dx * dx + dy * dy);
				return dist;
			}

			public void Update(float deltaTime)
			{
				x += vx * deltaTime;
				y += vy * deltaTime;
				timeAlvie += deltaTime;
			}
		}

		public void UpdateObjects(float deltaTIme)
		{
			foreach (var Object in objects)
			{
				Object.Update(deltaTIme);
			}
		}

		public void DrawDebug()
		{
			//Return if no params are defined
			if (debugParams == null) { return; }

			string line = "";
			for (var i = 0; i < debugParams.Length; i++)
			{
				Debug cParam = debugParams[i];
				if (debugParams[i].DrawVector2)
				{
					line += cParam.Title + "=(" + cParam.Value1.ToString("0.00") + ", " + cParam.Value2.ToString("0.00") + ") ";
				}
				else
				{
					line += cParam.Title + "=" + cParam.Value1.ToString("0.00") + " ";
				}
			}
			for (var x = 0; x < sWidth; x++)
			{
				Draw(x, 0, ' ', ColorInt(WHITE, BLACK));
			}
			for (var x = 0; x < line.Length; x++)
			{
				Draw(x, 0, line[x], ColorInt(WHITE, BLACK));
			}
			return;
		}

		public static float Overflowf(float value, float bottom, float top)
		{
			float number = value;

			while (number < bottom)
			{
				number += top;
			}
			while (number > top)
			{
				number -= top;
			}

			return number;
		}

		public static float Mapf(float value,
				   float x1, float y1,
				   float x2, float y2)
		{
			float number = x2 + (y2 - x2) * ((value - x1) / (y1 - x1));

			return number;
		}

		public static int Index(int x, int y, int width)
		{
			return (y * width + x);
		}

		public static int Even(int value)
		{
			int x = value;
			x = x % 2 == 0 ? x : x - 1;
			return x;
		}

		public struct Debug
		{
			public string Title { get; }
			public float Value1 { get; }
			public float Value2 { get; }
			public bool DrawVector2 { get; }

			public Debug(string _title, float _value1, float _value2, bool _drawVector2)
			{
				Title = _title;
				Value1 = _value1;
				Value2 = _value2;
				DrawVector2 = _drawVector2;
			}
		}
	}

	public struct Vector2
	{
		public int X { get; }
		public int Y { get; }

		public Vector2(int _x, int _y)
		{
			X = _x;
			Y = _y;
		}

		public override string ToString() => $"({X}, {Y})";
	}
}