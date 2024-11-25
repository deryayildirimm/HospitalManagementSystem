using Pusula.Training.HealthCare.Doctors;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.BloodTests.Categories
{
    public class TestCategory : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Name { get; set; }
        [NotNull]
        public virtual string Description { get; set; }
        [NotNull]
        public virtual string Url { get; set; }
        [NotNull]
        public virtual double Price { get; set; }

        protected TestCategory()
        {
            Name = string.Empty;
            Description = string.Empty;
            Url = string.Empty;
            Price = 0;
        }
        public TestCategory(Guid id, string name, string description, string url, double price)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name), BloodTestConst.CategoryNameMax, BloodTestConst.CategoryNameMin);
            Check.NotNullOrWhiteSpace(description, nameof(description), BloodTestConst.CategoryDescriptionMax, BloodTestConst.CategoryDescriptionMin);
            Check.NotNullOrWhiteSpace(price.ToString(), nameof(price));

            Id = id;
            Name = name;
            Description = description;
            Url = url;
            Price= price;
        }
    }
}
