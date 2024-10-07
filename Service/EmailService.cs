
using RestSharp;
using RestSharp.Authenticators;

namespace Auth.Service
{
    public class EmailService(IConfiguration config)
    {
        private readonly IConfiguration _config = config;
        public bool SendEmail(string body, string email)
        {
            Console.WriteLine(_config.GetSection("EmailConfig").Value);
            var ApiKey = _config.GetSection("EmailConfig").Value;
            if (ApiKey is null)
            {
                return false;
            }
            RestClientOptions options = new("https://api.mailgun.net/v3")
            {
                Authenticator = new HttpBasicAuthenticator("api", ApiKey)
            };
            RestClient client = new(options);
            RestRequest request = new("", Method.Post);
            request.AddParameter("domain", "sandbox71d423a509574eefb9fb6c59db099a05.mailgun.org",ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
  		    request.AddParameter ("from", "Excited User <mailgun@sandbox71d423a509574eefb9fb6c59db099a05.mailgun.org>");
            request.AddParameter("to", "baral.aaryan1100@gmail.com");
            request.AddParameter("subject", "Email Verification ");
            request.AddParameter("text", body);
            request.Method = Method.Post;
            var response = client.Execute(request);
            Console.WriteLine(response.ErrorMessage);
            return response.IsSuccessful;
        }
    }
}