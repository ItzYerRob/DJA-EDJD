using UnityEngine;

namespace StartButton
{
    public class StartBtnComponent : MonoBehaviour
    {
        private Animator _animator;
        private Transform _transform;
        private Material _material;
        private Vector3 _startScale;
        private Vector3 _shrinkScale;
        private Color _startColor;
        private Color _shrinkColor;

        private void Awake()
        {
            
            _transform = gameObject.GetComponent<Transform>();
            _animator = gameObject.GetComponent<Animator>();
            _material = gameObject.GetComponent<Renderer>().material;
            
            _startScale = gameObject.transform.localScale;
            _shrinkScale = _startScale * 2/3;
            if (!_material.shader.ToString().Contains("TextMeshPro")) _startColor = _material.color;
            if (!_material.shader.ToString().Contains("TextMeshPro")) _shrinkColor = _startColor - new Color(0, 0, 0, 0.5f);

        }

        public void Shrink()
        {
            _material.color = _shrinkColor;
            _animator.enabled = false;
            _transform.localScale = _shrinkScale;
        }

        public void Grow()
        {
            _material.color = _startColor;
            _animator.enabled = true;
            _transform.localScale = _startScale;
        }
    }
}
