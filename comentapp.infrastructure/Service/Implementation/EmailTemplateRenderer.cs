using Microsoft.AspNetCore.Hosting;
namespace comentapp.infrastructure.Service
{
    public class EmailTemplateRenderer : IEmailTemplateRenderer
    {
        private readonly IWebHostEnvironment _env;

        public EmailTemplateRenderer(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> RenderAsync(string templateName, Dictionary<string, string> variables)
        {
            var path = Path.Combine(
                            AppContext.BaseDirectory,
                            "Templates",
                            "Emails",
                            templateName
                        );

            if (!File.Exists(path))
                throw new FileNotFoundException($"Template no encontrado: {path}");

            var html = await File.ReadAllTextAsync(path);

            foreach (var (key, value) in variables)
                html = html.Replace($"{{{{{key}}}}}", value);  // reemplaza {{Key}}

            return html;
        }
    }
}
