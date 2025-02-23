using System;
using UnityEngine;

[Serializable]
public class TransformData
{
    public Vector3Data Position;
    public QuaternionData Rotation;
    public Vector3Data Scale;

    public TransformData(Transform transform)
    {
        if (transform == null)
        {
            return;
        }

        Position = new Vector3Data(transform.position);
        Rotation = new QuaternionData(transform.rotation);
        Scale = new Vector3Data(transform.localScale);
    }

    public void ApplyToTransform(Transform transform)
    {
        if (transform == null)
        {
            return;
        }

        transform.position = Position.ToVector3();
        transform.rotation = Rotation.ToQuaternion();
        transform.localScale = Scale.ToVector3();
    }
}
