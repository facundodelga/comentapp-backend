namespace comentapp.infrastructure.Service
{
    public interface IEmailTemplateRenderer
    {
        Task<string> RenderAsync(string templateName, Dictionary<string, string> variables);
    }
}
