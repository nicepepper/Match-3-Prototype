using TMPro;
using UnityEngine;

public class FloatingScore : MonoBehaviour
{
    [SerializeField] private TextMeshPro _displayText;
    [SerializeField, Range(0.1f, 1f)] private float _displayDuration = 0.5f;
    [SerializeField, Range(0f, 4f)] private float _riseSpeed = 2f;

    private float _age;
    private PrefabInstancePool<FloatingScore> _pool;

    public void Show (Vector3 position, int value)
    {
        FloatingScore instance = _pool.GetInstance(this);
        instance._pool = _pool;
        instance._displayText.SetText("{0}", value);
        instance.transform.localPosition = position;
        instance._age = 0f;
    }

    private void Update ()
    {
        _age += Time.deltaTime;
        if (_age >= _displayDuration)
        {
            _pool.Recycle(this);
        }
        else
        {
            Vector3 p = transform.localPosition;
            p.y += _riseSpeed * Time.deltaTime;
            transform.localPosition = p;
        }
    }
}
