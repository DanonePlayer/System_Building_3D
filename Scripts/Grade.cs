using UnityEngine;
using CodeMonkey.Utils;
using System;

public class Grade<TGridObject>
{
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int z;
    }

    private int largura;
    private int altura;
    private Vector3 originPosition;
    private float tamanho_celula;
    private TGridObject[,] matriz_Grade;

    public Grade(int largura, int altura, float tamanho_celula, Vector3 originPosition, Func<Grade<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.largura = largura;
        this.altura = altura;
        this.tamanho_celula = tamanho_celula;
        this.originPosition = originPosition;

        matriz_Grade = new TGridObject[largura, altura];

        for (int x = 0; x < matriz_Grade.GetLength(0); x++)
        {
            for (int z = 0; z < matriz_Grade.GetLength(1); z++)
            {
                matriz_Grade[x, z] = createGridObject(this, x, z);
            }
        }


        bool ShowDebug = true;
        if(ShowDebug)
        {
            TextMesh[,] debugTextMatriz = new TextMesh[largura, altura];

            for (int x = 0; x < matriz_Grade.GetLength(0); x++)
            {
                for (int z = 0; z < matriz_Grade.GetLength(1); z++)
                {
                    debugTextMatriz[x, z] = UtilsClass.CreateWorldText(matriz_Grade[x, z]?.ToString(), null, PegarPosiçãoMundial(x, z) + new Vector3(tamanho_celula, 0, tamanho_celula) * .5f, 30, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                    debugTextMatriz[x, z].transform.localScale = Vector3.one * .30f;
                    debugTextMatriz[x, z].transform.eulerAngles = new Vector3(90, 0, 0);
                    Debug.DrawLine(PegarPosiçãoMundial(x, z), PegarPosiçãoMundial(x, z + 1), Color.white, 100f);    
                    Debug.DrawLine(PegarPosiçãoMundial(x, z), PegarPosiçãoMundial(x + 1, z), Color.white, 100f);
                }
            }
            Debug.DrawLine(PegarPosiçãoMundial(0, altura), PegarPosiçãoMundial(largura, altura), Color.white, 100f);
            Debug.DrawLine(PegarPosiçãoMundial(largura, 0), PegarPosiçãoMundial(largura, altura), Color.white, 100f);

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                debugTextMatriz[eventArgs.x, eventArgs.z].text = matriz_Grade[eventArgs.x, eventArgs.z]?.ToString();
            };
        }
    }

    public int PegarLargura()
    {
        return largura;
    }

    public int PegarAltura()
    {
        return altura;
    }

    public float PegarTamanhoDaCelula()
    {
        return tamanho_celula;
    }

    public Vector3 PegarPosiçãoMundial(int x, int z)
    {
        return new Vector3(x, 0, z) * tamanho_celula + originPosition;
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / tamanho_celula);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / tamanho_celula);
    }

    public void DefinirValor(int x, int z, TGridObject value)
    {
        if (x >= 0 && z >= 0 && x < largura && z < altura)
        {
            matriz_Grade[x, z] = value;
            TriggerGridObjectChanged(x, z);
        }
    }

    public void TriggerGridObjectChanged(int x, int z)
    {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, z = z });
    }

    public void DefinirValor(Vector3 worldPosition, TGridObject value)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        DefinirValor(x, z, value);
    }

    public TGridObject Pegar_Valor(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < largura && z < altura)
        {
            return matriz_Grade[x, z];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject Pegar_Valor(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return Pegar_Valor(x, z);
    }

    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        return new Vector2Int(
            Mathf.Clamp(gridPosition.x, 0, largura - 1),
            Mathf.Clamp(gridPosition.y, 0, altura - 1)
        );
    }

    /*
    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        int x = gridPosition.x;
        int z = gridPosition.y;

        if (x >= 0 && z >= 0 && x < largura && z < altura)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsValidGridPositionWithPadding(Vector2Int gridPosition)
    {
        Vector2Int padding = new Vector2Int(2, 2);
        int x = gridPosition.x;
        int z = gridPosition.y;

        if (x >= padding.x && z >= padding.y && x < largura - padding.x && z < altura - padding.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    */
}

