using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager
{
    private AudioSource _mainSfx;
    private AudioClip _dash;
    private AudioClip _boom;
    private AudioClip _win;
    private AudioClip _lose;
    private AudioClip _exitOpen;
    private AudioClip _playerHit;

    public SfxManager()
    {
        _mainSfx = GameManager.singleton.ResourcesLoaderManager.MainSFX;
        _dash = GameManager.singleton.ResourcesLoaderManager.Dash;
        _boom = GameManager.singleton.ResourcesLoaderManager.Boom;
        _win = GameManager.singleton.ResourcesLoaderManager.Win;
        _lose = GameManager.singleton.ResourcesLoaderManager.Lose;
        _exitOpen = GameManager.singleton.ResourcesLoaderManager.ExitOpen;
        _playerHit = GameManager.singleton.ResourcesLoaderManager.PlayerHit;
    }

    public void PlayDash()
    {
        _mainSfx.PlayOneShot(_dash);
    }

    public void PlayBoom()
    {
        _mainSfx.PlayOneShot(_boom);
    }
    public void PlayWin()
    {
        _mainSfx.PlayOneShot(_win);
    }

    public void PlayLose()
    {
        _mainSfx.PlayOneShot(_lose);
    }

    public void PlayExitOpen()
    {
        _mainSfx.PlayOneShot(_exitOpen);
    }

    public void Playhit()
    {
        _mainSfx.PlayOneShot(_playerHit);
    }



}
