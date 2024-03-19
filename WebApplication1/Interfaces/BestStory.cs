namespace WebApplication1.Interfaces;

public class BestStory
{
    public string By { get; set; }
    public int Descendants { get; set; }
    public int Id { get; set; }
    public List<int> Kids { get; set; }
    public int Score { get; set; }
    public int Time { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public string Url { get; set; }
}