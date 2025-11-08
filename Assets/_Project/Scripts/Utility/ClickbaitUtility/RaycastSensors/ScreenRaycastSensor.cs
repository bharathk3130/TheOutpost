using UnityEngine;

namespace Clickbait.Utilities
{
    // Shoot a raycast from a a UI 2D coordinate to the real world
    public class ScreenRaycastSensor : RaycastSensorBase
    {
        Camera _camera;
        Vector2 _screenPosition;

        /// <summary>
        /// Screen Position is set to the center of the screen by default
        /// </summary>
        public ScreenRaycastSensor(Camera cam, float castLength) : base(cam.transform, castLength)
        {
            _screenPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
            _camera = cam;
        }

        public ScreenRaycastSensor(Camera cam, Vector2 screenPos, float castLength) : base(cam.transform, castLength)
        {
            _screenPosition = screenPos;
            _camera = cam;
        }

        public override void Cast()
        {
            Ray ray = _camera.ScreenPointToRay(_screenPosition);
            CastRay(ray.direction);
        }

        public void Cast(Vector2 screenPos)
        {
            _screenPosition = screenPos;
            Cast();
        }
    }
}