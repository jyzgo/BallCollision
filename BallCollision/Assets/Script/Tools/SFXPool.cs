using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTUnity;
public class SFXPool : Singleton<SFXPool>
{
    ComponentPool<AudioSource> _audioSouces;
    public void Init(int count)
    {

        GameObject gb = new GameObject("SFX");
        gb.transform.parent = transform;
        var s = gb.AddComponent<AudioSource>();
        _audioSouces = new ComponentPool<AudioSource>(count, gb, transform);
        _audioSouces.Retrive(s);
    }

    public void PlayClip(AudioClip clip, float retriveTime = 1f)
    {
       var source=  _audioSouces.GetUnusedOne();
        source.clip = clip;
    }

    public AudioSource GetUnuseSouce()
    {
        var source=  _audioSouces.GetUnusedOne();
        source.pitch = 1f;
        return source;
    }

    public void RetriveSource(AudioSource source,float t)
    {
        StartCoroutine(IRetrive(source, t));
    }

    IEnumerator IRetrive(AudioSource source, float t)
    {
        yield return new WaitForSeconds(t);
        source.Stop();
        _audioSouces.Retrive(source);
    }

   
}
