using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
	private readonly AudioSource _musicSource;
	private readonly AudioSource _playerEffectsSource;
	private readonly AudioSource _playerWalkingSource;
	private readonly AudioSource _enemyEffectsSource;
	private readonly AudioSource _enviromentEffectSource;

	public AudioSource MusicSource => _musicSource;
	public AudioSource PlayerSource => _playerEffectsSource;
	public AudioSource PlayerWalkingSource => _playerWalkingSource;
	public AudioSource EnemyEffectsSource => _enemyEffectsSource;
	public AudioSource EnviromentEffectSource => _enviromentEffectSource;


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
		EnviromentEffectSource.mute = true;
		_playerWalkingSource.mute = true;
	}

}
