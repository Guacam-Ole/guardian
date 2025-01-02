using Mastonet;

using Newtonsoft.Json;

namespace Limerick
{
    public class Mastodon
    {
        private static MastodonClient Login()
        {
            var secrets = JsonConvert.DeserializeObject<Secrets>(File.ReadAllText("secrets.json")) ?? throw new Exception("cannot read secrets");
            return new MastodonClient(secrets.Instance, secrets.AccessToken);
        }

        public async Task SendArticleToMastodon(Article article)
        {
            try
            {
                string hashtags = "#GuardianLimerick ";
                if (article.Categories != null) hashtags += string.Join(" ", article.Categories);
                string contents = article.Title + "\n\n" + hashtags + "\n\n" + article.Url;
                if (contents.Length > 499) contents = contents[..499];
                var client = Login();
                var mainEntry = await client.PublishStatus(contents, Visibility.Public);
                Console.WriteLine($"Sent '{contents.Length}' chars to mastodon");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed sending to mastodon: " + ex.Message);
            }
        }
    }
}