using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

/// <summary>
/// This class analyses the music to determine intensities.
/// 
/// This is a modified script from Tour de Tune
/// </summary>
public class MusicController : MonoBehaviour
{
    // Configuration
    public List<AudioClip> audioClips;
	private AudioClip currAudioClip;
    private AudioSource audioSource;
    private int idx = 0;

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
	private int numChannels;
	private int clipSampleRate;
	private int audioProgStepBlocks;
	private float[] cachedData;
	
	// Buffers with temporary contents
	private float[] stepBlockBuffer;
	private float[] resonanceBuffer;

    private float lowThreshold;
    private float highThreshold;

    private Engine engine;
    private GameObject playerController;
    private RingGenerator ringGenerator;

    public int BaseNote
    {
        get
        {
            return baseNote;
        }

        set
        {
            baseNote = value;
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
            numNotes = value;
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
            decayRate = value;
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
            stepsPerSec = value;
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
            intensityFadeFactor = value;
        }
    }

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        engine = GameObject.Find("GameObjectSpawner").GetComponent<Engine>();
        playerController = GameObject.FindGameObjectWithTag("Player");

        audioClips.Clear();
        string[] oggFiles = Directory.GetFiles(Path.Combine(Application.dataPath, "Audio"), "*.ogg");
        WWW www = new WWW("file://" + oggFiles[0]);
        while (!www.isDone) { }
        AudioClip clip = www.GetAudioClip(false, false);
        clip.name = Path.GetFileName(oggFiles[0]);
        audioClips.Add(clip);
        Debug.Log("Loaded audio clip from " + oggFiles[0]);
        for (int i = 1; i < oggFiles.Length; i++)
        {
            string filename = oggFiles[i];
            www = new WWW("file://" + filename);
            clip = www.GetAudioClip(false, false);
            clip.name = Path.GetFileName(filename);
            audioClips.Add(clip);
            Debug.Log("Loaded audio clip from " + filename);
        }
    }

    private void Start()
    {
        ringGenerator = GameObject.FindGameObjectWithTag("pipecreator").GetComponent<RingGenerator>();
        currAudioClip = audioClips[0];
        audioSource.clip = currAudioClip;
        ReInitialise();
        CalculateTo(currAudioClip.length);
        CalculateThresholds();
        //OutputIntensityData();
    }

    private void CalculateThresholds()
    {
        float[] intensities = new float[numStepBlocks];
        float maxIntensity = 0;
        float minIntensity = float.MaxValue;
        for (int i = 0; i < numStepBlocks; i++)
        {
            float intensity = 0;
            for (int channelNum = 0; channelNum < numChannels; ++channelNum)
            {
                intensity += cachedData[i*stepBlockSize + numChannels*numNotes + channelNum];
            }
            intensity /= numChannels;
            intensities[i] = intensity;
            if (intensity > maxIntensity) maxIntensity = intensity;
            if (intensity < minIntensity) minIntensity = intensity;
        }

        int numBins = 20;
        int[] hist = new int[numBins];
        float binWidth = (maxIntensity - minIntensity)/numBins;
        foreach (float intensity in intensities)
        {
            for (int i = 1; i <= numBins; i++)
            {
                if (intensity < i*binWidth + minIntensity)
                {
                    hist[i - 1] += 1;
                    break;
                }
            }
        }

        // Uses the balanced histogram thresholding algorithm from https://en.wikipedia.org/wiki/Balanced_histogram_thresholding
        //int iStart = 0, iEnd = numBins - 1, iMid = (iStart + iEnd)/2;
        //int wLeft = 0, wRight = 0;
        //for (int i = 0; i < iMid + 1; i++) wLeft += hist[i];
        //for (int i = iMid + 1; i < iEnd; i++) wRight += hist[i];
        //while (iStart < iEnd)
        //{
        //    if (wRight > wLeft)
        //    {
        //        wRight -= hist[iEnd--];
        //        if ((iStart + iEnd)/2 < iMid)
        //        {
        //            wRight += hist[iMid];
        //            wLeft -= hist[iMid--];
        //        }
        //    }
        //    else
        //    {
        //        wLeft -= hist[iStart++];
        //        if ((iStart + iEnd)/2 >= iMid)
        //        {
        //            wLeft += hist[iMid + 1];
        //            wRight -= hist[iMid + 1];
        //            iMid++;
        //        }
        //    }
        //}

        // Adaptive thresholding algorithm
        int total = 0, totalWeight = 0;
        for (int i = 0; i <= numBins - 1; i++)
        {
            total += hist[i];
            totalWeight += i * hist[i];
        }
        int iMid = totalWeight / total;
        int iMidPrev = 0;

        while (iMid != iMidPrev)
        {
            int totalLeft = 0, totalRight = 0, totalWeightLeft = 0, totalWeightRight = 0;
            for (int i = 0; i <= iMid; i++)
            {
                totalLeft += hist[i];
                totalWeightLeft += i * hist[i];
            }
            for (int i = iMid + 1; i <= numBins - 1; i++)
            {
                totalRight += hist[i];
                totalWeightRight += i * hist[i];
            }
            iMidPrev = iMid;
            iMid = (totalWeightRight / totalRight + totalWeightLeft / totalLeft) / 2;
        }

        float threshold = binWidth*iMid + minIntensity;
        lowThreshold = threshold - (maxIntensity - minIntensity)/10;
        highThreshold = threshold + (maxIntensity - minIntensity)/10;
        Debug.Log("Calculated thresholds low = " + lowThreshold + ", high = " + highThreshold);
    }

    private void Update()
    {
        if (engine.isStarted && !engine.isWarmingUp)
        {
            float playerVelocity = playerController.GetComponent<Rigidbody>().velocity.x + 0.1f;
            float timeToLastRing = Mathf.Abs(ringGenerator.LastGeneratedRing.transform.position.x - playerController.transform.position.x) / playerVelocity;
            if (timeToLastRing > currAudioClip.length - audioSource.time) timeToLastRing = currAudioClip.length - audioSource.time;
            float intensity = GetIntensityAt(audioSource.time + timeToLastRing);
            if ((intensity > highThreshold && !ringGenerator.IsHighIntensity) || (intensity < lowThreshold && ringGenerator.IsHighIntensity))
            {
                ringGenerator.PhaseChange();
                Debug.Log("Music intensity: " + (ringGenerator.IsHighIntensity ? Engine.Interval.HIGH_INTENSITY : Engine.Interval.LOW_INTENSITY));
            }
        }
    }

    public void StopSong()
    {
        audioSource.Stop();
    }

    public void PlaySong()
    {
        audioSource.Play();
    }

    public void ChangeSong()
    {
        StopSong();
        idx++;
        currAudioClip = audioClips[idx];
        audioSource.clip = currAudioClip;
        PlaySong();
    }

    public void ReInitialise()
	{
		numChannels = currAudioClip.channels;
		clipSampleRate = currAudioClip.frequency;
		
		// (notes + intensity) for each channel
		stepBlockSize = (NumNotes + 1) * numChannels;
		stepBlockSamples = Mathf.CeilToInt(clipSampleRate / StepsPerSec);
		numStepBlocks = (currAudioClip.samples + stepBlockSamples - 1) / stepBlockSamples; // = audioClip.samples / stepBlockSamples, but rounded up
		
		cachedData = new float[numStepBlocks * stepBlockSize];
		audioProgStepBlocks = -1;
		
		AudioAnalysis.InitTrackers(BaseNote, NumNotes, DecayRate, clipSampleRate, numChannels);
		
		// Initialise buffers
		stepBlockBuffer = new float[stepBlockSamples * numChannels];
		resonanceBuffer = new float[NumNotes * numChannels];
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
			int offsetCurr = blockOffset + i * numChannels + channelNum;
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
			int timeSamplesSize = stepBlockSamples * numChannels;
			
			// Send audio data to the tracker
			currAudioClip.GetData(stepBlockBuffer, timeSamplesBase);
			AudioAnalysis.AddSamples(stepBlockBuffer, timeSamplesSize);
			
			// Get resonance back from the tracker
			AudioAnalysis.GetResonance(resonanceBuffer, NumNotes * numChannels);
			System.Array.Copy(resonanceBuffer, 0, cachedData, stepBlockSize * audioProgStepBlocks, resonanceBuffer.Length);
			
			int intensityOffset = stepBlockSize * audioProgStepBlocks + resonanceBuffer.Length;
			for (int channelNum = 0; channelNum < numChannels; ++channelNum)
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
			noteArray[i] = cachedData[baseOffset + i * numChannels];
		}
		
		return noteArray;
	}
	
	public float GetIntensityAt(float timeSeconds)
	{
		CalculateTo(timeSeconds);
		float totalIntensity = 0.0f;
		
		int baseOffset = CalculateBlockNum(timeSeconds) * stepBlockSize + numChannels * NumNotes;
		for (int channelNum = 0; channelNum < numChannels; ++channelNum)
		{
			totalIntensity += cachedData[baseOffset + channelNum];
		}
		
		return totalIntensity / numChannels;
	}
	
	public float GetIntensityAt(float timeSeconds, int channelNum)
	{
		CalculateTo(timeSeconds);
		
		return cachedData[CalculateBlockNum(timeSeconds) * stepBlockSize + numChannels * NumNotes + channelNum];
	}

    private void OutputIntensityData()
    {
        CalculateTo(currAudioClip.length);
        string logPath = Path.Combine(Application.persistentDataPath, "intensity_log_" + currAudioClip.name + ".txt");
        string dataStr = "";
        Debug.Log("Outputting intensity data to file " + logPath + " ...");
        for (int i = 0; i < numStepBlocks; i++)
        {
            //float intensity = 0;
            //for (int channelNum = 0; channelNum < numChannels; ++channelNum)
            //{
            //    intensity += cachedData[i*stepBlockSize + numChannels*numNotes + channelNum];
            //}
            //intensity /= numChannels;
            //dataStr += intensity.ToString() + "\n";

            float intensity = 0;
            for (int j = 0; j < NumNotes + 1; j++)
            {
                if (j == NumNotes) continue;
                for (int channelNum = 0; channelNum < numChannels; ++channelNum)
                {
                    intensity += cachedData[i * stepBlockSize + j * numChannels + channelNum];
                }
            }
            intensity /= (numChannels * NumNotes);
            dataStr += intensity + "\n";
        }
        File.WriteAllText(logPath, dataStr);
    }
}
