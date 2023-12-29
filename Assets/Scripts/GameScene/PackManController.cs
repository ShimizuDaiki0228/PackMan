using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PackManController : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCamera;

    private const float SPEED = 3f;
    
    private Vector3 _currentDirection = Vector3.zero;

    private Vector3 _nextPos;
    private Vector3 _destination;

    [SerializeField]
    private LayerMask _unwalkableLayerMask;

    /// <summary>
    /// ���Z�b�g
    /// </summary>
    private Vector3 _initPosition;

    /// <summary>
    /// ���Z�b�g����邱�Ƃ�m�点��I�u�U�[�o�[
    /// </summary>
    private Subject<Unit> _onResetSubject = new Subject<Unit>();
    public IObservable<Unit> OnResetAsObservable => _onResetSubject.AsObservable();

    /// <summary>
    /// ������Ƃ��̃G�t�F�N�g
    /// </summary>
    [SerializeField]
    private GameObject _vanishingEffectPrefab;

    /// <summary>
    /// �e���|�[�g����{�b�N�X�̉E��
    /// </summary>
    [SerializeField]
    private GameObject _rightTeleportBox;

    /// <summary>
    /// �e���|�[�g����{�b�N�X�̍���
    /// </summary>
    [SerializeField]
    private GameObject _leftTeleportBox;

    /// <summary>
    /// �G�ɓ����������ǂ���
    /// </summary>
    public bool IsEnemyHit;

    /// <summary>
    /// �G��H�ׂ����ǂ���
    /// </summary>
    public bool IsEnemyEat;

    /// <summary>
    /// �A�C�e���⋯����Ԃ̓G�ɐG�ꂽ���Ɏ擾����_����\������e�L�X�g
    /// </summary>
    [SerializeField]
    private Text _getScoreText;

    /// <summary>
    /// _getScoreText��\������ʒu�̒���
    /// </summary>
    private readonly Vector3 _getScoreTextPositionOffset = new Vector3(0, 2, 0);

    /// <summary>
    /// �G��H�ׂ���
    /// </summary>
    private int _eatEnemyAmount = 0;

    /// <summary>
    /// �G�ɓ������ĉ��b��ɏ�����悤�ɂ��邩
    /// </summary>
    private const float DELAY_VANISH_TIME = 0.5f;

    /// <summary>
    /// �G�ɓ������ď��ł��Ă���ĊJ����܂łɂ����鎞��
    /// </summary>
    private const float DELEY_RESTART_TIME = 2f;

    /// <summary>
    /// �G��H�ׂ����ɃQ�b�g���链�_
    /// </summary>
    private const int ENEMY_EAT_POINT = 200;

    /// <summary>
    /// �a���擾���Ɏ�ɓ���X�R�A
    /// </summary>
    private const int PELLET_SCORE = 3;

    /// <summary>
    /// �p���[�a���擾���Ɏ�ɓ���X�R�A
    /// </summary>
    private const int POWERPELLET_SCORE = 10;

    /// <summary>
    /// �a��H�ׂ����Ƃ�m�点��I�u�U�[�o�[
    /// </summary>
    private Subject<int> _onEatPelletSubject = new Subject<int>();
    public IObservable<int> OnEatPelletAsObservable => _onEatPelletSubject.AsObservable();

    // Start is called before the first frame update
    void Start()
    {
        _initPosition = transform.position;

        Reset();
    }

    public void Reset()
    {
        transform.position = _initPosition;
        _currentDirection = InGameConst.Up;
        _nextPos = Vector3.forward;
        _destination = transform.position;
    }

    /// <summary>
    /// �蓮Update
    /// </summary>
    /// <param name="deltaTime"></param>
    public void ManualUpdate(float deltaTime)
    {
        Move(deltaTime);

    }

    private void Move(float deltaTime)
    {
        transform.position = Vector3.MoveTowards(transform.position, _destination, (SPEED + 0.3f * GameManager.Instance.Level) * deltaTime);

        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _nextPos = Vector3.forward;
            _currentDirection = InGameConst.Up;
        }
        else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _nextPos = Vector3.back;
            _currentDirection = InGameConst.Down;
        }
        else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _nextPos = Vector3.left;
            _currentDirection = InGameConst.Left;
        }
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            _nextPos = Vector3.right;
            _currentDirection = InGameConst.Right;
        }

        if(Vector3.Distance(transform.position, _destination) < 0.0001f)
        {
            transform.localEulerAngles = _currentDirection;
            {
                if(MoveValid())
                {
                    _destination = transform.position + _nextPos;
                }
            }
        }
    }

    /// <summary>
    /// �������Ԃ��ǂ���
    /// �ڂ̑O���ǂł����false��Ԃ��O�Ɉړ����Ȃ��悤�ɂ���
    /// </summary>
    /// <returns></returns>
    private bool MoveValid()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 0.25f, 0), transform.forward);
        RaycastHit raycastHit;

        if(Physics.Raycast(ray, out raycastHit, 1f, _unwalkableLayerMask))
        {
            if(raycastHit.collider.tag == "Wall")
            {
                return false;
            }
        }

        return true;
    }

    private async void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ghost")
        {
            PathFindings pGhost = other.GetComponent<PathFindings>();
            if (pGhost.State == GhostStates.FRIGHTEND)
            {
                int getScore = (int)Mathf.Pow(2, _eatEnemyAmount) * ENEMY_EAT_POINT;

                pGhost.StateProp.Value = GhostStates.GOT_EATEN;
                GameManager.Instance.AddScore(getScore);

                IsEnemyEat = true;

                await DisplayGetScoreText(getScore);
                
                IsEnemyEat = false;

                _eatEnemyAmount++;
            }
            else if (pGhost.State != GhostStates.FRIGHTEND && pGhost.State != GhostStates.GOT_EATEN)
            {
                IsEnemyHit = true;

                await HitEnemy();

                GameManager.Instance.LoseLife();
                Reset();

                //���C�t��0�łȂ��ꍇ�͍ēx���X�^�[�g����ۂ̃e�L�X�g��\������
                if (GameManager.Instance.Life != 0)
                    _onResetSubject.OnNext(Unit.Default);

                else if(GameManager.Instance.Life == 0)
                {
                    SceneManager.LoadScene("GameOverScene");
                }

                IsEnemyHit = false;
            }
        }

        else if (other.tag == "Item")
        {
            int score = other.GetComponent<PickupItem>().ItemData.Score;
            GameManager.Instance.ScoreProp.Value += score;
            DisplayGetScoreText(score).Forget();
            Destroy(other.gameObject);
        }

        else if (other.tag == "Pellet")
        {
            _onEatPelletSubject.OnNext(PELLET_SCORE);
            GameManager.Instance.ReducePellet(PELLET_SCORE);
            Destroy(other.gameObject);
        }

        else if (other.tag == "PowerPellet")
        {
            _onEatPelletSubject.OnNext(POWERPELLET_SCORE);
            GameManager.Instance.ReducePellet(POWERPELLET_SCORE);
            GameManager.Instance.OnFrightenSubject.OnNext(Unit.Default);
            Destroy(other.gameObject);
        }

        else if (other.gameObject == _rightTeleportBox)
        {
            transform.position = _leftTeleportBox.transform.position + new Vector3(2f, 0, 0);
            _destination = transform.position;
        }

        else if(other.gameObject == _leftTeleportBox)
        {
            transform.position = _rightTeleportBox.transform.position + new Vector3(-2f, 0, 0);
            _destination = transform.position;
        }
    }

    /// <summary>
    /// �A�C�e���⋯����Ԃ̓G�ɐG�ꂽ���Ƀe�L�X�g��\������
    /// </summary>
    private async UniTask DisplayGetScoreText(int getScore)
    {
        _getScoreText.gameObject.SetActive(true);
        _getScoreText.text = getScore.ToString();

        Vector3 textPosition = transform.position + new Vector3(0, 2, 0);
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(textPosition);
        _getScoreText.transform.position = screenPos;

        await UniTask.WaitForSeconds(0.5f);

        _getScoreText.gameObject.SetActive(false);
    }

    /// <summary>
    /// �G�ƏՓ˂����Ƃ��ɌĂ΂��
    /// </summary>
    private async UniTask HitEnemy()
    {
        await UniTask.WaitForSeconds(DELAY_VANISH_TIME);

        await gameObject.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);

        //���ł���G�t�F�N�g�𐶐�
        EffectCreate(_vanishingEffectPrefab);

        await UniTask.WaitForSeconds(DELEY_RESTART_TIME);

        gameObject.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// �G�t�F�N�g�𐶐�����
    /// �G�t�F�N�g��Start Life Time���߂���Ɣj������
    /// </summary>
    private void EffectCreate(GameObject effectPrefab)
    {
        var effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        var mainModule = effectPrefab.GetComponent<ParticleSystem>().main;
        float lifeTime = mainModule.startLifetime.constant;

        Destroy(effect, lifeTime);
    }

    /// <summary>
    /// �p�b�N�}�����G��H�ׂ��񐔂����Z�b�g����
    /// </summary>
    public void ResetEatEnemyAmount()
    {
        _eatEnemyAmount = 0;
    }
}
