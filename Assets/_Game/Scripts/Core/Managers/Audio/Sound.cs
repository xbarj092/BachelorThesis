using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound 
{
    public SoundType Name;
    public SoundGroup Group;

    public AudioClip Clip;

    public float SpatialBlend;
    public int NumberOfSource = 1;

    [Range(0f, 1f)] public float Volume;
    [Range(0.1f, 3f)] public float Pitch;
    public bool VariablePitch;

    [HideInInspector] public List<AudioSource> Source;
}
