using UnityEngine;

namespace com.vanhaodev.objectpool.samples
{
	public class BulletSpawner : MonoBehaviour
	{
		public GameObject bulletPrefab;

		private ObjectPool<GameObject> _pool;

		private int _bulletCount = 4;
		public float speed = 5f;

		private float _fireRate = 1f;
		private float _timer = 0f;
		float _deltaTime;
		private Camera _cam;

		void Start()
		{
			_cam = Camera.main;

			_pool = new ObjectPool<GameObject>(
				() =>
				{
					var go = Instantiate(bulletPrefab, transform);
					go.SetActive(false);
					return go;
				},
				initialSize: 100,
				onGet: go => go.SetActive(true),
				onRelease: go => go.SetActive(false),
				onDestroy: Destroy
			);
		}

		void Update()
		{
			_timer += Time.deltaTime;
			_deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
			if (_timer >= 1f / _fireRate)
			{
				_timer = 0f;
				Shoot();
			}
		}

		void Shoot()
		{
			float angleStep = 360f / _bulletCount;

			for (int i = 0; i < _bulletCount; i++)
			{
				float angle = i * angleStep * Mathf.Deg2Rad;
				Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

				var bullet = _pool.Get();
				bullet.transform.position = Vector3.zero;

				var sr = bullet.GetComponent<SpriteRenderer>();
				if (sr != null)
				{
					sr.color = Random.ColorHSV();
				}

				var mover = bullet.GetComponent<BulletMover>();
				if (mover == null)
				{
					mover = bullet.AddComponent<BulletMover>();
				}

				mover.Init(dir, speed, _pool, _cam);
			}
		}

		void OnGUI()
		{
			GUIStyle style = new GUIStyle(GUI.skin.label) { fontSize = 35 };
			GUIStyle btnStyle = new GUIStyle(GUI.skin.button) { fontSize = 30 };

			// -------- Controls --------
			GUI.Label(new Rect(20, 20, 600, 50), $"Bullet Count: {_bulletCount}", style);
			_bulletCount = (int)GUI.HorizontalSlider(new Rect(20, 70, 400, 40), _bulletCount, 1, 200);

			GUI.Label(new Rect(20, 120, 600, 50), $"Fire Rate: {_fireRate:F1}", style);
			_fireRate = GUI.HorizontalSlider(new Rect(20, 170, 400, 40), _fireRate, 0.1f, 10f);

			// -------- Clear Pool Button --------
			if (GUI.Button(new Rect(20, 470, 300, 70), "CLEAR POOL", btnStyle))
			{
				_pool.Clear(true); // clear including active objects
			}

			// -------- Pool Stats --------
			int usingCount = _pool.ActiveCount; // track active objects in pool
			int idleCount = _pool.InactiveCount;
			int totalCount = usingCount + idleCount;

			GUI.Label(new Rect(20, 230, 600, 50), $"Total Objects: {totalCount}", style);
			GUI.Label(new Rect(20, 270, 600, 50), $"Using Objects: {usingCount}", style);
			GUI.Label(new Rect(20, 310, 600, 50), $"Idle Objects: {idleCount}", style);

			// -------- Performance --------
			float ms = _deltaTime * 1000.0f;
			float fps = 1.0f / _deltaTime;

			GUI.Label(new Rect(20, 370, 600, 50), $"Frame Time: {ms:F1} ms", style);
			GUI.Label(new Rect(20, 410, 600, 50), $"FPS: {fps:F0}", style);
		}
	}
}