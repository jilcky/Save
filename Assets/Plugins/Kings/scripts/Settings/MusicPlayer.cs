using UnityEngine;
using System.Collections;


public class MusicPlayer : MonoBehaviour {

	[Tooltip("List of possible audio clips for playing in the game.")]
	public AudioClip[] _audioClips;
	[Tooltip("Audiosource for playing the audio clips.")]
	public AudioSource mainAudio;
	[Tooltip("Select one of the audi clips randomly at activation of the gameobject.")]
	public bool playRandomAtStart = false;
	[Tooltip("Loop the selected audio clip (or play all the clips in the list).")]
	public bool loopSong = false;


	private int nrOfSources;
	private AudioClip  actualSource;
	private int actualIndex = 0;

    string songIndexKey = "MusicIndex";

	public bool testNext = false;
	private float songDuration = 0f;

	[HideInInspector][Range(0.0f, 5f)] [SerializeField] private float fadeDuration = 0f; 

	void Start () {
		nrOfSources = _audioClips.Length;

        load();
        actualIndex = clipIndex(actualIndex);

		if (playRandomAtStart == true) {
			int rnd = Random.Range (0, nrOfSources);
			actualIndex = rnd;
		} 

        PlaySong(actualIndex);

		StartCoroutine (multimediaTimer ());


		if (loopSong == true) {
			mainAudio.loop = true;
		}
	}

    bool audioShouldPlay = false;
    void play()
    {
        audioShouldPlay = true;
        if(mainAudio.enabled == true)
        {
            mainAudio.Play();
        }
    }

    void stop()
    {
        audioShouldPlay = false;
        if (mainAudio.enabled == true)
        {
            mainAudio.Stop();
        }
    }

    void PlaySong(int index)
    {
        actualIndex = clipIndex(index);
        actualSource = _audioClips[actualIndex];
        mainAudio.clip = _audioClips[actualIndex];
        songDuration = mainAudio.clip.length;   //memorize the song-duration
        play();

        //Debug.LogWarning("play song index " + actualIndex.ToString());
    }


    IEnumerator multimediaTimer(){
	
		float now = Time.realtimeSinceStartup;
		float last = now;


		while (true) {
			now = Time.realtimeSinceStartup;

			songDuration-= (now-last);

			if(loopSong == false){
			if(songDuration<fadeDuration){
				nextListedSong();


				if(songDuration <=fadeDuration){
						Debug.LogWarning("An audioclip  is shorter than the fading time, this can cause strange sound behaviour.");
				}
			}
			}else{
				//no pre-time for fading out, if the song replays. This can't be controlled with the volume-Slider.
				if(songDuration<0f){
					//nextListedSong();
										
					//if(songDuration <=fadeDuration){
					//	Debug.LogWarning("An audioclip  is shorter than the fading time, this can cause strange sound behavior.");
					//}
				}
			}
		
			last = now;

			yield return new WaitForSeconds(0.1f); //this is not accurate. 
		}

	}

	public void nextSong(){

		mainAudio.loop = false;

		actualIndex++;
		if (actualIndex >= nrOfSources) {
			actualIndex = 0;
		}

        PlaySong(actualIndex);

        mainAudio.loop = true;
        loopSong = false;		//the user breaks the loop, we reload this value on next ending song.

        save();
	}

    int clipIndex(int index)
    {
        nrOfSources = _audioClips.Length;

        if (index >= nrOfSources)
        {
            index = nrOfSources - 1;
        }
        if (index < 0)
        {
            index = 0;
        }

        return index;
    }

    public void setSong(int index) {
        mainAudio.loop = false;

        PlaySong(index);

        mainAudio.loop = true;

        save();
        //loopSong = false;		//the user breaks the loop, we reload this value on next ending song.
    }

	private void replaySong(){

        PlaySong(actualIndex);
	}

	private void nextListedSong(){
		if (loopSong == true) {
			replaySong();
		} else {
			nextSong();
		}
        save();
	}

    bool oldAudiosourceEnabled = false;
	void Update () {
		if (testNext == true) {
			nextSong();
			testNext = false;
		}
        actualizeAudioPlay();

	}

    void actualizeAudioPlay()
    {
        //detect if state of the main audio souce changed.
        if (oldAudiosourceEnabled == false && mainAudio.enabled == true)
        {
            if (audioShouldPlay == true)
            {
                play();
            }

            if (audioShouldPlay == false)
            {
                stop();
            }
        }
        oldAudiosourceEnabled = mainAudio.enabled;
    }

	void OnDisable(){
        stop();
		StopAllCoroutines ();

	}

	void OnEnable(){
		if (actualSource != null) {
			songDuration = mainAudio.clip.length;   //memorize the song-duration
            play();
            StartCoroutine (multimediaTimer ());
		}
	}

    void save()
    {
        SecurePlayerPrefs.SetInt(songIndexKey, actualIndex);
       // Debug.LogWarning("Save: " + actualIndex.ToString());
    }

    void load()
    {
        if (SecurePlayerPrefs.HasKey(songIndexKey))
        {
            actualIndex = SecurePlayerPrefs.GetInt(songIndexKey);
        }
       // Debug.LogWarning("load: " + actualIndex.ToString());
    }
}
