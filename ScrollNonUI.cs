using DG.Tweening;
using UnityEngine;
using Curookie.Util;

namespace Curookie.UI
{
    public class ScrollNonUI : MonoBehaviour
    {
        public float tweenBackDuration = 0.3f;
        public Ease tweenBackEase;
        public bool freezeX;
        public FloatMinMax xConstraints = new FloatMinMax();
        public bool freezeY;
        public FloatMinMax yConstraints = new FloatMinMax();
        public float scrollSpeed = 1.0f;
        private Vector2 offset;
        // distance from the center of this Game Object to the point where we clicked to start dragging 
        private Vector3 pointerDisplacement;
        private float zDisplacement;
        private bool dragging;
        private bool _isHover = false;
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
            zDisplacement = -mainCamera.transform.position.z + transform.position.z;
        }

        public void OnMouseDown()
        {
            pointerDisplacement = -transform.position + MouseInWorldCoords();
            transform.DOKill();
            dragging = true;
        }

        public void OnMouseUp()
        {
            dragging = false;
            TweenBack();
        }

        public void OnMouseEnter() {
            
            _isHover = true;
        }

        public void OnMouseExit() {
            
            _isHover = false;
            TweenBack();
        }

        private void Update()
        {
            if (!dragging&&!_isHover) return;

            var mousePos = Vector3.zero;
            if (_isHover&&Input.mouseScrollDelta.y!=0) {
                pointerDisplacement = new Vector3(0, Input.mouseScrollDelta.y*scrollSpeed, 0);
                mousePos = transform.position;
            } else if(!dragging) {
                return;
            } else {
                mousePos = MouseInWorldCoords();
            }
            
            //Debug.Log(mousePos);
            // transform.position = new Vector3(
            //     freezeX ? transform.position.x : mousePos.x - pointerDisplacement.x,
            //     freezeY ? transform.position.y : mousePos.y - pointerDisplacement.y,
            //     transform.position.z);
            transform.position = new Vector3(
                freezeX ? transform.position.x : mousePos.x - pointerDisplacement.x,
                freezeY ? transform.position.y : mousePos.y - pointerDisplacement.y,
                transform.position.z);
        }

        // returns mouse position in World coordinates for our GameObject to follow. 
        private Vector3 MouseInWorldCoords()
        {
            var screenMousePos = Input.mousePosition;
            //Debug.Log(screenMousePos);
            screenMousePos.z = zDisplacement;
            return mainCamera.ScreenToWorldPoint(screenMousePos);
        }

        private void TweenBack()
        {
            if (freezeY)
            {
                if (transform.localPosition.x >= xConstraints.min && transform.localPosition.x <= xConstraints.max)
                    return;

                var targetX = transform.localPosition.x < xConstraints.min ? xConstraints.min : xConstraints.max;
                transform.DOLocalMoveX(targetX, tweenBackDuration).SetEase(tweenBackEase);
            }
            else if (freezeX)
            {
                if (transform.localPosition.y >= yConstraints.min && transform.localPosition.y <= yConstraints.max)
                    return;

                var targetY = transform.localPosition.y < yConstraints.min ? yConstraints.min : yConstraints.max;
                transform.DOLocalMoveY(targetY, tweenBackDuration).SetEase(tweenBackEase);
            }
        }
    }
}
