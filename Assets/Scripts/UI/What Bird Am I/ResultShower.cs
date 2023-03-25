using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultShower : MonoBehaviour
{
    [SerializeField]
    private ScoreAnalyzer _scoreAnalyzer;

    [SerializeField]
    private RectTransform _resultTransform;
    [SerializeField]
    private TextMeshProUGUI _eagleName;
    [SerializeField]
    private Image _eagleImage;


    [SerializeField]
    private float _entranceDuration;
    [SerializeField]
    private AnimationCurve _entranceCurve;
    private void Awake()
    {
        _resultTransform.localScale = Vector3.zero;
        _scoreAnalyzer.OnEagleResultFound += ShowResult;
    }
    private void OnDestroy()
    {
        _scoreAnalyzer.OnEagleResultFound -= ShowResult;
    }
    private void ShowResult(EagleTypeSO obj)
    {
        //   _eagleImage.sprite = obj.Image;
        _eagleName.text = obj.Name;

        StartCoroutine(VisualEntrance());
    }



    private IEnumerator VisualEntrance()
    {
        Vector3 scale = Vector3.zero;
        SetScale(scale);
        float curveTime = 0;
        float counter = 0;
        do
        {
            yield return null;
            counter += Time.deltaTime;
            curveTime = _entranceCurve.Evaluate(counter / _entranceDuration);
            scale = Vector3.Lerp(Vector3.zero, Vector3.one, curveTime);
            SetScale(scale);
        } while (counter <= _entranceDuration);

        void SetScale(Vector3 vector3)
            => _resultTransform.localScale = vector3;
    }
}
