namespace SSL.Models
{
    public class HomeModel
    {
        public HomeModel(string msg)
        {
            Message = msg;
        }
        public string Message { get; set; }
    }
}