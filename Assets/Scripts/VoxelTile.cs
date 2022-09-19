using System;
using UnityEngine;


public class VoxelTile : MonoBehaviour
{
    [SerializeField] public float VoxelSize = 0.1f;
    [SerializeField] public int TileSideVoxels = 8;

    [Range(1,100)]   
    public int Weight = 50;

    private float _voxelHalf;
    private MeshCollider _meshChildrenCollider;
    private Vector3 _rayStart;

    private void Awake()
    {
        _voxelHalf = VoxelSize / 2;
        _meshChildrenCollider = GetComponentInChildren<MeshCollider>();
    }

    public void CalculateSideColors(out byte[] colorRight, out byte[] colorLeft, out byte[] colorForward, out byte[] colorBack) 
    {
        colorRight = new byte[TileSideVoxels * TileSideVoxels];
        colorForward = new byte[TileSideVoxels * TileSideVoxels];
        colorLeft = new byte[TileSideVoxels * TileSideVoxels];
        colorBack = new byte[TileSideVoxels * TileSideVoxels];
        
        for (int y = 0; y < TileSideVoxels; y++)
        {
            for (int i = 0; i < TileSideVoxels; i++)
            {
                colorRight[y * TileSideVoxels + i] = GetVoxelColorRightSide(y, i);
                colorForward[y * TileSideVoxels + i] = GetVoxelColorForwardSide(y, i);
                colorLeft[y * TileSideVoxels + i] = GetVoxelColorLeftSide(y, i);
                colorBack[y * TileSideVoxels + i] = GetVoxelColorBackSide(y, i);
            }
        }
    }

    private byte GetVoxelColorBackSide(int verticalLayer, int horizontalOffSet)
    {
        Vector3 direction = Vector3.back;

        _rayStart = _meshChildrenCollider.bounds.max +
                       new Vector3(-_voxelHalf - (TileSideVoxels - horizontalOffSet - 1) * VoxelSize, 0, _voxelHalf);

        SpecifyVerticalPositionRaycastHit(_rayStart, _meshChildrenCollider, verticalLayer);

        return TryDropeRaycastHit(_rayStart, direction);
    }

    private byte GetVoxelColorForwardSide(int verticalLayer, int horizontalOffSet)
    {
        Vector3 direction = Vector3.forward;

        _rayStart = _meshChildrenCollider.bounds.min +
                      new Vector3(_voxelHalf + horizontalOffSet * VoxelSize, 0, -_voxelHalf);

        SpecifyVerticalPositionRaycastHit(_rayStart, _meshChildrenCollider, verticalLayer);

        return TryDropeRaycastHit(_rayStart, direction);

    }

    private byte GetVoxelColorLeftSide(int verticalLayer, int horizontalOffSet)
    {
        Vector3 direction = Vector3.left;

        _rayStart = _meshChildrenCollider.bounds.max +
                      new Vector3(_voxelHalf, 0, -_voxelHalf - (TileSideVoxels - horizontalOffSet - 1) * VoxelSize);

        SpecifyVerticalPositionRaycastHit(_rayStart, _meshChildrenCollider, verticalLayer);

        return TryDropeRaycastHit(_rayStart, direction);
    }

    private byte GetVoxelColorRightSide(int verticalLayer, int horizontalOffSet)
    {
        Vector3 direction = Vector3.right;

        _rayStart = _meshChildrenCollider.bounds.min +
                       new Vector3(-_voxelHalf, 0, _voxelHalf + horizontalOffSet * VoxelSize);

        SpecifyVerticalPositionRaycastHit(_rayStart, _meshChildrenCollider, verticalLayer);

        return TryDropeRaycastHit(_rayStart, direction);
    }

    private void SpecifyVerticalPositionRaycastHit(Vector3 rayStart, MeshCollider meshCollider, int verticalLayer) => rayStart.y = meshCollider.bounds.min.y + _voxelHalf + verticalLayer * VoxelSize;

    private byte TryDropeRaycastHit(Vector3 rayStart, Vector3 direction) 
    {
        int triangleCount = 3;
        int maxNumberColor = 256;

        if (Physics.Raycast(new Ray(rayStart, direction), out RaycastHit hit, VoxelSize))
        {
            Mesh mesh = _meshChildrenCollider.sharedMesh;

            int hitTriangleVertex = mesh.triangles[hit.triangleIndex * triangleCount];

            byte colorIndex = (byte)(mesh.uv[hitTriangleVertex].x * maxNumberColor);

            return colorIndex;
        }
        return 0;
    }
}