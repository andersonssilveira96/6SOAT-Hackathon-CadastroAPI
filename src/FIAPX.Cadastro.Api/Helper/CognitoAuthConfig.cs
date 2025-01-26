namespace FIAPX.Cadastro.Api.Helper
{
    public class CognitoAuthConfig
    {
        public string Region { get; set; } = string.Empty;
        public string UserPoolId { get; set; } = string.Empty;
        public string UserPoolClientId { get; set; } = string.Empty;
        public string ClientPoolSecret { get; set; } = string.Empty;
        public string CognitoUri { get => $"https://cognito-idp.{Region}.amazonaws.com/{UserPoolId}"; }
    }
}
