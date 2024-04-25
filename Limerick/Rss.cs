using Microsoft.SyndicationFeed.Rss;
using Microsoft.SyndicationFeed;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Limerick
{

    public record Article
    {
        public string Title { get; set; }
        public string Url { get; set; } 
        public DateTimeOffset Date { get; set; }
    }

    public class Rss
    {
        private const string _url = "https://www.theguardian.com/world/europe-news/rss";

        public  async   Task< List<Article>> GetArticlesSince(DateTimeOffset? since) {
            since ??= DateTimeOffset.Now.AddMinutes(-15);
            var articlesSince = await ReadFeedSince(_url, since.Value);
            var articles = articlesSince.Select(q => new Article { Date = q.Published, Title = q.Title, Url = q.Links.First().Uri.ToString() }).ToList();
            return articles;
        }

        public async Task<List<ISyndicationItem>> ReadFeedSince(string url, DateTimeOffset lastKnownEntry)
        {
            List<ISyndicationItem> items = new List<ISyndicationItem>();

            using (var xmlReader = XmlReader.Create(url, new XmlReaderSettings() { Async = true }))
            {
                var feedReader = new RssFeedReader(xmlReader);

                while (await feedReader.Read())
                {
                    if (feedReader.ElementType != SyndicationElementType.Item) continue;
                    ISyndicationItem item = await feedReader.ReadItem();
                    if (item.Published <= lastKnownEntry) continue;
                    items.Add(item);
                }
            }
            return items;
        }

    }
}
