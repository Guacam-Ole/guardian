using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;

using System.Xml;

namespace Limerick
{
    public record Article
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTimeOffset Date { get; set; }
        public string[] Categories { get; set; }
    }

    public class Rss
    {
        private const string _url = "https://www.theguardian.com/world/europe-news/rss";

        public async Task<IEnumerable<Article>> GetArticlesSince(DateTimeOffset? since)
        {
            bool firstCall = since == null;
            since ??= DateTimeOffset.Now.AddHours(-10);
            var articlesSince = await ReadFeedSince(_url, since.Value);
            var articles = articlesSince.Select(q => new Article
            {
                Date = q.Published,
                Title = q.Title,
                Url = q.Links.First().Uri.ToString(),
                Categories = q.Categories.Where(q => q.Name.All(Char.IsLetter)).Select(q => "#" + q.Name).ToArray()
            });

            if (firstCall && articles.Count() > 1)
            {
                return new[] { articles.MaxBy(q => q.Date) };
            }
            return articles;
        }

        public async Task<List<ISyndicationItem>> ReadFeedSince(string url, DateTimeOffset lastKnownEntry)
        {
            List<ISyndicationItem> items = [];

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