public interface IBurst
{
    public int id { get; set; }
    public int burst { get; set; }
    public int max_burst { get; set; }
    public void Damage(int value, int id);
}