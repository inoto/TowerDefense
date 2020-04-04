namespace TowerDefense
{
    public enum MoveSpeedType
    {
        Tempo, // run
        Interval, // run -> walk -> run
        Fartlek // run -> run fast -> run
    }

    public enum MoveSpeed
    {
        Slowest = 30,
        Walk = 40,
        Hustle = 50,
        Run = 60,
        Sprint = 70
    }
}