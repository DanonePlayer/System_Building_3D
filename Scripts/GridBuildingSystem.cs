using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UIElements;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance { get; private set; }


    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList;
    private PlacedObjectTypeSO placedObjectTypeSO;

    private Grade<GridObject> grid;
    private PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;

    private void Awake()
    {
        Instance = this;

        int gridWidth = 10;
        int gridHeight = 10;
        float cellSize = 10f;
        grid = new Grade<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (Grade<GridObject> g, int x, int z) => new GridObject(g, x, z));

        placedObjectTypeSO = null; // placedObjectTypeSOList[0]; 
    }



    public class GridObject
    {
        private Grade<GridObject> grid;
        private int x;
        private int z;
        private PlaceObject placeObject;

        public GridObject(Grade<GridObject> grid , int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetPlacedObject(PlaceObject placeObject)
        {
            this.placeObject = placeObject;
            grid.TriggerGridObjectChanged(x, z);
        }

        public PlaceObject GetPlacedObject()
        {
            return placeObject;
        }

        public void ClearPlacedObject()
        {
            placeObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild()
        {
            return placeObject == null;
        }

        public override string ToString()
        {
            return x + "," + z + "\n" + placeObject;
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            grid.GetXZ(Mouse3D.GetMouseWorldPosition(), out int x, out int z);

            List<Vector2Int> gridPositionList = placedObjectTypeSO.Pegar_Lista_Posição_Grade(new Vector2Int(x, z), dir);

            //Test Can build 
            bool canBuld = true;
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                if(!grid.Pegar_Valor(gridPosition.x, gridPosition.y).CanBuild())
                {
                    //Cannot Build here
                    canBuld = false;
                    break;
                }
            }
    
            if(canBuld)
            {
                Vector2Int rotationOffset = placedObjectTypeSO.Pegar_Deslocamento_Rotação(dir);
                Vector3 placedObjectWorldPosition = grid.PegarPosiçãoMundial(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.PegarTamanhoDaCelula();

                PlaceObject placeObject =  PlaceObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), dir, placedObjectTypeSO);

                foreach(Vector2Int gridPosition in gridPositionList)
                {
                    grid.Pegar_Valor(gridPosition.x, gridPosition.y).SetPlacedObject(placeObject);
                }
            }
            else
            {
                UtilsClass.CreateWorldTextPopup("Não é possível construir aqui!", Mouse3D.GetMouseWorldPosition());
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            GridObject gridObject =  grid.Pegar_Valor(Mouse3D.GetMouseWorldPosition());
            PlaceObject placeObject = gridObject.GetPlacedObject();
            if(placeObject != null)
            {
                placeObject.DestroySelf();

                List<Vector2Int> gridPositionList = placeObject.Pegar_Lista_Posição_Grade(); 

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.Pegar_Valor(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }
            }
        }   

        Botãopress();
        RotaçãoObj();
    }


    private void RotaçãoObj()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
            //UtilsClass.CreateWorldTextPopup("" + dir, Mouse3D.GetMouseWorldPosition());
        }
    }


    private void Botãopress()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { placedObjectTypeSO = placedObjectTypeSOList[3]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { placedObjectTypeSO = placedObjectTypeSOList[4]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { placedObjectTypeSO = placedObjectTypeSOList[5]; RefreshSelectedObjectType(); }

        if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }

        /*
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 10)
            {
                placedObjectTypeSO = placedObjectTypeSOList[number - 1];
                RefreshSelectedObjectType();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }
        */
    }

    private void DeselectObjectType()
    {
        placedObjectTypeSO = null; RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    public Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        grid.GetXZ(mousePosition, out int x, out int z);

        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.Pegar_Deslocamento_Rotação(dir);
            Vector3 placedObjectWorldPosition = grid.PegarPosiçãoMundial(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.PegarTamanhoDaCelula();
            return placedObjectWorldPosition;
        }
        else
        {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation()
    {
        if (placedObjectTypeSO != null)
        {
            return Quaternion.Euler(0, placedObjectTypeSO.Pegar_Ângulo_Rotação(dir), 0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return placedObjectTypeSO;
    }
}
