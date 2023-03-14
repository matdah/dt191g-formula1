namespace Formula1TeamApp.Models
{
    public class MiniDriver
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string? Image { get; set; }
        public string? Team { get; internal set; }

    }
}