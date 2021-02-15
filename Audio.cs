using System;
using System.Media;
using System.Threading;
using System.Windows.Media;
using System.IO;
using System.Collections.Generic;

public class AudioManager
{
	private string local = Environment.CurrentDirectory;
	public bool menu;

	public int _master = 75;
	private int master = 75;
	public int _music = 75;
	private int music = 75;
	public int _other = 75;
	private int other = 75;

	private MediaPlayer othermp;
	private MediaPlayer bgmmp;

	public void Play(string filename)
	{
		othermp = new MediaPlayer();
		othermp.Open(new Uri(filename));
		othermp.Volume = (master / 100.0f) * (other / 100.0f);
		othermp.Play();
	}
	public void SetVolume()
	{
		if (_master > 100) { _master = 100; }
		if (_master < 0) { _master = 0; }

		if (_music > 100) { _music = 100; }
		if (_music < 0) { _music = 0; }

		if (_other > 100) { _other = 100; }
		if (_other < 0) { _other = 0; }


		master = _master;
		music = _music;
		other = _other;
	}

	public void PlaySound(string relativeAdress)
	{
		string adress = local + "\\audio\\" + relativeAdress;
		Play(adress);
	}
	public void PlayBGM()
	{
		bgmmp = new MediaPlayer();
		string bgm1 = local + "\\audio\\" + "BGMPRE.wav";
		string bgm2 = local + "\\audio\\" + "BGM.wav";

		while (true)
		{
			bgmmp.Volume = (master / 100.0f) * (music / 100.0f);

			if (menu)
			{
				bgmmp.Open(new Uri(bgm1));
				bgmmp.Play();
				System.Threading.Thread.Sleep(2240);
				bgmmp.Position = TimeSpan.FromMilliseconds(1);
			}
			else
			{
				bgmmp.Open(new Uri(bgm2));
				bgmmp.Play();
				System.Threading.Thread.Sleep(56680);
				bgmmp.Position = TimeSpan.FromMilliseconds(1);
			}
		}
	}
}