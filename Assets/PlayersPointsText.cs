using Midbaryom.Core;
using TMPro;
using UnityEngine;
namespace Midbaryom.UI
{

    public class PlayersPointsText : MonoBehaviour
    {
        private const string Score = "Score: ";
        [SerializeField]
        private TextMeshProUGUI _textMeshProUGUI;
        private IStat _points;
        private void Awake()
        {
            ResetText();
        }
        private void Start()
        {
            _points = Spawner.Instance.Player.StatHandler[StatType.Points];
            _points.OnValueChanged += UpdateText;
        }

        private void ResetText()
        {
            UpdateText(0);
        }
        private void UpdateText(float obj)
        {
            _textMeshProUGUI.text = string.Concat(Score, Mathf.RoundToInt(obj));
        }

        private void OnDestroy()
        {
            _points.OnValueChanged -= UpdateText;
        }
    }

}