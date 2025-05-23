using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFurnitureLayout", menuName = "Room Layout/Furniture Layout")]
public class FurnitureLayout : ScriptableObject
{
    [System.Serializable]
    public struct FurnitureItem
    {
        public RoomFurniturePlacer.FurnitureType type;
        public Vector3 position;
        public Vector3 rotationEuler;
    }

    public List<FurnitureItem> items;
}

