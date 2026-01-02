using System;
using FastEndpoints;
using FluentValidation;

namespace AI.OfferService.Domain.OfferFeature
{
    public class CreateOfferValidator : Validator<CreateOfferRequestDto>
    {
        public CreateOfferValidator()
        {
            RuleFor(x => x.SellerId).GreaterThan(0);
            RuleFor(x => x.VIN).NotEmpty().Length(17);
            RuleFor(x => x.OfferAmount).GreaterThan(0);
            RuleFor(x => x.Status).NotEmpty().Must(s => s == "OPEN" || s == "SOLD" || s == "CANCELLED");
            RuleFor(x => x.Year).GreaterThan(1900).LessThanOrEqualTo(DateTime.UtcNow.Year + 1);
            RuleFor(x => x.Make).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Trim).MaximumLength(100);
        }
    }
}
