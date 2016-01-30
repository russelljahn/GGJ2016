using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.OutOfTheBox.Scripts.Components
{
    public class InputDebugger : MonoBehaviour
    {
        [SerializeField] private Text _text;

        private void Update()
        {
            var axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            var arrowLeft = Input.GetKey(KeyCode.LeftArrow);
            var arrowRight = Input.GetKey(KeyCode.RightArrow);
            var arrowUp = Input.GetKey(KeyCode.UpArrow);
            var arrowDown = Input.GetKey(KeyCode.DownArrow);
            var mouseDown = Input.GetMouseButton(0);

            _text.text = string.Format("Axis: {0}\nLeft? {1}\nRight? {2}\nUp? {3}\nDown? {4}\nMouseDown? {5}", axis, arrowLeft, arrowRight, arrowUp, arrowDown, mouseDown);
        }
    }
}
