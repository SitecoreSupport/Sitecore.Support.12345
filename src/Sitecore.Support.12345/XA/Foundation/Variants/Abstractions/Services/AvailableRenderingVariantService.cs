using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.XA.Foundation.IoC;
using Sitecore.XA.Foundation.Presentation;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
using Sitecore.XA.Foundation.SitecoreExtensions.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Support.XA.Foundation.Variants.Abstractions.Services
{
  public class AvailableRenderingVariantService : IAvailableRenderingVariantService
  {
    public IContentRepository ContentRepository { get; set; }
    public IPresentationContext PresentationContext { get; set; }

    public ID[] AllowedTemplates { get; set; } = {
      Sitecore.XA.Foundation.Presentation.Templates.MetadataPartialDesign.ID,
      Sitecore.XA.Foundation.Presentation.Templates.PartialDesign.ID,
      Sitecore.XA.Foundation.Presentation.Templates.Design.ID,
      Sitecore.XA.Foundation.Multisite.Templates.Page.ID
        };

    public AvailableRenderingVariantService()
    {
      ContentRepository = ServiceLocator.Current.Resolve<IContentRepository>();
      PresentationContext = ServiceLocator.Current.Resolve<IPresentationContext>();
    }

    public IEnumerable<Item> GetAvailableRenderingVariants(Item item, string componentName, string pageTemplateId)
    {
      Item presentationItem = PresentationContext.GetPresentationItem(item);

      var availableComponentVariants = GetSystemVariants(componentName);
      if (presentationItem != null)
      {
        IEnumerable<Item> siteVariants = GetSiteVariants(componentName, pageTemplateId, presentationItem);
        availableComponentVariants = availableComponentVariants.Concat(siteVariants);
      }

      if (InheritsFromAllowedTemplate(pageTemplateId))
      {
        return availableComponentVariants;
      }
      return availableComponentVariants.Where(i => AllowedInTemplate(i, pageTemplateId));
    }

    protected virtual IEnumerable<Item> GetSiteVariants(string componentName, string pageTemplateId, Item presentationItem)
    {
      var variantsGrouping = presentationItem.FirstChildInheritingFrom(Sitecore.XA.Foundation.Variants.Abstractions.Templates.VariantsGrouping.ID);
      var root = variantsGrouping?.Children.FirstOrDefault(item => item.Name.Equals(componentName));
      if (root != null)
      {
        return root.Children;
      }
      return new Item[0];
    }

    protected virtual IEnumerable<Item> GetSystemVariants(string componentName)
    {
      var variantsGrouping = ContentRepository.GetItem(Sitecore.XA.Foundation.Variants.Abstractions.Items.SystemVariants);
      var root = variantsGrouping?.Children.FirstOrDefault(item => item.Name.Equals(componentName));
      if (root != null)
      {
        return root.Children;
      }
      return new Item[0];
    }

    protected virtual bool AllowedInTemplate(Item item, string pageTemplateId)
    {
      var field = item.Fields[Sitecore.XA.Foundation.Variants.Abstractions.Templates.IVariantDefinition.Fields.AllowedInTemplates];
      if (field != null)
      {
        return field.Value == string.Empty || field.Value.Contains(pageTemplateId);
      }
      return true;
    }

    protected virtual bool InheritsFromAllowedTemplate(string pageTemplateId)
    {
      var templateItem = ServiceLocator.Current.Resolve<IContentRepository>().GetTemplate(new ID(pageTemplateId));
      return AllowedTemplates.Any(id => templateItem.DoesTemplateInheritFrom(id));
    }
  }
}
