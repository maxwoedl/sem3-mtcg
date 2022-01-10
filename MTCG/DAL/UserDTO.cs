namespace MTCG.DAL
{
    public class UserDTO
    {
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string Name { get; set; }
        
        public string Bio { get; set; }
        
        public string Image { get; set; }
        
        public int Coins { get; set; }
        
        public int Elo { get; set; }
    }
}