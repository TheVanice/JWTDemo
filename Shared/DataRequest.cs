namespace Shared
{
    public class DataRequest
    {
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

        public static DataRequest FromString(string message)
        {
            var parts = message.Split(';');
            return new DataRequest
            {
                Message = parts[0],
                Token = parts[1],
            };
        }

        public override string ToString()
        {
            return $"{Message};{Token}";
        }
    }
}