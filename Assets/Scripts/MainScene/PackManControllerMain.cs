using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PackManControllerMain : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreText;

    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private Camera _mainCamera;

    private Animator _animator;

    private readonly ReactiveProperty<bool> _isPowerPelletHitProp = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> IsPowerPelletHitProp => _isPowerPelletHitProp;
    private bool IsPowerPelletHit => _isPowerPelletHitProp.Value;

    private int _hitEnemyAmount = 0;

    private const int ENEMY_POINT = 200;

    public bool IsEnemyHit;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void ManualUpdate()
    {
        if(!IsPowerPelletHit)
            transform.position += new Vector3(-4, 0, 0) * Time.deltaTime;
        else
            transform.position += new Vector3(4, 0, 0) * Time.deltaTime;
    }

    private async void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PowerPellet")
        {
            Destroy(other.gameObject);
            _isPowerPelletHitProp.Value = true;
            transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        else if(other.tag == "Ghost")
        {
            _animator.speed = 0;

            IsEnemyHit = true;

            other.gameObject.SetActive(false);
            await DisplayScoreText((int)Mathf.Pow(2, _hitEnemyAmount) * ENEMY_POINT);

            IsEnemyHit = false;

            _hitEnemyAmount++;
            _animator.speed = 1;
        }


    }

    /// <summary>
    /// 敵に触れた時にスコアを表示する
    /// </summary>
    private async UniTask DisplayScoreText(int getScore)
    {
        _scoreText.gameObject.SetActive(true);
        _scoreText.text = getScore.ToString();

        Vector3 textPosition = transform.position + new Vector3(0, 2, 0);
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(textPosition);

        // スクリーン座標を Canvas 座標に変換
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPos, _mainCamera, out Vector2 movePos);
        _scoreText.rectTransform.anchoredPosition = movePos;

        await UniTask.WaitForSeconds(0.5f);

        _scoreText.gameObject.SetActive(false);
    }
}
