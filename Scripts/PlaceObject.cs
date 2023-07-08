using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    public static PlaceObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO)
    {
        Transform placeObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.Pegar_�ngulo_Rota��o(dir), 0));

        PlaceObject placeObject = placeObjectTransform.GetComponent<PlaceObject>();

        placeObject.placedObjectTypeSO = placedObjectTypeSO;
        placeObject.origin = origin;
        placeObject.dir = dir;
                
        return placeObject;


    }
    private PlacedObjectTypeSO placedObjectTypeSO;
    private Vector2Int origin;
    private PlacedObjectTypeSO.Dir dir;


    public List<Vector2Int> Pegar_Lista_Posi��o_Grade()
    {
        return placedObjectTypeSO.Pegar_Lista_Posi��o_Grade(origin, dir);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
