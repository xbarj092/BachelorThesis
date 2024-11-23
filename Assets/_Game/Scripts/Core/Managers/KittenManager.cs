using System.Collections.Generic;
using UnityEngine;

public class KittenManager : MonoSingleton<KittenManager>
{
    [SerializeField] private Kitten _kittenPrefab;
    
    public List<Kitten> Kittens = new();
    public Transform SpawnTransform;

    public void CreateKitten(Vector3 position, bool? male = null)
    {
        Kitten kitten = Instantiate(_kittenPrefab, position, Quaternion.identity, SpawnTransform);
        if (male != null)
        {
            kitten.Male = male.Value;
        }
        else
        {
            kitten.Male = DetermineKittenGender();
        }

        Kittens.Add(kitten);
    }

    private bool DetermineKittenGender()
    {
        return Random.Range(0, 2) == 0;
    }
}
