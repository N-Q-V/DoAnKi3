namespace CallApi.Models.Authen
{
    public class UpdateUserViewModel
    {
        public int UserId { get; set; }    
        public string? Email { get; set; }
        public string FullName { get; set; }
        public string Phonenumber { get; set; }
        public string Adress { get; set; }
    }
}
