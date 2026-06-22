public interface IBurst
{
    public int id { get; set; }
    public int burst { get; set; }
    public int max_burst { get; set; }

    /// <summary> ダメージを与える </summary>
    /// <param name="value"> ダメージ量 </param>
    /// <param name="id"> ダメージの識別 </param>
    public void Damage(int value, int id);
}