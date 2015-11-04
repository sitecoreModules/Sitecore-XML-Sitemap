using ICR.SC.Framework.Helpers;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using System;
using System.Xml;
using Sitecore.Tasks;

namespace ICR.SC.Internet.Website.Logic.Tasks
{
    public class ScheduleContainer
    {       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="command"></param>
        /// <param name="schedule"></param>
        public void SetXmLsitemap(Item[] items, CommandItem command, ScheduleItem schedule)
        {   
            if (ConfigurationHelper.IsAuthoringServer())
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);

                    XmlElement rootNode = xmlDoc.CreateElement("urlset");
                    rootNode.SetAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
                    xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
                    xmlDoc.AppendChild(rootNode);

                    //we will take only approved items / pages
                    string databaseName = "web";
                    string startItemPath = ConfigurationHelper.GetSitecoreSetting("rootPath");
                    Database database = Factory.GetDatabase(databaseName);
                    UrlOptions urlOptions = UrlOptions.DefaultOptions;
                    urlOptions.AlwaysIncludeServerUrl = false;

                    Item item = database.GetItem(startItemPath);
                    AddUrlEntry(item, xmlDoc, rootNode, urlOptions);
                    xmlDoc.Save(System.Web.Hosting.HostingEnvironment.MapPath("/sitemap.xml"));                    
                }
                catch (Exception ex)
                {
                    Log.Error("Error at sitemap xml handler.", ex, this);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pItem"></param>
        /// <param name="pXmlDoc"></param>
        /// <param name="pRootNode"></param>
        /// <param name="pUrlOptions"></param>
        public void AddUrlEntry(
          Item pItem, XmlDocument pXmlDoc, XmlElement pRootNode, UrlOptions pUrlOptions)
        {
            if (pItem != null && pItem.Visualization != null && pItem.Visualization.Layout != null && pItem.Name != "*")
            {
                if (pItem["Hide From Navigation"] != "1")
                {
                    string url = "www.clariant.com" + UrlHelper.GetUrl(pItem);
                    url = url.Replace("sitecore/content/", "");
                    GenerateUrlEntry(pXmlDoc, pRootNode, url);
                }
            }

            if (pItem != null)
            {
                foreach (Item childItem in pItem.Children)
                {
                    AddUrlEntry(childItem, pXmlDoc, pRootNode, pUrlOptions);
                }
            }
        }

        /// <summary>
        /// The generate url entry.
        /// </summary>
        /// <param name="pXmlDoc">
        /// The xml doc.
        /// </param>
        /// <param name="pRootNode">
        /// The root node.
        /// </param>
        /// <param name="pUrl">
        /// The url.
        /// </param>
        private void GenerateUrlEntry(XmlDocument pXmlDoc, XmlElement pRootNode, string pUrl)
        {
            XmlElement sitemapNode = pXmlDoc.CreateElement("url");
            pRootNode.AppendChild(sitemapNode);

            XmlElement locationNode = pXmlDoc.CreateElement("loc");
            locationNode.AppendChild(pXmlDoc.CreateTextNode(pUrl));
            sitemapNode.AppendChild(locationNode);
        }

    }
}