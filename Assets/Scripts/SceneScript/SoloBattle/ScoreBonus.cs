using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    // ----- 定数 ----- //
    [Header("▼ スコアボーナス")]
    [SerializeField] float BONUS_TIMELIMIT = 3.0f;

    // ----- プロパティ ----- //
    public int CurrentScore => _score;

    // ----- 変数 ----- //
    // スコア
    [SerializeField] Text _scoreUI;
    int _score = 0;

    // ボーナス
    [SerializeField] Image _bonusTimerUI;
    [SerializeField] Text _scoreBonusUI;
    float _currentScoreBonus = 1.0f;
    float _defaultScoreBonus = 1.0f;
    float _scoreBonusTimer = 0.0f;

    // 演出
    DirectionStep _direction = DirectionStep.None;
    float _acceleration = 0.0f;
    Vector2 _startPos;

    void Start()
    {
        _currentScoreBonus = _defaultScoreBonus;
        _startPos = _scoreBonusUI.rectTransform.position;
    }

    void Update()
    {
        // ボーナスが無いなら中断
        if (_currentScoreBonus != _defaultScoreBonus)
        {
            // タイマーを進めて、タイマーが最大になったらボーナスを終了
            _scoreBonusTimer = Mathf.Max(0.0f, _scoreBonusTimer - Time.deltaTime);
            if (_scoreBonusTimer == 0.0f)
            {
                _currentScoreBonus = _defaultScoreBonus;
                _scoreBonusTimer = 0.0f;
            }

            // UIの同期
            Draw();
            _bonusTimerUI.fillAmount = _scoreBonusTimer / BONUS_TIMELIMIT;
        }

        // 演出中でないなら中断
        if (_direction == DirectionStep.None) return;
        Direction();
    }

    public void CalculateScore(int value)
    {
        // 計算（0未満になるなら調整）
        _score += Mathf.RoundToInt(value * _currentScoreBonus);

        // ボーナス、タイマーの調整
        _currentScoreBonus += 0.1f;
        _scoreBonusTimer = BONUS_TIMELIMIT;

        // UI、演出の同期
        Draw();
        _bonusTimerUI.fillAmount = 1;
        _direction = DirectionStep.Enter;

    }

    void Draw()
    {
        // UIの同期
        _scoreBonusUI.text = "x " + _currentScoreBonus.ToString("N1");
        _scoreUI.text = _score.ToString();
    }

    void Direction()
    {
        switch (_direction)
        {
            // 演出を開始
            case DirectionStep.Enter:
                
                // 初期化
                _scoreBonusUI.rectTransform.position = _startPos;
                _direction = DirectionStep.Current;
                _acceleration = 0.0f;
                break;

            // 演出中
            case DirectionStep.Current:

                // 位置とサイズを徐々に変更
                _acceleration += Time.deltaTime / 7.0f;
                Vector2 pos = _scoreBonusUI.rectTransform.position;
                float nextY = pos.y + (0.05f - _acceleration);

                if (nextY < _startPos.y)
                {
                    _direction = DirectionStep.None;
                    _scoreBonusUI.rectTransform.position = _startPos;
                }
                else
                    _scoreBonusUI.rectTransform.position = new Vector2(pos.x, nextY);

                break;
        }
    }

    enum DirectionStep
    {
        None,
        Enter,
        Current
    }
}