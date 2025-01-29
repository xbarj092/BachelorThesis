using System;
using System.Collections.Generic;

[Serializable]
public class MapLayout
{
    public List<TransformData> RoomWallTransforms;
    public List<TransformData> HallwayWallTransforms;
    public List<TransformData> RoomFloorTransforms;
    public List<TransformData> HallwayFloorTransforms;

    public MapLayout(List<TransformData> roomWallTransforms, 
        List<TransformData> hallwayWallTransforms, 
        List<TransformData> roomFloorTransforms, 
        List<TransformData> hallwayFloorTransforms)
    {
        RoomWallTransforms = roomWallTransforms;
        HallwayWallTransforms = hallwayWallTransforms;
        RoomFloorTransforms = roomFloorTransforms;
        HallwayFloorTransforms = hallwayFloorTransforms;
    }
}
