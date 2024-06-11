namespace TokenApi11062024.Model
{
    public class AppSettings
    {
        public int PaginationOffset { get; set; }
        public int TokenValidityInMinutes { get; set; }
        public Jwt Jwt { get; set; }
    }

    public class Jwt
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Key { get; set; }
    }
}
