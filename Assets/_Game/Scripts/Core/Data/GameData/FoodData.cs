using System;
using System.Collections.Generic;

[Serializable]
public class FoodData
{
    public List<TransformData> FoodTransforms;

    public FoodData(List<TransformData> foodTransforms)
    {
        FoodTransforms = foodTransforms;
    }
}
