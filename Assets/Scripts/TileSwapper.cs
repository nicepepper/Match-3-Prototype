using UnityEngine;

[System.Serializable]
public class TileSwapper
{
    [SerializeField, Range(0.1f, 10f)] private float _duration = 0.25f;
    [SerializeField, Range(0f, 1f)] private float _maxDepthOffset = 0.5f;

    private Tile _tileFirst;
    private Tile _tileSecond;
    private Vector3 _positionFirst;
    private Vector3 _positionSecond;
    private float _progress = -1f;
    private bool _pingPong;

    public float Swap (Tile first, Tile second, bool pingPong)
    {
        _tileFirst = first;
        _tileSecond = second;
        _positionFirst = first.transform.localPosition;
        _positionSecond = second.transform.localPosition;
        _pingPong = pingPong;
        _progress = 0f;
        return pingPong ? 2f * _duration : _duration;
    }

    public void Update ()
    {
        if (_progress < 0f)
        {
            return;
        }

        _progress += Time.deltaTime;
        if (_progress >= _duration)
        {
            if (_pingPong)
            {
                _progress -= _duration;
                _pingPong = false;
                (_tileFirst, _tileSecond) = (_tileSecond, _tileFirst);
            }
            else
            {
                _progress = -1f;
                _tileFirst.transform.localPosition = _positionSecond;
                _tileSecond.transform.localPosition = _positionFirst;
                return;
            }
        }

        MoveTiles();
    }

    private void MoveTiles()
    {
        float t = _progress / _duration;
        float z = Mathf.Sin(Mathf.PI * t) * _maxDepthOffset;
        Vector3 p = Vector3.Lerp(_positionFirst, _positionSecond, t);
        p.z = -z;
        _tileFirst.transform.localPosition = p;
        p = Vector3.Lerp(_positionFirst, _positionSecond, 1f - t);
        p.z = z;
        _tileSecond.transform.localPosition = p;
    }
}
