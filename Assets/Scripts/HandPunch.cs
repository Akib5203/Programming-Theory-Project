using UnityEngine;

public class HandPunch : MonoBehaviour
{
    public float punchForce; // Adjust the force of the punch

    [SerializeField] private StarterAssets.ThirdPersonController _thirdPersonController;

    [SerializeField] private TMPro.TMP_Text _punchStat;
    [SerializeField] private TMPro.TMP_Text _totalHitStat;

    private readonly string _punchCountText = "Punch Count: ";
    private readonly string _totalCountText = "Total Hit Count: ";

    private void OnEnable() => GameManager.Instance.StatsReadyForDisplay += OnStatsReadyForDisplay;

    private void OnStatsReadyForDisplay()
    {
        // On Stats Ready, display the values
        _punchStat.text = _punchCountText + GameManager.Instance.PunchCount;
        _totalHitStat.text = _totalCountText + GameManager.Instance.TotalHitsCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is the punching bag
        if (other.CompareTag("PunchingBag"))
        {
            // Get the Rigidbody of the punching bag
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                //Enable the punch audio
                _thirdPersonController.combatAudioEnabled = true;

                // Calculate the punch direction (from hand to the bag's center)
                Vector3 punchDirection = (other.transform.position - transform.position).normalized;

                // Apply force to the punching bag
                rb.AddForce(punchDirection * punchForce, ForceMode.Impulse);

                //Increase Punch Count and Total hit count by 1 and update the UI
                _punchStat.text = _punchCountText + ++GameManager.Instance.PunchCount;
                _totalHitStat.text = _totalCountText + ++GameManager.Instance.TotalHitsCount;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Disable the punch audio
        if (other.CompareTag("PunchingBag")) _thirdPersonController.combatAudioEnabled = false;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null) GameManager.Instance.StatsReadyForDisplay -= OnStatsReadyForDisplay;
    }
}