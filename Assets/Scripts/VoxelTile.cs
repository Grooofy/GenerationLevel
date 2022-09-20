using System;
using UnityEngine;


public class VoxelTile : MonoBehaviour
{
    [SerializeField] public float VoxelSize = 0.1f;
    [SerializeField] public int TileSideVoxels = 8;

<<<<<<< Updated upstream
    [HideInInspector] public byte[] ColorRight;
    [HideInInspector] public byte[] ColorForward;
    [HideInInspector] public byte[] ColorLeft;
    [HideInInspector] public byte[] ColorBack;

    [Range(1,100)]   
    public int Weight = 50;
=======
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

    // public IReadOnlyCollection<byte> ColorBack => _colorBack;  
    public IReadOnlyCollection<byte> ColorLeft => _colorLeft;
>>>>>>> Stashed changes

    private MeshCollider _meshCollider;

    private void Awake()
    {
<<<<<<< Updated upstream
        _meshCollider = GetComponentInChildren<MeshCollider>();
    }

    public void CalculateSideColors()
    {
        ColorRight = new byte[TileSideVoxels * TileSideVoxels];
        ColorForward = new byte[TileSideVoxels * TileSideVoxels];
        ColorLeft = new byte[TileSideVoxels * TileSideVoxels];
=======
        _meshChildrenCollider = GetComponentInChildren<MeshCollider>();
    }

    
    public void CalculateSideColors()
    {
        //ColorLeft = new byte[TileSideVoxels * TileSideVoxels];
>>>>>>> Stashed changes
        ColorBack = new byte[TileSideVoxels * TileSideVoxels];
        
        for (int y = 0; y < TileSideVoxels; y++)
        {
            for (int i = 0; i < TileSideVoxels; i++)
            {
<<<<<<< Updated upstream
                ColorRight[y * TileSideVoxels + i] = GetVoxelColor(y, i, Vector3.right);
                ColorForward[y * TileSideVoxels + i] = GetVoxelColor(y, i, Vector3.forward);
                ColorLeft[y * TileSideVoxels + i] = GetVoxelColor(y, i, Vector3.left);
                ColorBack[y * TileSideVoxels + i] = GetVoxelColor(y, i, Vector3.back);
=======
                _colorRight.Add(GetVoxelColorRightSide(y, i));
                _colorForward.Add(GetVoxelColorForwardSide(y, i));
                _colorLeft.Add(GetVoxelColorLeftSide(i, y));
                ColorBack[y * TileSideVoxels + i] = GetVoxelColorBackSide(y, i);
>>>>>>> Stashed changes
            }
        }
    }

<<<<<<< Updated upstream
    private byte GetVoxelColor(int verticalLayer, int horizontalOffSet, Vector3 direction)
    {        
        float vox = VoxelSize;
        float half = VoxelSize / 2;
=======
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
>>>>>>> Stashed changes

        Vector3 rayStart ;
        
        if (direction == Vector3.right)
        {
            rayStart = _meshCollider.bounds.min + 
                       new Vector3(-half, 0, half + horizontalOffSet * vox);
        }
        else if(direction == Vector3.forward)
        {
            rayStart = _meshCollider.bounds.min +
                       new Vector3(half + horizontalOffSet * vox, 0, -half);
        }
        else if (direction == Vector3.left)
        {
            rayStart = _meshCollider.bounds.max + 
                       new Vector3(half, 0, -half - (TileSideVoxels - horizontalOffSet -1) * vox);
        }
        else if (direction == Vector3.back)
        {
            rayStart = _meshCollider.bounds.max +
                       new Vector3(-half - (TileSideVoxels - horizontalOffSet -1) * vox, 0, half);
        }
        else
        {
            throw new ArgumentException("Tou suck",  nameof(direction));
           
        }
        rayStart.y = _meshCollider.bounds.min.y + half + verticalLayer * vox;

        return TryDropeRay(rayStart, direction);
    }

<<<<<<< Updated upstream
  

    private byte TryDropeRay(Vector3 rayStart, Vector3 direction)
=======
    private float SpecifyVerticalPositionRaycastHit(Vector3 rayStart, int verticalLayer)
    {
        return _meshChildrenCollider.bounds.min.y + _voxelHalf + verticalLayer * VoxelSize;
    }

    private byte TryDropeRaycastHit(Vector3 rayStart, Vector3 direction)
>>>>>>> Stashed changes
    {
        if (Physics.Raycast(new Ray(rayStart, direction), out RaycastHit hit, VoxelSize))
        {
            Mesh mesh = _meshCollider.sharedMesh;

            int hitTriangleVertex = mesh.triangles[hit.triangleIndex * 3];
            byte colorIndex = (byte)(mesh.uv[hitTriangleVertex].x * 256);

            return colorIndex;
        }
        return 0;
    }
}