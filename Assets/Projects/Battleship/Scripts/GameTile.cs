using UnityEditor;
using UnityEngine;

namespace Battleship
{
    public class GameTile : MonoBehaviour
    {
        private int _x, _y;

        public void Init(float scale, int x, int y)
        {
            _x = x;
            _y = y;
            transform.localScale = Vector3.one * scale;
        }

        public void Marked()
        {
            GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
        }

        //private void OnDrawGizmos()
        //{
        //    GUIStyle style = new GUIStyle();
        //    style.normal.textColor = Color.black;
        //    Handles.Label(transform.position, $"{_x}, {_y}", style);
        //}
    }
}
