using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintManager : MonoBehaviour
{
    public static PaintManager instance;

    [SerializeField] private Material paintableMaterial;

    //public static Dictionary<ushort, Vector3> paintLocations = new Dictionary<ushort, Vector3>();

    Texture2D generatedTexture;

    Renderer rend;

    private class SplitFloat
    {
        public int length => splitList.Count;

        public List<int> splitList;

        public int decimalIndex;

        public int sign = 1;

        public SplitFloat(float floatToSplit)
        {
            splitList = new List<int>();
            FloatToIntList(floatToSplit);
        }

        private void FloatToIntList(float value)
        {
            if (value < 0)
            {
                sign = 0;
                value *= -1;
            }

            string stringValue = value.ToString("F1");
            decimalIndex = stringValue.Length;
            for (int i = 0; i < stringValue.Length; i++)
            {
                if (stringValue[i] == '.')
                {
                    decimalIndex = i;
                }
                else
                {
                    splitList.Add(int.Parse(stringValue.Substring(i, 1)));
                }
            }
        }
    }

    private class SplitVector3
    {
        public int splitLength;

        public SplitFloat xPos;
        public SplitFloat yPos;
        public SplitFloat zPos;

        public SplitVector3(Vector3 vector3ToSplit)
        {
            xPos = new SplitFloat(vector3ToSplit.x);
            yPos = new SplitFloat(vector3ToSplit.y);
            zPos = new SplitFloat(vector3ToSplit.z);

            splitLength = Mathf.Max(new int[] { xPos.length, yPos.length, zPos.length });
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        rend = GetComponent<Renderer>();
        //GenerateTextureFromPositions();
    }

    private void Update()
    {
        //GenerateTextureFromPositions(); //todo:REMOVE FROM UPDATE, Have this change only when visual effects are added and removed
    }

    private void GenerateTextureFromPositions() //improve method for adding new paints. instead of recreating the whole texture, just remove/add pixels?
    {
        PositionalVisualEffect[] effects = VisualEffects.GetAllPositionalEffectsOfType(VisualEffectType.Positional_Ice);

        List<SplitVector3> splitLocations = new List<SplitVector3>();
        foreach (PositionalVisualEffect effect in effects)
        {
            splitLocations.Add(new SplitVector3(effect.position));
        }

        int fullMaxLength = 0;
        for (int i = 0; i < splitLocations.Count; i++)
        {
            if (splitLocations[i].splitLength > fullMaxLength)
            {
                fullMaxLength = splitLocations[i].splitLength;
            }
        }
        fullMaxLength += 2;

        generatedTexture = new Texture2D(fullMaxLength, splitLocations.Count);
        generatedTexture.filterMode = FilterMode.Point;

        for (int i = 0; i < splitLocations.Count; i++)
        {
            SplitVector3 splitLocation = splitLocations[i];
            Color[] colours = new Color[fullMaxLength];

            for (int j = 0; j < splitLocation.xPos.length; j++)
            {
                colours[j].r = (float)splitLocation.xPos.splitList[j] / 10;
            }

            for (int j = 0; j < splitLocation.yPos.length; j++)
            {
                colours[j].g = (float)splitLocation.yPos.splitList[j] / 10;
            }

            for (int j = 0; j < splitLocation.zPos.length; j++)
            {
                colours[j].b = (float)splitLocation.zPos.splitList[j] / 10;
            }

            colours[fullMaxLength - 1] = new Color(
                (float)splitLocation.xPos.decimalIndex / 10,
                (float)splitLocation.yPos.decimalIndex / 10,
                (float)splitLocation.zPos.decimalIndex / 10);

            colours[fullMaxLength - 2] = new Color(
                splitLocation.xPos.sign,
                splitLocation.yPos.sign,
                splitLocation.zPos.sign);

            generatedTexture.SetPixels(0, i, fullMaxLength, 1, colours);
        }
        generatedTexture.Apply();

        //DisplayProjectedValues(generatedTexture, new Vector2Int(generatedTexture.width, generatedTexture.height));

        rend.material.mainTexture = generatedTexture;

        paintableMaterial.SetTexture("PositionTexture", generatedTexture);
        paintableMaterial.SetVector("TextureSize", new Vector2(fullMaxLength, splitLocations.Count));
    }

    private Vector3 Sample(Texture2D texture, int x, int y, int multiplier = 10)
    {
        Color col = texture.GetPixel(x, y);
        return new Vector3(Mathf.FloorToInt(col.r * multiplier), Mathf.FloorToInt(col.g * multiplier), Mathf.FloorToInt(col.b * multiplier));
    }

    public void RegisterPosition(ushort id, Vector3 position)
    {
        /*if (!paintLocations.ContainsKey(id))
        {
            paintLocations.Add(id, position);
        }*/
    }

    public void UpdatePaint()
    {
        GenerateTextureFromPositions();
    }








    private void DisplayProjectedValues(Texture2D PositionTexture, Vector2Int TextureSize)
    {
        print("---------");
        for (int i = 0; i < TextureSize.y; i++)
        {
            Vector3 position = Sample(PositionTexture, 0, i);
            Vector3 decimalIndex = Sample(PositionTexture, TextureSize.x - 1, i);
            Vector3 sign = Sample(PositionTexture, TextureSize.x - 2, i, 1);

            //position *= 10;
            //decimalIndex *= 10;

            for (int j = 1; j < TextureSize.x - 2; j++)
            {
                Vector3 positionDigit = Sample(PositionTexture, j, i);
                //positionDigit *= 10f;

                position *= 10f;
                position += positionDigit;
            }
            position.x /= Mathf.Pow(10, TextureSize.x - 2 - decimalIndex.x);
            position.y /= Mathf.Pow(10, TextureSize.x - 2 - decimalIndex.y);
            position.z /= Mathf.Pow(10, TextureSize.x - 2 - decimalIndex.z);

            sign = (sign - new Vector3(0.5f, 0.5f, 0.5f)) * 2;

            position.x *= sign.x;
            position.y *= sign.y;
            position.z *= sign.z;

            Debug.Log(position);
        }
    }
}
