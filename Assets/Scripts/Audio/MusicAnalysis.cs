using UnityEngine;
using System.Collections;

/// <summary>
/// This class analyses the music to determine intensities.
/// 
/// This is a modified script from Tour de Tune
/// </summary>
public class MusicAnalysis : MonoBehaviour
{
	// Configuration
	public  AudioClip audioClip;
    private AudioSource audioSource;

	private int baseNote = 0;
	private int numNotes = 120;
	private float decayRate = 0.999f;
	private float stepsPerSec = 30.0f;
	private float intensityFadeFactor = 0.02f;
	
	// Values derived from configuration constants
	private int numStepBlocks;
	private int stepBlockSamples;
	private int stepBlockSize;
	
	// Changing state
	private int clipChannels;
	private int clipSampleRate;
	private int audioProgStepBlocks;
	private float[] cachedData;
	
	// Buffers with temporary contents
	private float[] stepBlockBuffer;
	private float[] resonanceBuffer;

    public int BaseNote
    {
        get
        {
            return baseNote;
        }

        set
        {
            baseNote=value;
        }
    }

    public int NumNotes
    {
        get
        {
            return numNotes;
        }

        set
        {
            numNotes=value;
        }
    }

    public float DecayRate
    {
        get
        {
            return decayRate;
        }

        set
        {
            decayRate=value;
        }
    }

    public float StepsPerSec
    {
        get
        {
            return stepsPerSec;
        }

        set
        {
            stepsPerSec=value;
        }
    }

    public float IntensityFadeFactor
    {
        get
        {
            return intensityFadeFactor;
        }

        set
        {
            intensityFadeFactor=value;
        }
    }

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        Initialise();
        CalculateTo(audioClip.length);
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private void Update()
    {
        float intensity = GetIntensityAt(audioSource.time);
        Debug.Log("Intensity: " + intensity);
    }

    public void Initialise()
	{
		clipChannels = audioClip.channels;
		clipSampleRate = audioClip.frequency;
		
		// (notes + intensity) for each channel
		stepBlockSize = (NumNotes + 1) * clipChannels;
		stepBlockSamples = (int)(clipSampleRate / StepsPerSec + 0.5f);
		numStepBlocks = (audioClip.samples + stepBlockSamples - 1) / stepBlockSamples; // = audioClip.samples / stepBlockSamples, but rounded up
		
		cachedData = new float[numStepBlocks * stepBlockSize];
		audioProgStepBlocks = -1;
		
		AudioAnalysis.InitTrackers(BaseNote, NumNotes, DecayRate, clipSampleRate, clipChannels);
		
		// Initialise buffers
		stepBlockBuffer = new float[stepBlockSamples * clipChannels];
		resonanceBuffer = new float[NumNotes * clipChannels];
	}
	
	protected int CalculateBlockNum(float timeSeconds)
	{
		// Calculate the number of samples represented by this time
		int timeSamples = (int)(0.5f + timeSeconds * clipSampleRate);
		
		// Convert to the step block number. This is the block that contains the time "timeSeconds"
		return timeSamples / stepBlockSamples;
	}
	
	protected float CalculateIntensity(int blockNum, int channelNum)
	{
		float hitScale  = 75.0f;
		float holdScale = 25.0f;
		
		int blockOffset = blockNum * stepBlockSize;
		
		float channelIntensity = 0.0f;
		for (int i = 0; i < NumNotes; ++i)
		{
			int offsetCurr = blockOffset + i * clipChannels + channelNum;
			int offsetPrev = offsetCurr - stepBlockSize;
			float intensityMult = 1.0f;//AudioAnalysis.getHearingAbility(i);
			
			float hitComponent = 0.0f;
			float holdComponent = holdScale * cachedData[offsetCurr];
			if (offsetPrev >= 0)
			{
				hitComponent = hitScale * System.Math.Max(0.0f, cachedData[offsetCurr] - cachedData[offsetPrev]);
			}
			
			channelIntensity += intensityMult * (hitComponent + holdComponent);
		}
		
		return channelIntensity / NumNotes;
	}
	
	public void CalculateTo(float timeSeconds)
	{
		int timeStepBlocks = CalculateBlockNum(timeSeconds);
		
		// We will calculate blocks in the range [audioProgStepBlocks, timeStepBlocks]
		while (audioProgStepBlocks < timeStepBlocks)
		{
			++audioProgStepBlocks;
			
			int timeSamplesBase = stepBlockSamples * audioProgStepBlocks;
			int timeSamplesSize = stepBlockSamples * clipChannels;
			
			// Send audio data to the tracker
			audioClip.GetData(stepBlockBuffer, timeSamplesBase);
			AudioAnalysis.AddSamples(stepBlockBuffer, timeSamplesSize);
			
			// Get resonance back from the tracker
			AudioAnalysis.GetResonance(resonanceBuffer, NumNotes * clipChannels);
			System.Array.Copy(resonanceBuffer, 0, cachedData, stepBlockSize * audioProgStepBlocks, resonanceBuffer.Length);
			
			int intensityOffset = stepBlockSize * audioProgStepBlocks + resonanceBuffer.Length;
			for (int channelNum = 0; channelNum < clipChannels; ++channelNum)
			{
				float rawIntensity = CalculateIntensity(audioProgStepBlocks, channelNum);
				if (audioProgStepBlocks == 0)
				{
					cachedData[intensityOffset + channelNum] = rawIntensity;
				}
				else
				{
					float oldIntensity = cachedData[intensityOffset + channelNum - stepBlockSize];
					cachedData[intensityOffset + channelNum] = oldIntensity + (rawIntensity - oldIntensity) * IntensityFadeFactor;
				}
			}
		}
	}
	
	public float[] GetNotesAt(float timeSeconds, int channelNum)
	{
		CalculateTo(timeSeconds);
		float[] noteArray = new float[NumNotes];
		
		int baseOffset = CalculateBlockNum(timeSeconds) * stepBlockSize + channelNum;
		for (int i = 0; i < NumNotes; ++i)
		{
			noteArray[i] = cachedData[baseOffset + i * clipChannels];
		}
		
		return noteArray;
	}
	
	public float GetIntensityAt(float timeSeconds)
	{
		CalculateTo(timeSeconds);
		float totalIntensity = 0.0f;
		
		int baseOffset = CalculateBlockNum(timeSeconds) * stepBlockSize + clipChannels * NumNotes;
		for (int channelNum = 0; channelNum < clipChannels; ++channelNum)
		{
			totalIntensity += cachedData[baseOffset + channelNum];
		}
		
		return totalIntensity / clipChannels;
	}
	
	public float GetIntensityAt(float timeSeconds, int channelNum)
	{
		CalculateTo(timeSeconds);
		
		return cachedData[CalculateBlockNum(timeSeconds) * stepBlockSize + clipChannels * NumNotes + channelNum];
	}
}
