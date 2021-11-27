namespace Hokm
{
    public record ValidationResult
    {
        public string Error { get; init; }
        
        public bool IsValid { get; init; }

        public static readonly ValidationResult Valid = new ValidationResult() {IsValid = true};

        public static ValidationResult ErrorResult(string error)
        {
            return new ValidationResult { Error = error, IsValid = false};
        }
    }
}