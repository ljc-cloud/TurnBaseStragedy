using Cinemachine;

namespace TurnBaseStragedy.Cameras
{
    public class CameraShake : MonoSingleton<CameraShake>
    {
        private CinemachineImpulseSource _impulseSource;
        protected override void Awake()
        {
            base.Awake();
            _impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public void Shake(float intensity = 1f)
        {
            _impulseSource.GenerateImpulse(intensity);
        }
    }
}