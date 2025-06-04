using GangstaBean.UI;
using UnityEngine;

namespace GangstaBean.UI
{
    public class Window_QuestPointer : MonoBehaviour {



        private Vector3 targetPosition;
        private RectTransform pointerRectTransform;
        public GameObject Pointer;

        private void Awake() {
            pointerRectTransform = Pointer.GetComponent<RectTransform>();

            //Hide();
        }

        private void Update() {
            float borderSize = 100f;
            Vector3 targetPositionScreenPoint = CursorManager.GetCamera().WorldToScreenPoint(targetPosition);
            bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;

            if (isOffScreen) {
                RotatePointerTowardsTargetPosition();

                Pointer.SetActive(true);
                Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
                if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
                if (cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;
                if (cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
                if (cappedTargetScreenPosition.y >= Screen.height - borderSize) cappedTargetScreenPosition.y = Screen.height - borderSize;

                Vector3 pointerWorldPosition = CursorManager.GetCamera().ScreenToWorldPoint(cappedTargetScreenPosition);
                pointerRectTransform.position = pointerWorldPosition;
                pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);
            } else {
                Pointer.SetActive(false);
                Vector3 pointerWorldPosition = CursorManager.GetCamera().ScreenToWorldPoint(targetPositionScreenPoint);
                pointerRectTransform.position = pointerWorldPosition;
                pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);

                pointerRectTransform.localEulerAngles = Vector3.zero;
            }
        }

        private void RotatePointerTowardsTargetPosition() {
            Vector3 toPosition = targetPosition;
            Vector3 fromPosition = CursorManager.GetCamera().transform.position;
            fromPosition.z = 0f;
            Vector3 dir = (toPosition - fromPosition).normalized;
            float angle = GetAngleFromVectorFloat(dir);
            pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
        }

        public static float GetAngleFromVectorFloat(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }
        public void Hide() {
            gameObject.SetActive(false);
        }

        public void Show(Vector3 targetPosition) {
            gameObject.SetActive(true);
            this.targetPosition = targetPosition;
        }
    }
}
