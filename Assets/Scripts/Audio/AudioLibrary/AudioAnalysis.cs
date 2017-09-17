using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class AudioAnalysis
{
// Public interface
	public static void InitTrackers(int baseNote, int numNotes, float decayRate, int sampleRate, int numChannels)
	{
		AA_InitTrackers(baseNote, numNotes, decayRate, sampleRate, numChannels);
	}
	
	public static void AddSamples(float[] samples, int nSamples)
	{
		AA_AddSamples(samples, nSamples);
	}
	
	public static void GetResonance(float[] outputResonance, int outputResonanceSize)
	{
		AA_GetResonance(outputResonance, outputResonanceSize);
	}
	
	public static int GetFundamental(int minNote, int maxNote, float fundamentalBiasPower)
	{
		return AA_GetFundamental(minNote, maxNote, fundamentalBiasPower);
	}

	public static int GetStrongestNote(int minNote, int maxNote)
	{
		return AA_GetStrongestNote(minNote, maxNote);
	}

	public static int GetStrongestNoteAcrossOctaves(int minNote, int maxNote)
	{
		return AA_GetStrongestNoteAcrossOctaves(minNote, maxNote);
	}

	public static float GetDisharmonyFactor(int drumNoteLower, int drumNoteUpper, int drumNoteSpread)
	{
		return AA_GetDisharmonyFactor(drumNoteLower, drumNoteUpper, drumNoteSpread);
	}
	
// Extra functions
	public static float getNoteFrequency(int noteNum)
	{
		return 27.5f * (float)System.Math.Pow(2.0f, (noteNum - 9) / 12.0f);
	}
	
	// Very rough approximation based on eyeballing an equal-loudness curve
	public static float getHearingAbility(int noteNum)
	{
		float logValue  = 0.0f;
		int[]   centres = new int[]   { -67,   67,   72,   75,  111,  112,  113};
		float[] logVals = new float[] {7.4f, 0.4f, 0.8f, 0.0f, 1.9f, 1.6f, 7.4f};
		
		// Check extremes
		if      (noteNum  < centres[0]) logValue = logVals[0];
		else if (noteNum >= centres[6]) logValue = logVals[6];
		else
		{
			// Find the interval containing the note
			int index = 1;
			for (; index < 6; ++index)
			{
				if (centres[index] > noteNum) break;
			}
			--index;
			
			// Get the centres above and below, and their associated values
			int   centreBelow = centres[index];
			int   centreAbove = centres[index + 1];
			float logValBelow = logVals[index];
			float logValAbove = logVals[index + 1];
			
			float smoothLerpVal = (noteNum - centreBelow) / (float)(centreAbove - centreBelow);
			smoothLerpVal *= smoothLerpVal * (3.0f - 2.0f * smoothLerpVal);
			
			logValue = logValBelow + (logValAbove - logValBelow) * smoothLerpVal;
		}
		
		return (float)System.Math.Pow(0.1f, logValue);
	}
	
	public class KeyDetector
	{
		private float[] allNotes;
		private float[] frequencyAccum;
		private int minNote;
		private int numNotes;
		private int numChannels;
		
		public KeyDetector(int trackerMinNote, int trackerNumNotes, int trackerNumChannels)
		{
			minNote = trackerMinNote;
			numNotes = trackerNumNotes;
			numChannels = trackerNumChannels;
			
			allNotes = new float[trackerNumNotes];
			frequencyAccum = new float[12];
		}
		
		public void update()
		{
			AudioAnalysis.GetResonance(allNotes, numNotes);
			for (int i = 0; i < numNotes; ++i)
			{
				int noteNum = minNote + (i / numChannels);
				frequencyAccum[noteNum % 12] += allNotes[i];
			}
		}
		
		public int getKeyVal()
		{
			int bestOffset = 0;
			float bestOffsetVal = 0.0f;
			
			// Where are the white keys on the keyboard?
			bool[] keyDef = new bool[12] {true, false, true, false, true, true, false, true, false, true, false, true};
			
			// Try shifting over
			for (int i = 0; i < 12; ++i)
			{
				float thisOffsetVal = 0.0f;
				for (int j = 0; j < 12; ++j)
				{
					// If a white key
					if (keyDef[(i+j)%12])
					{
						thisOffsetVal += frequencyAccum[j];
					}
				}
				
				if (thisOffsetVal > bestOffsetVal)
				{
					bestOffset = i;
					bestOffsetVal = thisOffsetVal;
				}
			}
			
			return bestOffset;
		}
		
		public System.String getKeyName(int keyVal, bool getMinorKey)
		{
			int keyValShifted = (keyVal + (getMinorKey ? 9 : 0)) % 12;
			
			// Pick one. We can't detect the difference.
			System.String[] keyNamesSharp = new System.String[12] { "C","C#","D","D#","E","F","F#","G","G#","A","A#","B" };
			//System.String[] keyNamesFlat  = new System.String[12] { "C","Db","D","Eb","E","F","Gb","G","Ab","A","Bb","B" };
			
			return keyNamesSharp[keyValShifted] + (getMinorKey ? " Minor" : " Major");
		}
	}


// Library interactions
	[DllImport ("AudioAnalysis")]
	private static extern void AA_InitTrackers(int baseNote, int numNotes, float decayRate, int sampleRate, int numChannels);
	
	[DllImport ("AudioAnalysis")]
	private static extern void AA_AddSamples(float[] samples, int nSamples);

	[DllImport ("AudioAnalysis")]
	private static extern void AA_GetResonance(float[] outputResonance, int outputResonanceSize);
	
	[DllImport ("AudioAnalysis")]
	private static extern int AA_GetFundamental(int minNote, int maxNote, float fundamentalBiasPower);
	
	[DllImport ("AudioAnalysis")]
	private static extern int AA_GetStrongestNote(int minNote, int maxNote);

	[DllImport ("AudioAnalysis")]
	private static extern int AA_GetStrongestNoteAcrossOctaves(int minNote, int maxNote);
	
	[DllImport ("AudioAnalysis")]
	private static extern float AA_GetDisharmonyFactor(int drumNoteLower, int drumNoteUpper, int drumNoteSpread);
}
