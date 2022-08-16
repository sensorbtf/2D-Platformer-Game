using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager 
{
	readonly private AudioSource _musicSource;
	readonly private AudioSource _playerEffectsSource;
	readonly private AudioSource _playerWalkingSource;
	readonly private AudioSource _enemyEffectsSource;
	readonly private AudioSource _enviromentEffectSource;

	public AudioSource musicSource => _musicSource;
	public AudioSource playerSource => _playerEffectsSource;
	public AudioSource playerWalkingSource => _playerWalkingSource;
	public AudioSource enemyEffectsSource => _enemyEffectsSource;
	public AudioSource enviromentEffectSource => _enviromentEffectSource;

    public SoundManager(AudioSource music, AudioSource playereffects, AudioSource playerwalking, 
			AudioSource enemyeffects, AudioSource enviromenteffects)
	{
		_musicSource = music;
		_playerEffectsSource = playereffects;
		_playerWalkingSource = playerwalking;
		_enemyEffectsSource = enemyeffects;
		_enviromentEffectSource = enviromenteffects;
	}

	public void PlayMusic(AudioClip clip)
	{
		_musicSource.clip = clip;
		_musicSource.Play();
	}
	public void PlayWalkingEffect(AudioClip clip)
	{
		_playerWalkingSource.clip = clip;
		_playerWalkingSource.Play();
	}
	public void PlayPlayerEffects(AudioClip clip)
	{
		_playerEffectsSource.clip = clip;
		_playerEffectsSource.Play();
	}
	public void PlayEnemyEffects(AudioClip clip)
	{
		_enemyEffectsSource.clip = clip;
		_enemyEffectsSource.Play();
	}
	public void PlayEnviromentEffects(AudioClip clip)
	{
		_enviromentEffectSource.clip = clip;
		_enviromentEffectSource.Play();
	}
	public void MuteDespiteMusic()
	{
		_musicSource.loop = false;
		_playerEffectsSource.mute = true;
		_enemyEffectsSource.mute = true;
		_enviromentEffectSource.mute = true;
		_playerWalkingSource.mute = true;
	}

}
