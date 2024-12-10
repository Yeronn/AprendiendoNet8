using System.ComponentModel.DataAnnotations;

namespace Application.Validators.Attributes
{
    public class CompareDatesAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public CompareDatesAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success; // No hay nada que validar si el valor actual es nulo.

            var currentValue = (DateTime)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                throw new ArgumentException($"No se encontró la propiedad '{_comparisonProperty}' en el objeto.");

            var comparisonValueObj = property.GetValue(validationContext.ObjectInstance);
            if (comparisonValueObj == null)
                return new ValidationResult(ErrorMessage ?? $"{_comparisonProperty} no puede ser nulo.");

            var comparisonValue = (DateTime)comparisonValueObj;

            if (currentValue <= comparisonValue)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} debe ser mayor que {_comparisonProperty}.");
            }

            return ValidationResult.Success;
        }
    }
}