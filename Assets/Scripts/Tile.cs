using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField, Range(0f, 5f)] private float _disappearDuration = 0.25f;
    private PrefabInstancePool<Tile> _pool;
    private float _disappearProgress;

    [System.Serializable]
    private struct FallingState
    {
        public float FromY;
        public float ToY;
        public float Duration;
        public float Progress;
    }
    private FallingState _falling;

    public Tile Spawn (Vector3 position)
    {
        Tile instance = _pool.GetInstance(this);
        instance._pool = _pool;
        instance.transform.localPosition = position;
        instance.transform.localScale = Vector3.one;
        instance._disappearProgress = -1f;
        instance._falling.Progress = -1f;
        instance.enabled = false;
        return instance;
    }

    public void Despawn () => _pool.Recycle(this);

    public float Disappear ()
    {
        _disappearProgress = 0f;
        enabled = true;
        return _disappearDuration;
    }

    public float Fall (float toY, float speed)
    {
        _falling.FromY = transform.localPosition.y;
        _falling.ToY = toY;
        _falling.Duration = (_falling.FromY - toY) / speed;
        _falling.Progress = 0f;
        enabled = true;
        return _falling.Duration;
    }

    void Update ()
    {
        if (_disappearProgress >= 0f)
        {
            _disappearProgress += Time.deltaTime;
            if (_disappearProgress >= _disappearDuration)
            {
                Despawn();
                return;
            }
            transform.localScale =
                Vector3.one * (1f - _disappearProgress / _disappearDuration);
        }

        if (_falling.Progress >= 0f)
        {
            Vector3 position = transform.localPosition;
            _falling.Progress += Time.deltaTime;
            if (_falling.Progress >= _falling.Duration)
            {
                _falling.Progress = -1f;
                position.y = _falling.ToY;
                enabled = _disappearProgress >= 0f;
            }
            else
            {
                position.y = Mathf.Lerp(
                    _falling.FromY, 
                    _falling.ToY, 
                    _falling.Progress / _falling.Duration
                    );
            }
            transform.localPosition = position;
        }
    }
}