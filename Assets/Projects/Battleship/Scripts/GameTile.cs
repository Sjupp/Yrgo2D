using UnityEngine;

namespace Battleship
{
    public class GameTile : MonoBehaviour
    {
        [SerializeField] private float _oscillationSpeed = 0.05f;
        [SerializeField] private float _oscillationHeight = 0.1f;
        [SerializeField] private float _lerpSpeed = 3f;
        [SerializeField] private float _punchResetLerpSpeed = 3f;
        private Vector3 _startPos = Vector3.zero;
        [SerializeField]
        private Vector3 _punchOffset = Vector3.zero;
        private int _x, _y;
        private float _offset = 0f;

        public void Init(float scale, int x, int y)
        {
            _x = x;
            _y = y;
            transform.localScale = Vector3.one * scale;
            _startPos = transform.localPosition;
            _offset = 0.1f * x;
        }

        public void Marked()
        {
            GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
        }

        public void UpdateTile(Sprite sprite, float pieceRotation)
        {
            if (sprite != null)
            {
                if (TryGetComponent(out SpriteRenderer comp))
                {
                    comp.sprite = sprite;
                    
                    transform.localRotation = Quaternion.Euler(0f, 0, pieceRotation);
                }
            }
        }

        public void PunchTile(Vector3 direction, float strength)
        {
            _punchOffset = direction * strength;
        }

        private void Update()
        {
            if (_punchOffset != Vector3.zero)
            {
                _punchOffset = Vector3.Lerp(_punchOffset, Vector3.zero, Time.deltaTime * _punchResetLerpSpeed);
            }

            var targetPosition = _startPos + _punchOffset + Vector3.up * Mathf.PingPong((Time.time + _offset) * _oscillationSpeed, _oscillationHeight);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * _lerpSpeed);
        }

        //private void OnDrawGizmos()
        //{
        //    GUIStyle style = new GUIStyle();
        //    style.normal.textColor = Color.black;
        //    Handles.Label(transform.position, $"{_x}, {_y}", style);
        //}
    }
}
