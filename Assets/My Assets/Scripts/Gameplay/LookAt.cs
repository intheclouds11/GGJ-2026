using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LookAt : MonoBehaviour
{
    [field: SerializeField, Tooltip("If blank, uses Camera.main")]
    protected Transform _target;
    public bool LookAtTarget;
    public float Smoothing = 1f;
    public bool FaceTargetForward = true;
    [EnableIf(nameof(FaceTargetForward))]
    public bool FlipDirection;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (!_target)
        {
            _target = Camera.main.transform;
        }
    }

    // Fixes Player LookAt components losing camera reference on scene reload
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Awake();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void Update()
    {
        if (_target)
        {
            if (LookAtTarget)
            {
                var targetDir = Quaternion.LookRotation(transform.DirectionTo(_target));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, Smoothing * Time.deltaTime);
            }
            else if (FaceTargetForward)
            {
                var dir = FlipDirection ? -_target.forward : _target.forward;
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }
}