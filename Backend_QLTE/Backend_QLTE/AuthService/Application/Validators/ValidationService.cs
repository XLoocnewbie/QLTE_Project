using Backend_QLTE.AuthService.Application.Interfaces.Validators;

namespace Backend_QLTE.AuthService.Application.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        // Validate DTO
        public async Task ValidateAsync<T>(T dto, CancellationToken cancellationToken = default)
        {
            var validators = _serviceProvider.GetServices<IValidator<T>>();

            foreach (var validator in validators)
            {
                await validator.ValidateAsync(dto, cancellationToken);
            }
        }

        // Validate DTO có thêm context
        public async Task ValidateAsync<T, TContext>(T dto, TContext context, CancellationToken cancellationToken = default)
        {
            var validators = _serviceProvider.GetServices<IContextValidator<T, TContext>>();

            foreach (var validator in validators)
            {
                await validator.ValidateAsync(dto, context, cancellationToken);
            }
        }

        public async Task<TEntity> ValidateAsync<T, TEntity>(T dto, CancellationToken cancellationToken = default)
        {
            var validators = _serviceProvider.GetServices<IEntityValidator<T, TEntity>>();

            foreach (var validator in validators)
            {
                return await validator.ValidateAndGetAsync(dto, cancellationToken);
            }

            throw new InvalidOperationException($"No IEntityValidator found for {typeof(T).Name} -> {typeof(TEntity).Name}");
        }

    }
}
