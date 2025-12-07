using System.Text.RegularExpressions;

namespace Biss.MultiSinkLogger.Security
{
    /// <summary>
    /// Filtra dados sensíveis de logs para evitar exposição de informações confidenciais.
    /// </summary>
    public static class SensitiveDataFilter
    {
        private static readonly string[] SensitiveHeaders = new[]
        {
            "Authorization",
            "X-API-Key",
            "X-Auth-Token",
            "Cookie",
            "Set-Cookie",
            "X-Api-Key",
            "Api-Key"
        };

        private static readonly string[] SensitivePatterns = new[]
        {
            // Padrão para password (formato key=value ou key:value)
            @"(?i)(""password""|""passwd""|""pwd""|password|passwd|pwd)\s*[:=]\s*[""']?([^""'\s,}]+)[""']?",
            // Padrão para token (incluindo JSON)
            @"(?i)(""token""|""apikey""|""api_key""|""access_token""|""bearer""|token|apikey|api_key|access_token|bearer)\s*[:=]\s*[""']?([^""'\s,}]+)[""']?",
            // Padrão para secret
            @"(?i)(""secret""|""secretkey""|""secret_key""|""private_key""|secret|secretkey|secret_key|private_key)\s*[:=]\s*[""']?([^""'\s,}]+)[""']?",
            // Padrão para credit card
            @"(?i)(""creditcard""|""cc""|""cardnumber""|""card_number""|creditcard|cc|cardnumber|card_number)\s*[:=]\s*[""']?([^""'\s,}]+)[""']?",
            // Padrão para SSN
            @"(?i)(""ssn""|""social_security""|ssn|social_security)\s*[:=]\s*[""']?([^""'\s,}]+)[""']?"
        };

        /// <summary>
        /// Filtra headers sensíveis de um dicionário.
        /// </summary>
        /// <param name="headers">Dicionário de headers.</param>
        /// <returns>Dicionário com headers sensíveis filtrados.</returns>
        public static Dictionary<string, string> FilterHeaders(Dictionary<string, string> headers)
        {
            if (headers == null)
                return new Dictionary<string, string>();

            var filtered = new Dictionary<string, string>(headers);

            foreach (var sensitiveHeader in SensitiveHeaders)
            {
                // Verificar com case-insensitive
                var key = filtered.Keys.FirstOrDefault(k => 
                    string.Equals(k, sensitiveHeader, StringComparison.OrdinalIgnoreCase));
                
                if (key != null)
                {
                    filtered[key] = "[REDACTED]";
                }
            }

            return filtered;
        }

        /// <summary>
        /// Filtra dados sensíveis de uma string (body, query string, etc.).
        /// </summary>
        /// <param name="content">Conteúdo a ser filtrado.</param>
        /// <returns>Conteúdo com dados sensíveis filtrados.</returns>
        public static string FilterSensitiveData(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            var filtered = content;

            foreach (var pattern in SensitivePatterns)
            {
                filtered = Regex.Replace(
                    filtered,
                    pattern,
                    match => $"{match.Groups[1].Value}: [REDACTED]",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }

            return filtered;
        }

        /// <summary>
        /// Sanitiza uma connection string removendo credenciais.
        /// </summary>
        /// <param name="connectionString">Connection string a ser sanitizada.</param>
        /// <returns>Connection string sanitizada.</returns>
        public static string SanitizeConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return connectionString;

            // Remove password, pwd, user id, etc.
            var sanitized = Regex.Replace(
                connectionString,
                @"(?i)(password|pwd|user\s*id|uid|user\s*name|uid)\s*=\s*[^;]+",
                match => $"{match.Groups[1].Value}=[REDACTED]",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return sanitized;
        }
    }
}

