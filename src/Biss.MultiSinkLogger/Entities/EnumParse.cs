namespace Biss.MultiSinkLogger.Entities
{
    /// <summary>
    /// Utilitário para conversão segura de strings para enums.
    /// </summary>
    public static class EnumParse
    {
        /// <summary>
        /// Converte uma string para um enum de forma segura.
        /// </summary>
        /// <typeparam name="T">Tipo do enum.</typeparam>
        /// <param name="value">Valor a ser convertido.</param>
        /// <returns>Valor do enum.</returns>
        /// <exception cref="ArgumentException">Lançada quando o valor não é válido ou está vazio.</exception>
        public static T ParseEnum<T>(this string value) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Valor não pode ser nulo ou vazio.", nameof(value));
            }

            if (Enum.TryParse<T>(value, true, out var result))
            {
                return result;
            }

            var validValues = string.Join(", ", Enum.GetNames(typeof(T)));
            throw new ArgumentException(
                $"Valor '{value}' não é um {typeof(T).Name} válido. Valores válidos: {validValues}",
                nameof(value));
        }

        /// <summary>
        /// Tenta converter uma string para um enum.
        /// </summary>
        /// <typeparam name="T">Tipo do enum.</typeparam>
        /// <param name="value">Valor a ser convertido.</param>
        /// <param name="result">Resultado da conversão.</param>
        /// <returns>True se a conversão foi bem-sucedida.</returns>
        public static bool TryParseEnum<T>(this string value, out T result) where T : struct, Enum
        {
            result = default;
            return !string.IsNullOrWhiteSpace(value) && Enum.TryParse<T>(value, true, out result);
        }
    }
}
