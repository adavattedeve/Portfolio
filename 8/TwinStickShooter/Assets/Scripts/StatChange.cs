public class StatChange
{
    public float flatIncrease;
    public float mpl;
    public float timeLeft;
    public bool permanent = false;
    public StatType type;

    public StatChange(StatType _type, float _flatIncrease, float _time, float _mpl)
    {
        flatIncrease = _flatIncrease;
        mpl = _mpl;
        type = _type;
        timeLeft = _time;
    }
    public StatChange(StatType _type, float _flatIncrease, float _mpl = 1f)
    {
        flatIncrease = _flatIncrease;
        mpl = _mpl;
        type = _type;
        permanent = true;
    }
}