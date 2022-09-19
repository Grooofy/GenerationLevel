using System;
using UnityEngine;


public class VoxelTile : MonoBehaviour
{
    [SerializeField] public float VoxelSize = 0.1f;
    [SerializeField] public int TileSideVoxels = 8;

    [HideInInspector] public byte[] ColorRight;
    [HideInInspector] public byte[] ColorForward;
    [HideInInspector] public byte[] ColorLeft;
    [HideInInspector] public byte[] ColorBack;

    [Range(1,100)]   
    public int Weight = 50;

    public void CalculateSideColors()
    {
        ColorRight = new byte[TileSideVoxels * TileSideVoxels];
        ColorForward = new byte[TileSideVoxels * TileSideVoxels];
        ColorLeft = new byte[TileSideVoxels * TileSideVoxels];
        ColorBack = new byte[TileSideVoxels * TileSideVoxels];
        
        for (int y = 0; y < TileSideVoxels; y++)
        {
            for (int i = 0; i < TileSideVoxels; i++)
            {
                ColorRight[y * TileSideVoxels + i] = GetVoxelColor(y, i, Vector3.right);
                ColorForward[y * TileSideVoxels + i] = GetVoxelColor(y, i, Vector3.forward);
                ColorLeft[y * TileSideVoxels + i] = GetVoxelColor(y, i, Vector3.left);
                ColorBack[y * TileSideVoxels + i] = GetVoxelColor(y, i, Vector3.back);
            }
        }
    }

    private byte GetVoxelColor(int verticalLayer, int horizontalOffSet, Vector3 direcrion)
    {
        MeshCollider meshCollider = GetComponentInChildren<MeshCollider>();
        
        float vox = VoxelSize;
        float half = VoxelSize / 2;

        Vector3 rayStart ;
        
        if (direcrion == Vector3.right)
        {
            rayStart = meshCollider.bounds.min + 
                       new Vector3(-half, 0, half + horizontalOffSet * vox);
        }
        else if(direcrion == Vector3.forward)
        {
            rayStart = meshCollider.bounds.min +
                       new Vector3(half + horizontalOffSet * vox, 0, -half);
        }
        else if (direcrion == Vector3.left)
        {
            rayStart = meshCollider.bounds.max + 
                       new Vector3(half, 0, -half - (TileSideVoxels - horizontalOffSet -1) * vox);
        }
        else if (direcrion == Vector3.back)
        {
            rayStart = meshCollider.bounds.max +
                       new Vector3(-half - (TileSideVoxels - horizontalOffSet -1) * vox, 0, half);
        }
        else
        {
            throw new ArgumentException("Tou suck",  nameof(direcrion));
           
        }
        rayStart.y = meshCollider.bounds.min.y + half + verticalLayer * vox;

        if (Physics.Raycast(new Ray(rayStart, direcrion), out RaycastHit hit, VoxelSize))
        {
            Mesh mesh = meshCollider.sharedMesh;
            
            int hitTriangleVertex = mesh.triangles[hit.triangleIndex * 3];
            byte colorIndex = (byte)(mesh.uv[hitTriangleVertex].x * 256); 

            return colorIndex;
        }
        return 0;
    }
}