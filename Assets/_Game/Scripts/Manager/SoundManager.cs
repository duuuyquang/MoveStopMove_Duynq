using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip btnClick;
    [SerializeField] AudioClip btnClick2;
    [SerializeField] AudioClip count1;
    [SerializeField] AudioClip count2;
    [SerializeField] AudioClip count3;
    [SerializeField] AudioClip sizeUp1;
    [SerializeField] AudioClip sizeUp2;
    [SerializeField] AudioClip victory;
    [SerializeField] AudioClip lose;
    [SerializeField] AudioClip electricShock;
    [SerializeField] AudioClip hitBooster;
    [SerializeField] AudioClip[] deads;
    [SerializeField] AudioClip[] throwWeapons;
    [SerializeField] AudioClip[] weaponHits;

    public float Volume => audioSource.volume;

    private void OnPlay(AudioClip sound)
    {
        audioSource.Stop();
        audioSource.clip = sound;
        audioSource.Play();
    }

    public void PlayBtnClick()
    {
        OnPlay(btnClick);
    }

    public void PlayBtnClickError()
    {
        OnPlay(btnClick2);
    }

    public void PlayCount1()
    {
        OnPlay(count1);
    }

    public void PlayCount2()
    {
        OnPlay(count2);
    }

    public void PlayCount3()
    {
        OnPlay(count3);
    }

    public void PlayVictory()
    {
        OnPlay(victory);
    }

    public void PlayLose()
    {
        OnPlay(lose);
    }

    public void PlayHitBooster()
    {
        OnPlay(hitBooster);
    }

    public void PlayDead(AudioSource source)
    {
        source.Stop();
        source.clip = deads[Random.Range(0, deads.Length)];
        ChangeVolumeAndPlay(source);
    }

    public void PlayThrowWeapon(AudioSource source, bool isReturn = false)
    {
        source.Stop();
        source.clip = throwWeapons[1];
        if(isReturn)
        {
            source.clip = throwWeapons[0];
        }
        ChangeVolumeAndPlay(source);
    }

    public void PlaySizeUp1(AudioSource source)
    {
        source.Stop();
        source.clip = sizeUp1;
        ChangeVolumeAndPlay(source);
    }

    public void PlaySizeUp2(AudioSource source)
    {
        source.Stop();
        source.clip = sizeUp2;
        ChangeVolumeAndPlay(source);
    }

    public void PlayWeaponHit(AudioSource source)
    {
        source.Stop();
        source.clip = weaponHits[Random.Range(0, weaponHits.Length)];
        ChangeVolumeAndPlay(source);
    }

    public void PlayAtkBoosterEffect(AudioSource source)
    {
        source.Stop();
        source.clip = electricShock;
        ChangeVolumeAndPlay(source);
    }

    public void PlaySpeedBoosterEffect(AudioSource source)
    {
        source.Stop();
        source.clip = electricShock;
        ChangeVolumeAndPlay(source);
    }

    public void SetVolume(float value)
    {
        audioSource.volume = value;
    }

    private void ChangeVolumeAndPlay(AudioSource source)
    {
        source.volume = Volume;
        source.Play();
    }
}
