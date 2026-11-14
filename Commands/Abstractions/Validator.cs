using System.Globalization;
using unhsjm_asistencia_backend.Exceptions;

namespace unhsjm_asistencia_backend.Commands.Abstractions
{
    public sealed class Validator
    {
        private readonly List<Func<(bool IsValid, string FieldKey, string ErrorMessage)>> _rules = [];
        private string _currentFieldKey = string.Empty;
        private object? _currentValue;
        private string _currentDisplayName = string.Empty;

        public static Validator Create() => new();

        public Validator Field(string fieldKey, object? value, string? displayName = null)
        {
            _currentFieldKey = fieldKey;
            _currentValue = value;
            _currentDisplayName = string.IsNullOrWhiteSpace(displayName) ? fieldKey : displayName;
            return this;
        }

        public Validator NotEmpty(string? errorMessage = null)
        {
            var (fieldKey, value, displayName) = CaptureContext();
            var message = errorMessage ?? $"{displayName} es requerido.";

            _rules.Add(() =>
            {
                var isValid = value is string text && !string.IsNullOrWhiteSpace(text);
                return (isValid, fieldKey, message);
            });

            return this;
        }

        public Validator MaxLength(int maxLength, string? errorMessage = null)
        {
            var (fieldKey, value, displayName) = CaptureContext();
            var message = errorMessage ?? $"{displayName} debe tener maximo {maxLength} caracteres.";

            _rules.Add(() =>
            {
                if (value is not string text || string.IsNullOrWhiteSpace(text))
                {
                    return (true, fieldKey, message);
                }

                return (text.Trim().Length <= maxLength, fieldKey, message);
            });

            return this;
        }

        public Validator RequiredGuid(string? errorMessage = null)
        {
            var (fieldKey, value, displayName) = CaptureContext();
            var message = errorMessage ?? $"Id {displayName} tipo UUID valido es requerido.";

            _rules.Add(() =>
            {
                var isValid = value is Guid guid && guid != Guid.Empty;
                return (isValid, fieldKey, message);
            });

            return this;
        }

        public Validator Positive(string? errorMessage = null)
        {
            var (fieldKey, value, displayName) = CaptureContext();
            var message = errorMessage ?? $"{displayName} debe ser mayor que 0.";

            _rules.Add(() =>
            {
                var isValid = TryToDecimal(value, out var number) && number > 0m;
                return (isValid, fieldKey, message);
            });

            return this;
        }

        public Validator Between(decimal min, decimal max, string? errorMessage = null)
        {
            var (fieldKey, value, displayName) = CaptureContext();
            var message = errorMessage ?? $"{displayName} debe estar entre {min} y {max}.";

            _rules.Add(() =>
            {
                var isValid = TryToDecimal(value, out var number) && number >= min && number <= max;
                return (isValid, fieldKey, message);
            });

            return this;
        }

        public Validator AllowedValues(IEnumerable<string> allowedValues, string? errorMessage = null)
        {
            var (fieldKey, value, displayName) = CaptureContext();
            var allowedSet = allowedValues
                .Select(v => v.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var message = errorMessage ?? $"{displayName} debe ser: {string.Join(", ", allowedSet)}.";

            _rules.Add(() =>
            {
                var normalized = (value as string)?.Trim() ?? string.Empty;
                return (allowedSet.Contains(normalized), fieldKey, message);
            });

            return this;
        }

        public Validator Must(Func<bool> predicate, string errorMessage)
        {
            var (fieldKey, _, _) = CaptureContext();
            _rules.Add(() => (predicate(), fieldKey, errorMessage));
            return this;
        }

        public Validator Rule(string fieldKey, Func<bool> predicate, string errorMessage)
        {
            _rules.Add(() => (predicate(), fieldKey, errorMessage));
            return this;
        }

        public IReadOnlyDictionary<string, string[]> Validate()
        {
            var errors = new Dictionary<string, List<string>>(StringComparer.Ordinal);

            foreach (var rule in _rules)
            {
                var (isValid, fieldKey, errorMessage) = rule();
                if (isValid)
                {
                    continue;
                }

                if (!errors.TryGetValue(fieldKey, out var fieldErrors))
                {
                    fieldErrors = [];
                    errors[fieldKey] = fieldErrors;
                }

                if (!fieldErrors.Contains(errorMessage))
                {
                    fieldErrors.Add(errorMessage);
                }
            }

            return errors.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToArray(),
                StringComparer.Ordinal);
        }

        public void ValidateAndThrow(string summaryMessage = "Ocurrieron uno o mas errores de validacion.")
        {
            var errors = Validate();
            if (errors.Count > 0)
            {
                throw new ValidationApiException(
                    summaryMessage,
                    new Dictionary<string, string[]>(errors, StringComparer.Ordinal));
            }
        }

        private (string FieldKey, object? Value, string DisplayName) CaptureContext()
        {
            if (string.IsNullOrWhiteSpace(_currentFieldKey))
            {
                throw new InvalidOperationException("Call Field(...) before chaining field-level validation rules.");
            }

            return (_currentFieldKey, _currentValue, _currentDisplayName);
        }

        private static bool TryToDecimal(object? value, out decimal result)
        {
            switch (value)
            {
                case byte b:
                    result = b;
                    return true;
                case sbyte sb:
                    result = sb;
                    return true;
                case short s:
                    result = s;
                    return true;
                case ushort us:
                    result = us;
                    return true;
                case int i:
                    result = i;
                    return true;
                case uint ui:
                    result = ui;
                    return true;
                case long l:
                    result = l;
                    return true;
                case ulong ul:
                    result = ul;
                    return true;
                case float f:
                    result = Convert.ToDecimal(f, CultureInfo.InvariantCulture);
                    return true;
                case double d:
                    result = Convert.ToDecimal(d, CultureInfo.InvariantCulture);
                    return true;
                case decimal dec:
                    result = dec;
                    return true;
                default:
                    result = 0m;
                    return false;
            }
        }
    }
}
