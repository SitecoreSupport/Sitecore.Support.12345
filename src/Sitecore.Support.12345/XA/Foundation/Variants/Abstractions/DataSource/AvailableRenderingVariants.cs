using Sitecore.Buckets.FieldTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sitecore.Support.XA.Foundation.Variants.Abstractions.DataSource
{
  public class AvailableRenderingVariants : IDataSource
  {
    private Item _renderingItem;

    public Item[] ListQuery(Item item)
    {
      string url = Context.RawUrl;
      if (!string.IsNullOrWhiteSpace(url) && url.Contains("hdl"))
      {
        string renderingName = GetRenderingName(item, FieldEditorOptions.Parse(new UrlString(url)).Parameters["rendering"]);
        string pageTemplateId = GetPageTemplateId(item, FieldEditorOptions.Parse(new UrlString(url)).Parameters["contentitem"]);

        if (!string.IsNullOrWhiteSpace(renderingName) && !string.IsNullOrWhiteSpace(pageTemplateId))
        {
          var variantService = new Sitecore.Support.XA.Foundation.Variants.Abstractions.Services.AvailableRenderingVariantService();
          return variantService.GetAvailableRenderingVariants(item, renderingName, pageTemplateId).ToArray(); 
        }
      }
      return new Item[0];
    }

    protected virtual Item GetRenderingItem(Item item, string id)
    {
      return _renderingItem ?? (_renderingItem = item.Database.GetItem(new ID(id)));
    }

    /// <summary>
    /// Gets the name of the sublayout for which rendering parameters we are opening
    /// </summary>
    protected virtual string GetRenderingName(Item item, string rendering)
    {
      Match match = Regex.Match(rendering, @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b");
      if (match.Success)
      {
        if (ID.IsID(match.Value))
        {
          Item renderingItem = GetRenderingItem(item, match.Value);
          return renderingItem.Name;
        }
      }
      return string.Empty;
    }

    /// <summary>
    /// Get id of the page template, on which component is placed
    /// </summary>
    protected virtual string GetPageTemplateId(Item item, string rendering)
    {
      Match match = Regex.Match(rendering, @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b");
      if (match.Success)
      {
        if (ID.IsID(match.Value))
        {
          Item page = item.Database.GetItem(new ID(match.Value));
          return page.TemplateID.ToString();
        }
      }
      return string.Empty;
    }
  }
}


