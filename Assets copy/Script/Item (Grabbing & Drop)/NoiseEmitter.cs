using UnityEngine;
using System.Collections;

public class NoiseEmitter : MonoBehaviour
{
    [SerializeField] private float noiseRadius = 10f; // Jarak deteksi suara
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float noiseDelay = 0.1f; // Delay agar suara tidak langsung terdeteksi

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(GenerateNoiseWithDelay());
    }

    private IEnumerator GenerateNoiseWithDelay()
    {
        yield return new WaitForSeconds(noiseDelay);

        Collider[] enemies = Physics.OverlapSphere(transform.position, noiseRadius, enemyLayer);
        foreach (Collider enemy in enemies)
        {
            EnemyManager enemyManager = enemy.GetComponent<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.OnHearNoise(transform.position);
            }
        }
    }
}
