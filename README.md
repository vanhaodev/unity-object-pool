# Object Pool for Unity (com.vanhaodev.objectpool)

A simple, high-performance, generic object pooling library for Unity. Works with any type `T`, including `GameObject` and custom classes.

## Features

* Pre-warm pool with configurable initial size

* O(1) `Get` and `Release` operations

* Optional callbacks: `onGet`, `onRelease`, `onDestroy`

* Tracks active objects for accurate stats

* Clear idle or all objects safely

* Lightweight and dependency-free

## Use Cases

* Bullet/projectile systems

* Particle or effect pooling

* Reusable UI elements

* Any scenario requiring frequent object creation/destruction

## ⚡ Install via UPM

[Download the latest release](https://github.com/vanhaodev/unity-object-pool/releases)

## Example (Import to project to watch full)

```csharp
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
}
```
