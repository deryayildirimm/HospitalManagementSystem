using System;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.BloodTests.Categories
{
    public class TestCategory : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Name { get; private set; } = string.Empty;
        [NotNull]
        public virtual string Description { get; private set; } = string.Empty;
        [NotNull]
        public virtual string Url { get; private set; } = string.Empty;
        [NotNull]
        public virtual double Price { get; private set; }

        protected TestCategory()
        {
            Price = 0;
        }

        public TestCategory(Guid id, string name, string description, string url, double price)
        {
            Id = id;
            SetName(name);
            SetDescription(description);
            SetUrl(url);
            SetPrice(price);
        }

        public void SetName(string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name), BloodTestConst.CategoryNameMax, BloodTestConst.CategoryNameMin);
            Name = name;
        }

        public void SetDescription(string description)
        {
            Check.NotNullOrWhiteSpace(description, nameof(description), BloodTestConst.CategoryDescriptionMax, BloodTestConst.CategoryDescriptionMin);
            Description = description;
        }

        public void SetUrl(string url)
        {
            Check.NotNullOrWhiteSpace(url, nameof(url));
            Url = url;
        }

        public void SetPrice(double price)
        {
            Check.NotNullOrWhiteSpace(price.ToString(), nameof(price));
            Price = price;
        }
    }
}
