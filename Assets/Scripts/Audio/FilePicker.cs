using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class FilePicker
{
	// Note: This is a pretty ugly function. Under the hood, it relies on some behaviour that is actually
	// contrary to the MSDN documentation, but is nevertheless pretty reliably. The documentation is probably wrong.
	// What behaviour is that? This uses a flag to avoid changing the working directory (which Unity doesn't allow),
	// but MSDN says that flag is ignored for GetOpenFilename - it actually isn't.
	// 
	// Other than that, this locks up the main thread of the game, which causes its own problems.
	// The workaround for that is to the pause the game and mute the audio, then un-pause and un-mute.
	public static System.String OpenMusicFilePicker(System.String startLocation)
	{
		int bufferSize = 256;
		System.Text.StringBuilder chars = new System.Text.StringBuilder(startLocation);
		chars.Capacity = bufferSize;
		
		// Pause and mute
		float oldTimescale  = Time.timeScale;
		float oldAudioLevel = AudioListener.volume;
		Time.timeScale = 0.0f;
		AudioListener.volume = 0.0f;
		
		// The danger zone.
		FP_OpenMusicFilePicker(chars, bufferSize);
		
		// Unpause and unmute
		Time.timeScale = oldTimescale;
		AudioListener.volume = oldAudioLevel;
		
		// Change from Windows-style to *nix/Unity-style.
		return chars.ToString().Replace("\\", "/");
	}
	
	[DllImport ("FilePicker", CharSet = CharSet.Ansi)]
	private static extern void FP_OpenMusicFilePicker(System.Text.StringBuilder outBuffer, int bufferSize);
}
