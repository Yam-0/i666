using System;
using static asparagus.Game;

class Counter
{
	private static string[] values = new string[10]{
		"#### ## ## ####",
		"  #  #  #  #  #",
		"###  #####  ###",
		"###  ####  ####",
		"# ## ####  #  #",
		"####  ###  ####",
		"####  #### ####",
		"###  #  #  #  #",
		"#### ##### ####",
		"#### ####  ####"
	};

	public static Pixel[] Counter1(int value, Color _color)
	{
		int number = value;
		if (number < 0) { number = 0; }
		if (number > 9) { number = 9; }

		Pixel[] pixels = new Pixel[3 * 5];

		for (var i = 0; i < pixels.Length; i++)
		{
			pixels[i].Char.UnicodeChar = ' ';
			int color = ColorInt(TRANSPARENT, TRANSPARENT);

			if (values[number][i] == '#')
			{
				color = ColorInt(_color.foreground, _color.background);
			}
			else
			{
				color = ColorInt(TRANSPARENT, TRANSPARENT);
			}

			pixels[i].Attributes = (short)color;
		}

		return pixels;
	}
}