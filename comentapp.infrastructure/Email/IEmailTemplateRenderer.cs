namespace comentapp.infrastructure.Email
{
    public interface IEmailTemplateRenderer
    {
        Task<string> RenderAsync(string templateName, Dictionary<string, string> variables);
    }
}
