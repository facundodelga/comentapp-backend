namespace comentapp_authentication_manager.Services
{
    public interface IEmailTemplateRenderer
    {
        Task<string> RenderAsync(string templateName, Dictionary<string, string> variables);
    }
}
