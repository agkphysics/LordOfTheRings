using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Threading;
using System.Collections;
using ProgressBar;
using UnityEngine.Audio;

/// <summary>
/// This class analyses the music to determine intensities.
/// 
/// This is a modified script from Tour de Tune.
/// </summary>
public class MusicController : MonoBehaviour
{
    public static int BaseNote { get; set; }
    public static int NumNotes { get; set; }
    public static float DecayRate { get; set; }
    public static float StepsPerSec { get; set; }
    public static float IntensityFadeFactor { get; set; }
    
    public float[] BeatTimes { get { return currSong.BeatTimes; } }
    public float NextBeat { get { return BeatTimes[beatIdx]; } }
    public float NextBeatDelta { get { return NextBeat - audioSource.time; } }
    public Boolean IsEnded { get; set; }
    public Engine.Interval Intensity { get; private set; }

    static MusicController()
    {
        BaseNote = 0;
        NumNotes = 120;
        DecayRate = 0.999f;
        StepsPerSec = 30.0f;
        IntensityFadeFactor = 0.02f;
    }

    private List<Song> songs;
    private Song currSong;
    private AudioSource audioSource;
    private float sessionLen;
    private int idx = 0;
    private int beatIdx = 0;
    private float pitchChange = 0.10f;
    private float pitchChangeTime = 1f;
    private float maxPitch = 1.2f;
    private float minPitch = 0.8f;
    private object pitchLock = new object();
    private bool stopThread;

    private Engine engine;
    private BirdController playerController;
    private RingGenerator ringGenerator;
    private AudioMixerGroup audioMasterGroup;

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        engine = GameObject.FindGameObjectWithTag("GameController").GetComponent<Engine>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<BirdController>();
        audioMasterGroup = audioSource.outputAudioMixerGroup;
        IsEnded = false;
        stopThread = false;
        Intensity = Engine.Interval.LOW_INTENSITY;

        songs = new List<Song>();
        string[] oggFiles = Directory.GetFiles(Path.Combine(Application.dataPath, "Audio"), "*.ogg");
        foreach (string filename in oggFiles)
        {
            WWW www = new WWW("file://" + filename);
            while (!www.isDone) { }
            AudioClip clip = www.GetAudioClip(false, false);
            clip.name = Path.GetFileName(filename);
            songs.Add(new Song(clip));
            
            Debug.Log("Loaded audio clip from " + filename);
        }

        sessionLen += songs[0].Clip.length;
    }

    private void Start()
    {
        ringGenerator = GameObject.FindGameObjectWithTag("pipecreator").GetComponent<RingGenerator>();
        currSong = songs[idx++];
        audioSource.clip = currSong.Clip;
        if (!engine.noMusicCondition)
        {
            new Thread(new ThreadStart(() =>
            {
                foreach (Song song in songs)
                {
                    song.Initialise();
                    if (stopThread) return;
                }
                playerController.TargetRPM = currSong.BPM;
            })).Start();
        }
    }

    private void OnApplicationQuit()
    {
        stopThread = true;
    }

    private void Update()
    {
        if (currSong.IsFinishedInitialising)
        {
            if (engine.IsStarted && !engine.IsWarmingUp)
            {
                float playerVelocity = playerController.GetComponent<Rigidbody>().velocity.x + 1f;
                float timeToLastRing = Mathf.Abs(ringGenerator.LastGeneratedRing.transform.position.x - playerController.transform.position.x) / playerVelocity;
                if (timeToLastRing > currSong.Length - audioSource.time) timeToLastRing = currSong.Length - audioSource.time;
                float intensity = currSong.GetIntensityAt(audioSource.time + timeToLastRing);

                if (((intensity > currSong.HighThreshold && !ringGenerator.IsHighIntensity) || (intensity < currSong.LowThreshold && ringGenerator.IsHighIntensity)) && !engine.noMusicCondition)
                {
                    ringGenerator.PhaseChange();
                    Intensity = Intensity == Engine.Interval.LOW_INTENSITY ? Engine.Interval.HIGH_INTENSITY : Engine.Interval.LOW_INTENSITY;
                    Debug.Log("Time until last ring: " + timeToLastRing + ", music intensity: " + intensity + ", " + (ringGenerator.IsHighIntensity ? Engine.Interval.HIGH_INTENSITY : Engine.Interval.LOW_INTENSITY));
                }
            }

            if (audioSource.isPlaying)
            {
                if (BeatTimes[beatIdx] < audioSource.time && beatIdx < BeatTimes.Length - 1) beatIdx++;

                GameObject.Find("ProgressBar").GetComponent<ProgressBarBehaviour>().Value = 100f*audioSource.time/currSong.Clip.length;
                if (Input.GetKeyDown(KeyCode.RightBracket))
                {
                    IncreasePitch();
                }
                else if (Input.GetKeyDown(KeyCode.LeftBracket))
                {
                    DecreasePitch();
                }
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                currSong.OutputIntensityData();
                currSong.OutputBeatData();
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            ChangeSong();
            PlaySong();
        }
    }

    public void IncreasePitch()
    {
        if (audioSource.pitch < maxPitch) StartCoroutine("_IncreasePitch");
    }

    public void DecreasePitch()
    {
        if (audioSource.pitch > minPitch) StartCoroutine("_DecreasePitch");
    }

    public void ResetPitch()
    {
        audioSource.pitch = 1;
        playerController.TargetRPM = currSong.BPM * audioSource.pitch;
    }

    private IEnumerator _IncreasePitch()
    {
        lock (pitchLock)
        {
            for (int i = 0; i < 10 && audioSource.pitch < maxPitch; i++)
            {
                audioSource.pitch += pitchChange / 10;
                //audioMasterGroup.audioMixer.SetFloat("Pitch", 1/audioSource.pitch);
                yield return new WaitForSeconds(1f / 10f);
            }
            playerController.TargetRPM = currSong.BPM * audioSource.pitch;
        }
    }

    private IEnumerator _DecreasePitch()
    {
        lock (pitchLock)
        {
            for (int i = 0; i < 10 && audioSource.pitch > minPitch; i++)
            {
                audioSource.pitch -= pitchChange / 10;
                //audioMasterGroup.audioMixer.SetFloat("Pitch", 1/audioSource.pitch);
                yield return new WaitForSeconds(1f / 10f);
            }
            playerController.TargetRPM = currSong.BPM * audioSource.pitch;
        }
    }

    private void OnGUI()
    {
        GUI.skin = engine.skin;
        if (!currSong.IsFinishedInitialising && !engine.noMusicCondition) GUI.Box(new Rect((Screen.width / 3), (Screen.height / 4), (Screen.width / 3), (Screen.height / 12)), new GUIContent("Loading song..."));
    }

    public void StopSong()
    {
        audioSource.Stop();
        StopCoroutine("_PlaySong");
    }

    public void PlaySong()
    {
        if (currSong.IsFinishedInitialising)
        {
            audioSource.Play();
            Invoke("EndSession", sessionLen);
        }
        else
        {
            StartCoroutine("_PlaySong");
        }
    }

    private IEnumerator _PlaySong()
    {
        while (!currSong.IsFinishedInitialising) yield return null;
        audioSource.Play();
    }

    public void ChangeSong()
    {
        StopSong();
        currSong = songs[idx++];
        audioSource.clip = currSong.Clip;
        sessionLen = currSong.Clip.length;
        playerController.TargetRPM = currSong.BPM * audioSource.pitch;
        //new Thread(new ThreadStart(() => {
        //    currSong.Initialise();
        //    playerController.TargetRPM = currSong.BPM;
        //    Debug.Log("Current BPM of song is " + currSong.BPM);
        //})).Start();
    }

    private void EndSession()
    {
        IsEnded = true;
    }

    /// <summary>
    /// Convenience class that holds all the song calculation functions.
    /// </summary>
    class Song
    {
        public AudioClip Clip { get; private set; }
        public float Length { get; private set; }
        public float LowThreshold { get; private set; }
        public float HighThreshold { get; private set; }
        public float BPM { get; private set; }
        public float[] BeatTimes { get; private set; }
        public bool IsFinishedInitialising { get; private set; }

        // Values derived from configuration constants
        private int numStepBlocks;
        private int stepBlockSamples;
        private int stepBlockSize;

        // Changing state
        private int numChannels;
        private int numSamples;
        private int clipSampleRate;
        private int audioProgStepBlocks;
        private float[] cachedData;
        private float[] clipData;

        // Buffers with temporary contents
        private float[] stepBlockBuffer;
        private float[] resonanceBuffer;

        public Song(AudioClip clip)
        {
            Clip = clip;
            BPM = -1;
            IsFinishedInitialising = false;
            numChannels = Clip.channels;
            clipSampleRate = Clip.frequency;
            numSamples = Clip.samples;
            clipData = new float[numSamples*numChannels];
            Length = Clip.length;
            Clip.GetData(clipData, 0);
        }

        public void Initialise()
        {
            if (!IsFinishedInitialising)
            {
                // (notes + intensity) for each channel
                stepBlockSize = (NumNotes + 1) * numChannels;
                stepBlockSamples = Mathf.CeilToInt(clipSampleRate / StepsPerSec);
                numStepBlocks = (numSamples + stepBlockSamples - 1) / stepBlockSamples; // = Clip.samples / stepBlockSamples, but rounded up

                cachedData = new float[numStepBlocks * stepBlockSize];
                audioProgStepBlocks = -1;

                AudioAnalysis.InitTrackers(BaseNote, NumNotes, DecayRate, clipSampleRate, numChannels);

                // Initialise buffers
                stepBlockBuffer = new float[stepBlockSamples * numChannels];
                resonanceBuffer = new float[NumNotes * numChannels];

                CalculateTo(Length);
                CalculateThresholds();
                CalculateBPM();
                IsFinishedInitialising = true;
            }
        }

        private void GetData(float[] buf, int offset)
        {
            for (int i = 0; i < buf.Length; i++) buf[i] = clipData[(offset*numChannels + i) % clipData.Length];
        }

        private int CalculateBlockNum(float timeSeconds)
        {
            // Calculate the number of samples represented by this time
            int timeSamples = (int)(0.5f + timeSeconds * clipSampleRate);

            // Convert to the step block number. This is the block that contains the time "timeSeconds"
            return timeSamples / stepBlockSamples;
        }

        private float CalculateIntensity(int blockNum, int channelNum)
        {
            float hitScale = 75.0f;
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

        private void CalculateTo(float timeSeconds)
        {
            int timeStepBlocks = CalculateBlockNum(timeSeconds);

            // We will calculate blocks in the range [audioProgStepBlocks, timeStepBlocks]
            while (audioProgStepBlocks < timeStepBlocks)
            {
                ++audioProgStepBlocks;

                int timeSamplesBase = stepBlockSamples * audioProgStepBlocks;
                int timeSamplesSize = stepBlockSamples * numChannels;

                // Send audio data to the tracker
                GetData(stepBlockBuffer, timeSamplesBase);
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
                    intensity += cachedData[i*stepBlockSize + numChannels*NumNotes + channelNum];
                }
                intensity /= numChannels;
                intensities[i] = intensity;
                if (intensity > maxIntensity) maxIntensity = intensity;
                if (intensity < minIntensity) minIntensity = intensity;
            }

            int[] hist = new int[20];
            float binWidth = (maxIntensity - minIntensity)/hist.Length;
            foreach (float intensity in intensities)
            {
                for (int i = 1; i <= hist.Length; i++)
                {
                    if (intensity < i*binWidth + minIntensity)
                    {
                        hist[i - 1] += 1;
                        break;
                    }
                }
            }

            // Truncating the histogram, to ignore skew
            int[] _hist = new int[hist.Length*2/3];
            int numDeletedBins = hist.Length - _hist.Length;
            for (int i = 0; i < _hist.Length; i++) _hist[i] = hist[i + numDeletedBins];
            hist = _hist;

            // Uses the balanced histogram thresholding algorithm from https://en.wikipedia.org/wiki/Balanced_histogram_thresholding
            //int iStart = 0, iEnd = hist.Length - 1, iMid = (iStart + iEnd)/2;
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
            for (int i = 0; i <= hist.Length - 1; i++)
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
                for (int i = iMid + 1; i <= hist.Length - 1; i++)
                {
                    totalRight += hist[i];
                    totalWeightRight += i * hist[i];
                }
                iMidPrev = iMid;
                iMid = (totalWeightRight / totalRight + totalWeightLeft / totalLeft) / 2;
            }

            float threshold = binWidth*(iMid + numDeletedBins) + minIntensity;
            LowThreshold = threshold - (maxIntensity - minIntensity)/10;
            HighThreshold = threshold + (maxIntensity - minIntensity)/10;
            Debug.Log("Calculated thresholds low = " + LowThreshold + ", high = " + HighThreshold);
        }

        private void CalculateBPM()
        {
            if (BPM == -1)
            {
                float[] beatIntensities = new float[numStepBlocks];
                int[] noteRange = { 47, 51 };
                for (int i = 0; i < numStepBlocks; i++)
                {
                    float intensity = 0;
                    for (int j = noteRange[0]; j <= noteRange[1]; j++)
                    {
                        for (int channelNum = 0; channelNum < numChannels; ++channelNum)
                        {
                            intensity += cachedData[i * stepBlockSize + j * numChannels + channelNum];
                        }
                    }
                    intensity /= ((noteRange[1] - noteRange[0] + 1)*numChannels);
                    beatIntensities[i] = intensity;
                }
                List<float> beatIntensitiesSorted = new List<float>(beatIntensities);
                
                beatIntensitiesSorted.Sort();
                // Use 90th percentile to use as cutoff for beat detection
                float percentile95 = beatIntensitiesSorted[(int)(beatIntensitiesSorted.Count*0.95)];
                
                int[] beatDiffArray = new int[numStepBlocks];
                int[] beatArray = new int[numStepBlocks];
                int idx = 0, prevBeat = 0;
                for (int i = 0; i < numStepBlocks; i++)
                {
                    float intensity = beatIntensities[i];
                    if (intensity > percentile95)
                    {
                        if (idx == 0) beatDiffArray[0] = i;
                        else beatDiffArray[idx] = i - prevBeat;
                        beatArray[idx] = i;
                        prevBeat = i;
                        idx++;
                    }
                }

                int[] beatHist = new int[100];
                for (int i = 1; i < idx; i++)
                {
                    int beatDiff = beatDiffArray[i];
                    // Ignore time differences too small or large. We expect BPM multiples to be between 20 and 200.
                    if (beatDiff >= 9 && beatDiff <= 90) beatHist[beatDiff] += 1;
                }
                // Sort by frequency and obtain value which occurs most frequently.
                Dictionary<int, int> sortedArray = new Dictionary<int, int>(beatHist.Length);
                for (int i = 1; i < beatHist.Length; i++)
                {
                    sortedArray.Add(i, beatHist[i]);
                }
                List<KeyValuePair<int, int>> pairs = sortedArray.ToList();
                pairs.Sort((x, y) => x.Value.CompareTo(y.Value));

                int mode1 = pairs[pairs.Count - 1].Key;
                int mode2 = pairs[pairs.Count - 2].Key;
                float a = mode1/(float)mode2;
                float b = mode2/(float)mode1;
                int mode;
                if (a - 1 > 0.5 && b - 1 > 0.5 && Mathf.Abs(Mathf.Round(a) - a) < 0.1 || Mathf.Abs(Mathf.Round(b) - b) < 0.1)
                {
                    // The two most common beat differences are multiples of each other but not equal,
                    // use the larger to give smaller BPM.
                    mode = Math.Max(mode1, mode2);
                }
                else
                {
                    // Use the most common if they're not multiples of each other.
                    mode = mode1;
                }

                BPM = 60*StepsPerSec/mode;
                while (BPM > 40) BPM /= 2;
                BeatTimes = new float[idx];
                for (int i = 0; i < idx; i++) BeatTimes[i] = beatArray[i]/StepsPerSec;
                Debug.Log("Calculated BPM: " + BPM);
            }
        }

        public void OutputIntensityData()
        {
            CalculateTo(Clip.length);
            string intensityPath = Path.Combine(Application.persistentDataPath, "intensity_" + Clip.name + ".tsv");
            System.Text.StringBuilder dataStr = new System.Text.StringBuilder();
            Debug.Log("Outputting intensity data to file " + intensityPath);
            for (int i = 0; i < numStepBlocks; i++)
            {
                dataStr.Append(i/StepsPerSec).Append("\t");
                for (int j = 0; j < NumNotes + 1; j++)
                {
                    float intensity = 0;
                    for (int channelNum = 0; channelNum < numChannels; ++channelNum)
                    {
                        intensity += cachedData[i * stepBlockSize + j * numChannels + channelNum];
                    }
                    intensity /= numChannels;
                    dataStr.Append(intensity).Append("\t");
                }
                dataStr.Append("\n");
            }
            File.WriteAllText(intensityPath, dataStr.ToString());
        }

        public void OutputBeatData()
        {
            string beatPath = Path.Combine(Application.persistentDataPath, "beat_" + Clip.name + ".tsv");
            System.Text.StringBuilder dataStr = new System.Text.StringBuilder();
            Debug.Log("Outputting beat data to file " + beatPath);
            dataStr = new System.Text.StringBuilder();
            for (int i = 0; i < BeatTimes.Length; i++)
            {
                dataStr.Append(BeatTimes[i]).Append("\n");
            }
            File.WriteAllText(beatPath, dataStr.ToString());
        }
    }
}
