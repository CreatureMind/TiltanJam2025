using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    List<AudioSourceData> audioSources = new();

    protected override void Awake()
    {
        base.Awake();
        gameObject.AddComponent<AudioListener>();
    }

    public AudioSourceData AddSource(string sourceName, AudioClip clip = null)
    {
        if (!"<SFX>".Equals(sourceName))
        {
            foreach (var source in audioSources)
            {
                if (source.CompareName(sourceName)) return null;
            }
        }

        var newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = clip;
        newSource.playOnAwake = false;

        AudioSourceData data = new(newSource, sourceName);

        audioSources.Add(data);

        return data;
    }

    public void RemoveSource(string sourceName)
    {
        foreach (var source in audioSources)
        {
            if (source.CompareName(sourceName))
            {
                RemoveSource(source);
                return;
            }
        }
    }

    public void RemoveSource(AudioSourceData sourceData)
    {
        if (!audioSources.Contains(sourceData)) return;

        audioSources.Remove(sourceData);
        Destroy(sourceData.source);
    }

    public AudioSource GetSource(string sourceName)
    {
        foreach (var source in audioSources)
        {
            if (source.CompareName(sourceName)) return source.source;
        }

        return null;
    }

    public AudioSource PlaySFX(AudioClip clip)
    {
        if (clip == null) return null;

        var source = AddSource("<SFX>", clip);
        source.source.Play();
        StartCoroutine(TerminateAfterRuntime(source));
        return source.source;
    }

    IEnumerator TerminateAfterRuntime(AudioSourceData source)
    {
        yield return new WaitForSeconds(source.source.clip.length);

        RemoveSource(source);
    }

    public void RemoveAllSouces()
    {
        audioSources.ForEach(source =>
        {
            source.source.Stop();
            Destroy(source.source);
        });
        audioSources.Clear();
    }
}

[System.Serializable]
public class AudioSourceData
{
    public AudioSource source { get; private set; }

    private string name;

    public AudioSourceData(AudioSource _source, string _name) => (source, name) = (_source, _name);

    public bool CompareName(string other) => name.Equals(other);
}
