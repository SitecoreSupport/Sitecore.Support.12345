using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetLookupSourceItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Support.XA.Foundation.LocalDatasources.Pipelines.GetLookupSourceItems
{
  public class CodeDatasource
  {
    public Services.ICodeDatasourceService CodeDatasourceService { get; set; }

    public CodeDatasource()
    {
      this.CodeDatasourceService = new Services.CodeDatasourceService();
    }

    public void Process(GetLookupSourceItemsArgs args)
    {
      Assert.IsNotNull((object)args, nameof(args));
      if (!args.Source.StartsWith("code:"))
        return;
      foreach (string oldValue in ((IEnumerable<string>)args.Source.Split('|')).Where<string>((Func<string, bool>)(x => x.StartsWith("code:"))))
      {
        GetLookupSourceItemsArgs lookupSourceItemsArgs = args;
        string str = lookupSourceItemsArgs.Source.Replace(oldValue, string.Empty).TrimStart('|').TrimEnd('|');
        lookupSourceItemsArgs.Source = str;
        string typeName = oldValue.Replace("code:", string.Empty);
        if (args.Item != null)
        {
          Item[] datasoureces = this.CodeDatasourceService.GetDatasoureces(args.Item, typeName);
          if (((IEnumerable<Item>)datasoureces).Any<Item>())
            args.Result.AddRange(datasoureces);
        }
      }
      if (args.Source.Length != 0)
        return;
      args.AbortPipeline();
    }
  }
}
