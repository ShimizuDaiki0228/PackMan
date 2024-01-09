using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static SingletonMonobehaviour;

public enum SFX
{
    BUTTONCLICK,
    SLIDEANIMATION,
    SPACEKEYCLICK
}

public enum BGM
{
    MAIN,
    RULE,
    GAMEOVER
}

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField] private AudioSource[] _sfx;
    [SerializeField] private AudioSource[] _bgm;

    private int _currentSFXIndex;
    private int _currentBGMIndex;

    /// <summary>
    /// SFX‚ð–Â‚ç‚·
    /// </summary>
    /// <param name="sfxIndex"></param>
    public void PlaySFX(int sfxIndex)
    {
        if (sfxIndex < _sfx.Length)
        {
            _sfx[sfxIndex].Play();
            _currentSFXIndex = sfxIndex;
        }
    }

    /// <summary>
    /// SFX‚ðŽ~‚ß‚é
    /// </summary>
    public void StopSFX()
    {
        _sfx[_currentSFXIndex].Stop();
    }

    /// <summary>
    /// BGM‚ð—¬‚·
    /// </summary>
    /// <param name="bgmIndex"></param>
    public void PlayBGM(int bgmIndex)
    {
        _currentBGMIndex = bgmIndex;

        if (bgmIndex < _bgm.Length)
        {
            _bgm[bgmIndex].Play();
        }
    }

    /// <summary>
    /// BGM‚ð‚·‚®‚ÉŽ~‚ß‚é
    /// </summary>
    public void StopBGM()
    {
        _bgm[_currentBGMIndex].Stop();
    }
}
