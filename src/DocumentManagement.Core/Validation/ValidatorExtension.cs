using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using FluentValidation;
using FluentValidation.Results;

namespace DocumentManagement.Core.Validation
{
    public static class ValidatorExtension
    {
        public static void ValidateCommand<TCommand>(this IValidator<TCommand> validator, TCommand command) where TCommand : ICommand
        {
            if (command == null)
                throw new ValidationException(ValidationConstants.EmptyCommand);

            var validationResult = validator.Validate(command);

            if (!validationResult.IsValid)
                throw new ValidationException(BuildErrorMesage(validationResult.Errors));
        }

        private static string BuildErrorMesage(IEnumerable<ValidationFailure> errors)
        {
            var errorsText = errors.Select(x => "\r\n " + x.ErrorMessage).ToArray();
            return string.Join("", errorsText);
        }
    }
}

