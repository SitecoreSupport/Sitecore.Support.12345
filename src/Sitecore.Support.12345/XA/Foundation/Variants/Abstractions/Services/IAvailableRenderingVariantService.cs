using Sitecore.Data.Items;
using System.Collections.Generic;

namespace Sitecore.Support.XA.Foundation.Variants.Abstractions.Services
{
  public interface IAvailableRenderingVariantService
  {
    IEnumerable<Item> GetAvailableRenderingVariants(Item item, string componentName, string pageTemplateId);
  }
}