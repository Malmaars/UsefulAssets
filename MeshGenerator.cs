using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.U2D;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(CanvasRenderer))]
public class MeshGenerator : MaskableGraphic
{
    public Texture texture;
    public Sprite sprite;
    public bool debugGridOn;

    public Vector2Int ratio;
    public int verticesMultiplier;
    public Vector3[] vertices = new Vector3[8];
    public int[] triangles = new int[18];
    public Vector2[] uv = new Vector2[8];


    public override Texture mainTexture
    {
        get
        {
            if(debugGridOn)
                return sprite.texture != null ? sprite.texture : null;
            return texture != null ? texture: null;
        }
    }
    protected override void Awake()
    {
        base.Awake();
    }
    private void Update()
    {
        SetAllDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        int newYRatio = ratio.y;
        int newXRatio = ratio.x;

        if (verticesMultiplier > 1)
        {
            newYRatio = ratio.y * (verticesMultiplier) - (verticesMultiplier - 1) ;
            newXRatio = ratio.x * (verticesMultiplier) - (verticesMultiplier - 1) ;
        }

        int yIndex = 0;
        int xIndex = 0;

        float yDivider = 0;
        for (int p = 0; p < newYRatio; p++)
        {
            for (int i = 0; i < newXRatio; i++)
            {
                float x = 0;
                float y = 0;
                float uvX = 0;
                float uvY = 1;

                if (i == 0)
                    xIndex = 0;
                else if (i % (newXRatio - 1) == 0)
                {
                    uvX = 1;
                    xIndex = 1;
                }


                //find the index, reverse formula
                if (p == 0)
                    yIndex = 0;

                else if (p % verticesMultiplier == 0)
                {
                    yIndex = ratio.x * (p / verticesMultiplier);
                    uvY = 0;
                    yDivider = 0;
                }

                int index = xIndex + yIndex;

                if (i == 0 || xIndex == 1)
                {
                    if (index + ratio.x + 1 < vertices.Length)
                    {
                        x = vertices[index].x + ((vertices[index + ratio.x].x - vertices[index].x) * (yDivider / (float)verticesMultiplier));
                    }
                    else
                        x = vertices[index].x;
                    uvX = uv[index].x;
                }
                else
                {
                    if (index + 1 + ratio.x > vertices.Length)
                        x = vertices[index].x + ((vertices[index + 1].x - vertices[index].x) * ((float)i / (float)verticesMultiplier));
                    else
                    {
                        float startPos = vertices[index].x + ((vertices[index + ratio.x].x - vertices[index].x) * (yDivider / (float)verticesMultiplier));
                        float endPos = vertices[index + 1].x + ((vertices[index + 1 + ratio.x].x - vertices[index + 1].x) * (yDivider / (float)verticesMultiplier));
                        x = startPos + ((endPos - startPos) * ((float)i / (float)verticesMultiplier));
                    }
                    uvX = uv[index].x + ((uv[index + 1].x - uv[index].x) * ((float)i / (float)verticesMultiplier));
                }
                if (p == 0 || index + ratio.x >= vertices.Length)
                {
                    y = vertices[index].y;
                    uvY = uv[index].y;
                }
                else
                {
                    y = vertices[index].y + ((vertices[index + ratio.x].y - vertices[index].y) * (yDivider / (float)verticesMultiplier));
                    uvY = uv[index].y + ((uv[index + ratio.x].y - uv[index].y) * (yDivider / (float)verticesMultiplier));
                }
                Vector3 vertex = new Vector3(x, y, 0);
                Vector2 endUv = new Vector2(uvX, uvY);

                //just calculate the position relative to the scene?
                vh.AddVert(vertex, color, endUv);
            }
            yDivider++;
        }

        //calculate triangles
        int triangleIndex = 0;
        for (int p = 0; p < newYRatio - 1; p++)
        {
            for (int i = 0; i < newXRatio; i++)
            {
                if (i != newXRatio - 1)
                {
                    int topLeft = triangleIndex;
                    int topRight = triangleIndex + 1;
                    int bottomLeft = triangleIndex + newXRatio;
                    int bottomRight = triangleIndex + newXRatio + 1;

                    vh.AddTriangle(topLeft, topRight, bottomLeft);
                    vh.AddTriangle(topRight, bottomRight, bottomLeft);
                }
                triangleIndex++;
            }
        }



    }
}
