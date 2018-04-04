using Sitecore.Data.Items;

namespace Sitecore.Support.XA.Foundation.LocalDatasources.Services
{
  public interface ICodeDatasourceService
  {
    Item[] GetDatasoureces(Item contextItem, string typeName);
  }
}