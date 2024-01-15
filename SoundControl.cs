using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundControl : MonoBehaviour
{
    //Initializes the sliders to control volume
    [SerializeField] Slider musicSlider, sfxSlider;

    //Initializes the audio sources
    AudioSource musicAudio, sfxAudio;

    void Awake()
    {
        //Gets the music slider
        Slider b = musicSlider.GetComponent<Slider>();

        //Gets the sound effects slider
        Slider s = sfxSlider.GetComponent<Slider>();

        //Get the audio source on GameMangager 
        musicAudio = GameObject.Find("GameManager").GetComponent<AudioSource>();

        //Get the audio source on the player
        sfxAudio = GameObject.Find("Player").GetComponent<AudioSource>();
    }

    //Start is called before the first frame update
    void Start()
    {
        //Starts the music
        initialize();
    }

    //Change volume of music using value of musicSlider
    public void changeMusicVol()
    {
        musicAudio.volume = musicSlider.value;
        Save();
    }

    //Change volume of sound effects using value of sfxSlider
    public void changeSFXVol()
    {
        sfxAudio.volume = sfxSlider.value;
        Save();
    }

    //Mute both music and sound effects and doesn't save values
    public void mute()
    {
        musicAudio.volume = 0;
        sfxAudio.volume = 0;
    }

    //Sets music and sound effects to its maximum volume
    private void initialize()
    {
        //If player prefs doesn't have a value for music volume ...
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            //Set volume to max
            PlayerPrefs.SetFloat("musicVolume", 1);
        }

        //If player prefs doesn't have a value for sfx volume ...
        if (!PlayerPrefs.HasKey("sfxVolume"))
        {
            //Set volume to max
            PlayerPrefs.SetFloat("sfxVolume", 1);
        }

        //Sets volume to player prefs value
        Load();

        //Sets the slider values to the audio volume
        musicSlider.value = musicAudio.volume;
        sfxSlider.value = sfxAudio.volume;
    }

    //Sets the volume to the value of the player preferences
    private void Load()
    {
        musicAudio.volume = PlayerPrefs.GetFloat("musicVolume");
        sfxAudio.volume = PlayerPrefs.GetFloat("sfxVolume");
    }

    //Saves the volume into player prefs
    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", musicAudio.volume);
        PlayerPrefs.SetFloat("sfxVolume", sfxAudio.volume);
    }

    //Restarts the volume settings
    public void restart()
    {
        initialize();
    }
}