using Sitecore.Buckets.FieldTypes;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Support.XA.Foundation.Variants.Abstractions.DataSource;

namespace Sitecore.Support.XA.Foundation.LocalDatasources.Services
{
  public class CodeDatasourceService : ICodeDatasourceService
  {
    public Item[] GetDatasoureces(Item currentItem, string typeName)
    {
      IDataSource instance;
      try
      {
        object[] objArray = new object[0];
        Match match = Regex.Match(typeName, "\\((.)*\\)", RegexOptions.Compiled);
        if (match.Success)
        {
          objArray = this.GetConstructorParameters(match.Value);
          typeName = typeName.Replace(match.Value, string.Empty);
        }
        instance = ReflectionUtility.CreateInstance(Type.GetType(typeName), objArray) as IDataSource;
        if (instance != null && instance.GetType().IsInstanceOfType(new Sitecore.XA.Foundation.Variants.Abstractions.DataSource.AvailableRenderingVariants()))
        {
          instance = new AvailableRenderingVariants();
        }
      }
      catch
      {
        Log.SingleError("Could not load IDataSource type: " + typeName, (object)this);
        return new Item[0];
      }
      if (currentItem != null && instance != null)
        return instance.ListQuery(currentItem) ?? new Item[0];
      return new Item[0];
    }

    protected virtual object[] GetConstructorParameters(string parameters)
    {
      parameters = parameters.TrimStart('(').TrimEnd(')');
      return ((IEnumerable<string>)parameters.Split(',')).Select<string, object>((Func<string, object>)(s => (object)s.TrimStart('\'').TrimEnd('\''))).ToArray<object>();
    }
  }
}