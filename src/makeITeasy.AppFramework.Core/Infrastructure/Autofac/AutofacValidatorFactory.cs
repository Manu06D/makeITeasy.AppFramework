using System;
using Autofac;
using FluentValidation;

namespace makeITeasy.AppFramework.Core.Infrastructure.Autofac
{
    public class AutofacValidatorFactory_old : ValidatorFactoryBase
    {
        private readonly IComponentContext _context;

        public AutofacValidatorFactory_old(IComponentContext context)
        {
            _context = context;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            object instance;

            if (_context.TryResolve(validatorType, out instance))
            {
                var validator = instance as IValidator;

                return validator;
            }

            return null;
        }
    }
}
