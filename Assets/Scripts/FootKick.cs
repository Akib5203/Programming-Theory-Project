using UnityEngine;

public class FootKick : MonoBehaviour
{
    public float kickForce; // Adjust the force of the kick

    [SerializeField] private StarterAssets.ThirdPersonController _thirdPersonController;

    [SerializeField] private TMPro.TMP_Text _kickStat;
    [SerializeField] private TMPro.TMP_Text _totalHitStat;

    private readonly string _kickCountText = "Kick Count: ";
    private readonly string _totalCountText = "Total Hit Count: ";

    private void OnEnable() => GameManager.Instance.StatsReadyForDisplay += OnStatsReadyForDisplay;

    private void OnStatsReadyForDisplay()
    {
        // On Stats Ready, display the values
        _kickStat.text = _kickCountText + GameManager.Instance.KickCount;
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
                //Enable the kick audio
                _thirdPersonController.combatAudioEnabled = true;

                // Calculate the kick direction (from foot to the bag's center)
                Vector3 kickDirection = (other.transform.position - transform.position).normalized;

                // Apply force to the punching bag
                rb.AddForce(kickDirection * kickForce, ForceMode.Impulse);

                //Increase Kick Count and Total hit count by 1 and update the UI
                _kickStat.text = _kickCountText + ++GameManager.Instance.KickCount;
                _totalHitStat.text = _totalCountText + ++GameManager.Instance.TotalHitsCount;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Disable the kick audio
        if (other.CompareTag("PunchingBag")) _thirdPersonController.combatAudioEnabled = false;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null) GameManager.Instance.StatsReadyForDisplay -= OnStatsReadyForDisplay;
    }
}
