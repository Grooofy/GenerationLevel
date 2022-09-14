using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class VoxelTile : MonoBehaviour
{
    [SerializeField] private float _voxelSize = 0.1f;
    [SerializeField] private int _tileSideVoxels = 8;

    [HideInInspector] public byte[] ColorRight;
    [HideInInspector] public byte[] ColorForward;
    [HideInInspector] public byte[] ColorLeft;
    [HideInInspector] public byte[] ColorBack;

    private void Start()
    {
        ColorRight = new byte[_tileSideVoxels * _tileSideVoxels];
        ColorForward = new byte[_tileSideVoxels * _tileSideVoxels];
        ColorLeft = new byte[_tileSideVoxels * _tileSideVoxels];
        ColorBack = new byte[_tileSideVoxels * _tileSideVoxels];
        
        for (int y = 0; y < _tileSideVoxels; y++)
        {
            for (int i = 0; i < _tileSideVoxels; i++)
            {
                ColorRight[y * _tileSideVoxels + i] = GetVoxelColor(y, i, Vector3.right);
                ColorForward[y * _tileSideVoxels + i] = GetVoxelColor(y, i, Vector3.forward);
                ColorLeft[y * _tileSideVoxels + i] = GetVoxelColor(y, i, Vector3.left);
                ColorBack[y * _tileSideVoxels + i] = GetVoxelColor(y, i, Vector3.back);
            }
        }
        
        Debug.Log(string.Join(", ", ColorRight));
    }

    private byte GetVoxelColor(int verticalLayer, int horizontalOffSet, Vector3 direcrion)
    {
        MeshCollider meshCollider = GetComponentInChildren<MeshCollider>();
        
        float vox = _voxelSize;
        float half = _voxelSize / 2;

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
                       new Vector3(half, 0, -half - (_tileSideVoxels - horizontalOffSet -1) * vox);
        }
        else if (direcrion == Vector3.back)
        {
            rayStart = meshCollider.bounds.max +
                       new Vector3(-half - (_tileSideVoxels - horizontalOffSet -1) * vox, 0, half);
        }
        else
        {
            throw new ArgumentException("Tou suck",  nameof(direcrion));
           
        }

        rayStart.y = meshCollider.bounds.min.y + half + verticalLayer * vox;
       

        Debug.DrawRay(rayStart, direcrion * 0.1f, Color.red, 3);

        if (Physics.Raycast(new Ray(rayStart, direcrion), out RaycastHit hit, _voxelSize))
        {
            Mesh mesh = meshCollider.sharedMesh;
            
            int hitTriangleVertex = mesh.triangles[hit.triangleIndex * 3];
            byte colorIndex = (byte)(mesh.uv[hitTriangleVertex].x * 256); 

            return colorIndex;
        }

        return 0;
    }
}