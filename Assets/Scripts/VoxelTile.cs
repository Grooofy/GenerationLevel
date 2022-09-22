using System;
using System.Collections.Generic;
using UnityEngine;


public class VoxelTile : MonoBehaviour
{
    [SerializeField] public float VoxelSize = 0.1f;
    [SerializeField] public int TileSideVoxels = 8;

    [Range(1, 100)]
    public int Weight = 50;

    //[HideInInspector] public byte[] ColorRight;
    //[HideInInspector] public byte[] ColorLeft;
    //[HideInInspector] public byte[] ColorForward;
    [HideInInspector] public byte[] ColorBack;

    private List<byte> _colorRight = new List<byte>();
    private List<byte> _colorLeft = new List<byte>();
    private List<byte> _colorForward = new List<byte>();
    private List<byte> _colorBack = new List<byte>();

    public IReadOnlyCollection<byte> ColorRight => _colorRight;
    public IReadOnlyCollection<byte> ColorForward => _colorForward;
    public IReadOnlyCollection<byte> ColorLeft => _colorLeft;

    private float _voxelHalf = 0.05f;
    private MeshCollider _meshChildrenCollider;
    private Vector3 _rayStart;


    private void OnEnable()
    {
        _meshChildrenCollider = GetComponentInChildren<MeshCollider>();
    }

    public void CalculateSideColors()
    {
        //ColorLeft = new byte[TileSideVoxels * TileSideVoxels];
        ColorBack = new byte[TileSideVoxels * TileSideVoxels];

        for (int y = 0; y < TileSideVoxels; y++)
        {
            for (int i = 0; i < TileSideVoxels; i++)
            {
                _colorRight.Add(GetVoxelColorRightSide(y, i));
                _colorForward.Add(GetVoxelColorForwardSide(y, i));
                _colorLeft.Add(GetVoxelColorLeftSide(y, i));
                ColorBack[y * TileSideVoxels + i] = GetVoxelColorBackSide(y, i);
            }
        }
    }

    private byte GetVoxelColorBackSide(int verticalLayer, int horizontalOffSet)
    {
        Vector3 direction = Vector3.back;

        _rayStart = _meshChildrenCollider.bounds.max +
                       new Vector3(-_voxelHalf - (TileSideVoxels - horizontalOffSet - 1) * VoxelSize, 0, _voxelHalf);

        _rayStart.y = SpecifyVerticalPositionRaycastHit(_rayStart, verticalLayer);

        return TryDropeRaycastHit(_rayStart, direction);
    }

    private byte GetVoxelColorForwardSide(int verticalLayer, int horizontalOffSet)
    {
        Vector3 direction = Vector3.forward;

        _rayStart = _meshChildrenCollider.bounds.min +
                      new Vector3(_voxelHalf + horizontalOffSet * VoxelSize, 0, -_voxelHalf);

        _rayStart.y = SpecifyVerticalPositionRaycastHit(_rayStart, verticalLayer);

        return TryDropeRaycastHit(_rayStart, direction);

    }

    private byte GetVoxelColorLeftSide(int verticalLayer, int horizontalOffSet)
    {
        Vector3 direction = Vector3.left;

        _rayStart = _meshChildrenCollider.bounds.max +
                      new Vector3(_voxelHalf, 0, -_voxelHalf - (TileSideVoxels - horizontalOffSet - 1) * VoxelSize);

        _rayStart.y = SpecifyVerticalPositionRaycastHit(_rayStart, verticalLayer);

        return TryDropeRaycastHit(_rayStart, direction);
    }

    private byte GetVoxelColorRightSide(int verticalLayer, int horizontalOffSet)
    {
        Vector3 direction = Vector3.right;

        _rayStart = _meshChildrenCollider.bounds.min +
                       new Vector3(-_voxelHalf, 0, _voxelHalf + horizontalOffSet * VoxelSize);

        _rayStart.y = SpecifyVerticalPositionRaycastHit(_rayStart, verticalLayer);

        return TryDropeRaycastHit(_rayStart, direction);
    }

    private float SpecifyVerticalPositionRaycastHit(Vector3 rayStart, int verticalLayer)
    {
        return _meshChildrenCollider.bounds.min.y + _voxelHalf + verticalLayer * VoxelSize;
    }

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