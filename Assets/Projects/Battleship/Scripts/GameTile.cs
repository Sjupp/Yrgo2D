using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

        //private void OnDrawGizmos()
        //{
        //    GUIStyle style = new GUIStyle();
        //    style.normal.textColor = Color.black;
        //    Handles.Label(transform.position, $"{_x}, {_y}", style);
        //}
    }
}
