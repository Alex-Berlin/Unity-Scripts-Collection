using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RobustMusicPlayer : MonoBehaviour
{
    /* 
    This is a music player script wich allows you to easily play, 
    shuffle and repeat entire playlists 
    and tracks from playlists and control the music volume.

    You want to control Looping and PlayOnAwake using variables IN THE SCRIPT, not on the AudioSource component.

    Following functions are all public
    and meant to be called from other scripts
    or added as listeners to events as needed:
    
    Play() starts playing the current track.
    Play(int index) plays specific track in playlist.
    Play(AudioClip clip) plays specific track.
    PlayNext() plays next track in list, PlayLast() - plays previous.
    Restart() stops the current track and resets the playlist to the beggining.
    Shuffle() randomises playlist order when called.
    Stop() stops the current track, Pause() pauses it.
    */

    [Header("Setup")]
    public bool isRandomOrder = true;
    public bool isLooped = true;
    public bool isPlaying = false;
    private bool isPaused = false;
    AudioSource audioSource;
    [Header("Track List")]
    [SerializeField] List<AudioClip> playlist;
    public AudioClip addClip;
    AudioClip currentTrack;
    int currentTrackIndex = 0;

    private void Awake()
    {
        TryGetComponent<AudioSource>(out audioSource);
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.Stop();
        if (isRandomOrder)
            Shuffle();
    }

    /// <summary>
    /// Shuffles the playlist using Fisher–Yates shuffle.
    /// </summary>
    public void Shuffle()
    {
        // swaps each track in playlist with another random track.
        for (int i = 0; i < playlist.Count; i++)
        {
            int randomIndex = Random.Range(0, playlist.Count - 1);
            AudioClip cachedSong = playlist[randomIndex];
            playlist[randomIndex] = playlist[i];
            playlist[i] = cachedSong;
        }
    }
    /// <summary>
    /// Stops the music playing and goes back to the first track in playlist.
    /// </summary>
    public void Restart()
    {
        audioSource.Stop();
        isPaused = false;
        isPlaying = false;
        currentTrackIndex = 0;
    }
    /// <summary>
    /// Stops the player.
    /// </summary>
    public void Stop()
    {
        audioSource.Stop();
        isPlaying = false;
        isPaused = false;
    }
    /// <summary>
    /// Pauses the current track.
    /// </summary>
    public void Pause()
    {
        audioSource.Pause();
        isPlaying = false;
        isPaused = true;
    }
    
    /// <summary>
    /// Starts the player on current song.
    /// </summary>
    public void Play()
    {
        if (!isPaused)
        {
            audioSource.PlayOneShot(playlist[currentTrackIndex]);
        }
        else
        {
            audioSource.Play();
            isPaused = false;
        }
        isPlaying = true;
    }
    /// <summary>
    /// Starts the player on indexed song in playlist.
    /// </summary>
    /// <param name="index"></param>
    public void Play(int index)
    {
        audioSource.Stop();
        isPaused = false;
        isPlaying = true;
        currentTrackIndex = index;
        Play();
    }
    /// <summary>
    /// Starts the player.
    /// </summary>
    /// <param name="clip"></param>
    public void Play(AudioClip clip)
    {
        audioSource.Stop();
        isPaused = false;
        isPlaying = true;
        audioSource.PlayOneShot(clip);
    }
    /// <summary>
    /// Plays next song in playlist.
    /// </summary>
    public void PlayNext()
    {
        isPaused = false;
        audioSource.Stop();
        currentTrackIndex = currentTrackIndex == playlist.Count - 1 ? 0 : currentTrackIndex + 1;
        Play();
    }
    /// <summary>
    /// Plays previous song in playlist.
    /// </summary>
    public void PlayPrevious()
    {
        isPaused = false;
        audioSource.Stop();
        currentTrackIndex = currentTrackIndex == 0 ? playlist.Count - 1 : currentTrackIndex - 1;
        Play();
    }

    /// <summary>
    /// Adds the song to playlist.
    /// </summary>
    /// <param name="clip"></param>
    public void AddToPlaylist(AudioClip clip)
    {
        playlist.Add(clip);
    }
    /// <summary>
    /// Removes indexed song from playlist.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveFromPlaylist(int index)
    {
        if (currentTrackIndex == index)
        {
            if (isPlaying)
                PlayNext();
            else
                currentTrackIndex = currentTrackIndex == playlist.Count - 1 ? 0 : currentTrackIndex + 1;
        }
        playlist.RemoveAt(index);
    }

    /// <summary>
    /// Main logic is contained here.
    /// </summary>
    private void Controller()
    {
        if (playlist.Count <= 0)
        {
            Stop();
            return;
        }
        Mathf.Clamp(currentTrackIndex, 0, playlist.Count - 1);
        if (isPlaying && !audioSource.isPlaying)
        {
            currentTrackIndex++;
            if (currentTrackIndex >= playlist.Count)
            {
                if (isRandomOrder)
                    Shuffle();

                if (isLooped)
                    currentTrackIndex = 0;
                else
                    isPlaying = false;
            }
            currentTrack = playlist[currentTrackIndex];
            audioSource.PlayOneShot(currentTrack);
        }
    }

    /// <summary>
    /// Keyboard controls used for testing, you don't need those.
    /// </summary>
    private void TestingControls()
    {
        // Play|Stop on SPACE, Shuffle on S, Restart on R, Pause on P, 
        //Right|Left for PlayNext and Previous, 
        //1 and 2 for playing first and second song in list.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isPlaying)
                Stop();
            else
                Play();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Shuffle();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PlayNext();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlayPrevious();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Play(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Play(2);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddToPlaylist(addClip);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RemoveFromPlaylist(currentTrackIndex);
        }
    }

    private void Update()
    {
        Controller();
        TestingControls();
    }

    // TODO:
    // Pause functionality
    // Playlist removal indexing fix



}
