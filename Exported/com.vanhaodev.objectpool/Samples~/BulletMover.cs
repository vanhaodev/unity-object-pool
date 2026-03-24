using UnityEngine;

namespace com.vanhaodev.objectpool.samples
{
	public class BulletMover : MonoBehaviour
	{
		private Vector2 _dir;
		private float _speed;

		private ObjectPool<GameObject> _pool;
		private Camera _cam;

		public void Init(Vector2 dir, float speed, ObjectPool<GameObject> pool, Camera cam)
		{
			_dir = dir;
			_speed = speed;
			_pool = pool;
			_cam = cam;
		}

		void Update()
		{
			transform.position += (Vector3)(_dir * _speed * Time.deltaTime);

			if (_cam == null) return;

			Vector3 view = _cam.WorldToViewportPoint(transform.position);

			if (view.x < -0.1f || view.x > 1.1f ||
			    view.y < -0.1f || view.y > 1.1f)
			{
				_pool.Release(gameObject);
			}
		}
	}
}